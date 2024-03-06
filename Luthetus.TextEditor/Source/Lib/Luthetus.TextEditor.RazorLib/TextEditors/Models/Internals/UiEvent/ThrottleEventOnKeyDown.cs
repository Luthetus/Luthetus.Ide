using Fluxor;
using Luthetus.Common.RazorLib.Clipboards.Models;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals.UiEvent;

public class ThrottleEventOnKeyDown : IThrottleEvent
{
    private readonly ThrottleController _throttleControllerUiEvents;
    private readonly TimeSpan _uiEventsDelay;
    private readonly CursorDisplay? _cursorDisplay;
    private readonly Func<ResourceUri, Key<TextEditorViewModel>, KeyboardEventArgs, Func<TextEditorMenuKind, bool, Task>, TextEditorEdit> _handleAfterOnKeyDownAsyncFactoryFunc;
    private readonly Func<ResourceUri, Key<TextEditorViewModel>, List<KeyboardEventArgs>, Func<TextEditorMenuKind, bool, Task>, TextEditorEdit> _handleAfterOnKeyDownRangeAsyncFactoryFunc;
    private readonly Action<TooltipViewModel?> _setTooltipViewModel;
    private readonly ITextEditorService _textEditorService;
    private readonly IClipboardService _clipboardService;
    private readonly Func<MouseEventArgs, Task>? _handleMouseStoppedMovingEventAsyncFunc;
    private readonly IJSRuntime? _jsRuntime;
    private readonly IDispatcher _dispatcher;
    private readonly IServiceProvider _serviceProvider;
    private readonly LuthetusTextEditorConfig _textEditorConfig;

    public ThrottleEventOnKeyDown(
        ThrottleController throttleControllerUiEvents,
        TimeSpan uiEventsDelay,
        KeyboardEventArgs keyboardEventArgs,
        TextEditorViewModelDisplayOptions viewModelDisplayOptions,
        CursorDisplay? cursorDisplay,
        ResourceUri resourceUri,
        Key<TextEditorViewModel> viewModelKey,
        Func<ResourceUri, Key<TextEditorViewModel>, KeyboardEventArgs, Func<TextEditorMenuKind, bool, Task>, TextEditorEdit> handleAfterOnKeyDownAsyncFactoryFunc,
        Func<ResourceUri, Key<TextEditorViewModel>, List<KeyboardEventArgs>, Func<TextEditorMenuKind, bool, Task>, TextEditorEdit> handleAfterOnKeyDownRangeAsyncFactoryFunc,
        Action<TooltipViewModel?> setTooltipViewModel,
        ITextEditorService textEditorService,
        IClipboardService clipboardService,
        Func<MouseEventArgs, Task>? handleMouseStoppedMovingEventAsyncFunc,
        IJSRuntime? jsRuntime,
        IDispatcher dispatcher,
        IServiceProvider serviceProvider,
        LuthetusTextEditorConfig textEditorConfig)
    {
        _throttleControllerUiEvents = throttleControllerUiEvents;
        _uiEventsDelay = uiEventsDelay;
        _textEditorService = textEditorService;
        _clipboardService = clipboardService;
        _handleMouseStoppedMovingEventAsyncFunc = handleMouseStoppedMovingEventAsyncFunc;
        _jsRuntime = jsRuntime;
        _dispatcher = dispatcher;
        _serviceProvider = serviceProvider;
        _textEditorConfig = textEditorConfig;
        _cursorDisplay = cursorDisplay;
        _handleAfterOnKeyDownAsyncFactoryFunc = handleAfterOnKeyDownAsyncFactoryFunc;
        _handleAfterOnKeyDownRangeAsyncFactoryFunc = handleAfterOnKeyDownRangeAsyncFactoryFunc;
        _setTooltipViewModel = setTooltipViewModel;

        KeyboardEventArgs = keyboardEventArgs;
        ViewModelDisplayOptions = viewModelDisplayOptions;
        ResourceUri = resourceUri;
        ViewModelKey = viewModelKey;

        var badViewModel = _textEditorService.ViewModelApi.GetOrDefault(ViewModelKey);

        if (badViewModel is not null)
        {
            TentativeHasSelection = TextEditorSelectionHelper.HasSelectedText(badViewModel.PrimaryCursor.Selection);

            TentativeKeyboardEventArgsKind = TextEditorViewModelDisplay.GetKeyboardEventArgsKind(
                KeyboardEventArgs,
                TentativeHasSelection,
                _textEditorService,
                out var localCommand);

            Command = localCommand;
        }
    }

    public TimeSpan ThrottleTimeSpan => TimeSpan.Zero;
    public KeyboardEventArgs KeyboardEventArgs { get; }
    public CommandNoType? Command { get; }

    /// <summary>
    /// The initial enqueueing of this throttle event might result in an incorrect <see cref="TentativeKeyboardEventArgsKind"/>
    /// This is due to the selection being checked, prior to the previous UI events that came before this have been handled.<br/><br/>
    ///
    /// A 'tab' key for example, without a selection: it inserts a tab character, but with a selection: it indents the lines
    /// that are selected. So, from not having a selection, to having one, a 'tab' key could be text or a command.<br/><br/>
    /// </summary>
    public KeyboardEventArgsKind TentativeKeyboardEventArgsKind { get; }

    /// <summary>
    /// The initial enqueueing of this throttle event might result in an incorrect <see cref="TentativeHasSelection"/>
    /// This is due to the selection being checked, prior to the previous UI events that came before this have been handled.<br/><br/>
    ///
    /// It is possible a user might at the moment of pressing a key, not have a selection.
    /// But have a previous event pending, that will result in a future selection when this event is handled.<br/><br/>
    /// </summary>
    public bool TentativeHasSelection { get; }

    public TextEditorViewModelDisplayOptions ViewModelDisplayOptions { get; }
    public ResourceUri ResourceUri { get; }
    public Key<TextEditorViewModel> ViewModelKey { get; }

    public IThrottleEvent? BatchOrDefault(IThrottleEvent moreRecentEvent)
    {
        if (moreRecentEvent is ThrottleEventOnKeyDown moreRecentEventOnKeyDown)
        {
            switch (TentativeKeyboardEventArgsKind)
            {
                case KeyboardEventArgsKind.Text:
                case KeyboardEventArgsKind.Movement:
                    if (moreRecentEventOnKeyDown.TentativeKeyboardEventArgsKind == TentativeKeyboardEventArgsKind)
                    {
                        return new ThrottleEventOnKeyDownBatch(
                            _throttleControllerUiEvents,
                            _uiEventsDelay,
                            new List<KeyboardEventArgs>()
                            {
                                moreRecentEventOnKeyDown.KeyboardEventArgs,
                                KeyboardEventArgs
                            },
                            TentativeKeyboardEventArgsKind,
                            ViewModelDisplayOptions,
                            _cursorDisplay,
                            ResourceUri,
                            ViewModelKey,
                            _handleAfterOnKeyDownRangeAsyncFactoryFunc,
                            _setTooltipViewModel,
                            _textEditorService);
                    }
                    break;
                case KeyboardEventArgsKind.ContextMenu:
                    break;
                case KeyboardEventArgsKind.Command:
                    break;
            }
        }
        
        if (moreRecentEvent is ThrottleEventOnKeyDownBatch moreRecentEventOnKeyDownBatch)
        {
            switch (TentativeKeyboardEventArgsKind)
            {
                case KeyboardEventArgsKind.Movement:
                case KeyboardEventArgsKind.Text:
                    if (moreRecentEventOnKeyDownBatch.KeyboardEventArgsKind == TentativeKeyboardEventArgsKind)
                    {
                        moreRecentEventOnKeyDownBatch.KeyboardEventArgsList.Add(KeyboardEventArgs);
                        return moreRecentEventOnKeyDownBatch;
                    }
                    break;
                case KeyboardEventArgsKind.ContextMenu:
                    break;
                case KeyboardEventArgsKind.Command:
                    break;
            }
        }

        return null;
    }

    public Task HandleEvent(CancellationToken cancellationToken)
    {
        _textEditorService.Post(
            nameof(ThrottleEventOnKeyDown),
            async editContext =>
            {
                var modelModifier = editContext.GetModelModifier(ResourceUri);
                var viewModelModifier = editContext.GetViewModelModifier(ViewModelKey);
                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
                var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                    return;

                // The initial enqueueing of this throttle event might result in an incorrect KeyboardEventArgsKind
                // This is due to the selection being checked, prior to the previous UI events that came before this have been handled.
                //
                // It is possible a user might at the moment of pressing a key, not have a selection.
                // But have a previous event pending, that will result in a future selection when this event is handled.
                var definiteHasSelection = TextEditorSelectionHelper.HasSelectedText(primaryCursorModifier);

                var definiteKeyboardEventArgsKind = TextEditorViewModelDisplay.GetKeyboardEventArgsKind(
                    KeyboardEventArgs, definiteHasSelection, _textEditorService, out var command);

                var shouldInvokeAfterOnKeyDownAsync = false;

                switch (definiteKeyboardEventArgsKind)
                {
                    case KeyboardEventArgsKind.Command:
                        shouldInvokeAfterOnKeyDownAsync = true;

                        await command.CommandFunc.Invoke(new TextEditorCommandArgs(
                                modelModifier.ResourceUri,
                                viewModelModifier.ViewModel.ViewModelKey,
                                definiteHasSelection,
                                _clipboardService,
                                _textEditorService,
                                _handleMouseStoppedMovingEventAsyncFunc,
                                _jsRuntime,
                                _dispatcher,
                                _serviceProvider,
                                _textEditorConfig))
                            .ConfigureAwait(false);
                        break;
                    case KeyboardEventArgsKind.Movement:
                        if ((KeyboardKeyFacts.MovementKeys.ARROW_DOWN == KeyboardEventArgs.Key || KeyboardKeyFacts.MovementKeys.ARROW_UP == KeyboardEventArgs.Key) &&
                             _cursorDisplay is not null && _cursorDisplay.MenuKind == TextEditorMenuKind.AutoCompleteMenu)
                        {
                            await _cursorDisplay.SetFocusToActiveMenuAsync().ConfigureAwait(false);
                        }
                        else
                        {
                            await _textEditorService.ViewModelApi.MoveCursorFactory(
                                    KeyboardEventArgs,
                                    modelModifier.ResourceUri,
                                    viewModelModifier.ViewModel.ViewModelKey)
                                .Invoke(editContext)
                                .ConfigureAwait(false);

                            await (_cursorDisplay?.SetShouldDisplayMenuAsync(TextEditorMenuKind.None) ?? Task.CompletedTask);
                        }
                        break;
                    case KeyboardEventArgsKind.ContextMenu:
                        await (_cursorDisplay?.SetShouldDisplayMenuAsync(TextEditorMenuKind.ContextMenu) ?? Task.CompletedTask);
                        break;
                    case KeyboardEventArgsKind.Text:
                    case KeyboardEventArgsKind.Other:
                        shouldInvokeAfterOnKeyDownAsync = true;

                        if (!TextEditorViewModelDisplay.IsAutocompleteMenuInvoker(KeyboardEventArgs))
                        {
                            if (KeyboardKeyFacts.MetaKeys.ESCAPE == KeyboardEventArgs.Key ||
                                KeyboardKeyFacts.MetaKeys.BACKSPACE == KeyboardEventArgs.Key ||
                                KeyboardKeyFacts.MetaKeys.DELETE == KeyboardEventArgs.Key ||
                                !KeyboardKeyFacts.IsMetaKey(KeyboardEventArgs))
                            {
                                await (_cursorDisplay?.SetShouldDisplayMenuAsync(TextEditorMenuKind.None) ?? Task.CompletedTask);
                            }
                        }

                        _setTooltipViewModel.Invoke(null);

                        await _textEditorService.ModelApi.HandleKeyboardEventFactory(
                                ResourceUri,
                                ViewModelKey,
                                KeyboardEventArgs,
                                CancellationToken.None)
                            .Invoke(editContext)
                            .ConfigureAwait(false);
                        break;
                }

                if (shouldInvokeAfterOnKeyDownAsync)
                {
                    if (command is null ||
                        command is TextEditorCommand commandTextEditor && commandTextEditor.ShouldScrollCursorIntoView)
                    {
                        primaryCursorModifier.ShouldRevealCursor = true;
                    }

                    var cursorDisplay = _cursorDisplay;

                    if (cursorDisplay is not null)
                    {
                        var afterOnKeyDownAsyncFactory = ViewModelDisplayOptions.AfterOnKeyDownAsyncFactory
                            ?? _handleAfterOnKeyDownAsyncFactoryFunc;

                        await afterOnKeyDownAsyncFactory.Invoke(
                                modelModifier.ResourceUri,
                                viewModelModifier.ViewModel.ViewModelKey,
                                KeyboardEventArgs,
                                cursorDisplay.SetShouldDisplayMenuAsync)
                            .Invoke(editContext)
                            .ConfigureAwait(false);
                    }
                }
            });

        return Task.CompletedTask;
    }
}
