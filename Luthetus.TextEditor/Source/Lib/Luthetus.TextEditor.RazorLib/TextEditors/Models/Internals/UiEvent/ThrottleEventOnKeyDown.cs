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
            ThinksHasSelection = TextEditorSelectionHelper.HasSelectedText(badViewModel.PrimaryCursor.Selection);

            KeyboardEventArgsKind = TextEditorViewModelDisplay.GetOnKeyDownKind(
                KeyboardEventArgs,
                ThinksHasSelection,
                _textEditorService,
                out var localCommand);

            Command = localCommand;
        }
    }

    public TimeSpan ThrottleTimeSpan => TimeSpan.Zero;
    public KeyboardEventArgs KeyboardEventArgs { get; }
    public KeyboardEventArgsKind KeyboardEventArgsKind { get; }
    public CommandNoType? Command { get; }
    
    public bool ThinksHasSelection { get; }

    public TextEditorViewModelDisplayOptions ViewModelDisplayOptions { get; }
    public ResourceUri ResourceUri { get; }
    public Key<TextEditorViewModel> ViewModelKey { get; }

    public IThrottleEvent? BatchOrDefault(IThrottleEvent moreRecentEvent)
    {
        if (moreRecentEvent is ThrottleEventOnKeyDown moreRecentEventOnKeyDown)
        {
            if (KeyboardEventArgsKind == KeyboardEventArgsKind.Movement)
            {
                // TODO: Batch 'movement'
            }
            else if (KeyboardEventArgsKind == KeyboardEventArgsKind.ContextMenu)
            {
                // TODO: Decide what 'context menu' means in the context of 'batching'
            }
            else if (KeyboardEventArgsKind == KeyboardEventArgsKind.Command)
            {
                // TODO: Decide what 'command' means in the context of 'batching'
            }
            else if (KeyboardEventArgsKind == KeyboardEventArgsKind.Text)
            {
                return new ThrottleEventOnKeyDownBatch(
                    _throttleControllerUiEvents,
                    _uiEventsDelay,
                    new List<KeyboardEventArgs>()
                    {
                        moreRecentEventOnKeyDown.KeyboardEventArgs,
                        KeyboardEventArgs
                    },
                    KeyboardEventArgsKind,
                    ViewModelDisplayOptions,
                    _cursorDisplay,
                    ResourceUri,
                    ViewModelKey,
                    _handleAfterOnKeyDownRangeAsyncFactoryFunc,
                    _setTooltipViewModel,
                    _textEditorService);
            }
        }
        
        if (moreRecentEvent is ThrottleEventOnKeyDownBatch moreRecentEventOnKeyDownBatch)
        {
            moreRecentEventOnKeyDownBatch.KeyboardEventArgsList.Add(KeyboardEventArgs);
            return moreRecentEventOnKeyDownBatch;
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

                if (modelModifier is null || viewModelModifier is null)
                    return;

                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier.ViewModel);

                if (cursorModifierBag is null)
                    return;

                var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (primaryCursorModifier is null)
                    return;

                var hasSelection = TextEditorSelectionHelper.HasSelectedText(primaryCursorModifier);
                var onKeyDownKind = TextEditorViewModelDisplay.GetOnKeyDownKind(KeyboardEventArgs, hasSelection, _textEditorService, out var command);

                if (onKeyDownKind == KeyboardEventArgsKind.Movement)
                {
                    if ((KeyboardKeyFacts.MovementKeys.ARROW_DOWN == KeyboardEventArgs.Key ||
                            KeyboardKeyFacts.MovementKeys.ARROW_UP == KeyboardEventArgs.Key) &&
                        _cursorDisplay is not null &&
                        _cursorDisplay.MenuKind == TextEditorMenuKind.AutoCompleteMenu)
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
                }
                else if (onKeyDownKind == KeyboardEventArgsKind.ContextMenu)
                {
                    await (_cursorDisplay?.SetShouldDisplayMenuAsync(TextEditorMenuKind.ContextMenu) ?? Task.CompletedTask);
                }
                else if (onKeyDownKind == KeyboardEventArgsKind.Command || onKeyDownKind == KeyboardEventArgsKind.Text || onKeyDownKind == KeyboardEventArgsKind.Other)
                {
                    if (onKeyDownKind == KeyboardEventArgsKind.Command)
                    {
                        await command.CommandFunc.Invoke(new TextEditorCommandArgs(
                                modelModifier.ResourceUri,
                                viewModelModifier.ViewModel.ViewModelKey,
                                hasSelection,
                                _clipboardService,
                                _textEditorService,
                                _handleMouseStoppedMovingEventAsyncFunc,
                                _jsRuntime,
                                _dispatcher,
                                _serviceProvider,
                                _textEditorConfig))
                            .ConfigureAwait(false);
                    }
                    else if (onKeyDownKind == KeyboardEventArgsKind.Text || onKeyDownKind == KeyboardEventArgsKind.Other)
                    {
                        if (!TextEditorViewModelDisplay.IsAutocompleteMenuInvoker(KeyboardEventArgs))
                        {
                            if (!KeyboardKeyFacts.IsMetaKey(KeyboardEventArgs)
                                || KeyboardKeyFacts.MetaKeys.ESCAPE == KeyboardEventArgs.Key ||
                                    KeyboardKeyFacts.MetaKeys.BACKSPACE == KeyboardEventArgs.Key ||
                                    KeyboardKeyFacts.MetaKeys.DELETE == KeyboardEventArgs.Key)
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
                    }

                    if (KeyboardEventArgs.Key != "Shift" &&
                        KeyboardEventArgs.Key != "Control" &&
                        KeyboardEventArgs.Key != "Alt")
                    {
                        if (command is null ||
                            command is TextEditorCommand commandTextEditor &&
                            commandTextEditor.ShouldScrollCursorIntoView)
                        {
                            primaryCursorModifier.ShouldRevealCursor = true;
                        }
                    }

                    var cursorDisplay = _cursorDisplay;

                    if (cursorDisplay is not null)
                    {
                        var afterOnKeyDownAsyncFactory = ViewModelDisplayOptions.AfterOnKeyDownAsyncFactory ?? _handleAfterOnKeyDownAsyncFactoryFunc;

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
