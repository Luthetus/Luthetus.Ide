using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals.UiEvent;

public class ThrottleEventOnKeyDown : IThrottleEvent
{
    private readonly ITextEditorService _textEditorService;

    public ThrottleEventOnKeyDown(
        ThrottleController throttleControllerUiEvents,
        TimeSpan uiEventsDelay,
        KeyboardEventArgs keyboardEventArgs,
        ResourceUri resourceUri,
        Key<TextEditorViewModel> viewModelKey,
        ITextEditorService textEditorService)
    {
        KeyboardEventArgs = keyboardEventArgs;
        ResourceUri = resourceUri;
        ViewModelKey = viewModelKey;
        _textEditorService = textEditorService;
    }

    public TimeSpan ThrottleTimeSpan => TimeSpan.Zero;
    public KeyboardEventArgs KeyboardEventArgs { get; }
    public ResourceUri ResourceUri { get; }
    public Key<TextEditorViewModel> ViewModelKey { get; }

    public IThrottleEvent? BatchOrDefault(IThrottleEvent moreRecentEvent)
    {
        if (moreRecentEvent is ThrottleEventOnKeyDown moreRecentEventOnKeyDown)
        {
            return new ThrottleEventOnKeyDownBatch(
                new List<KeyboardEventArgs>() 
                { 
                    moreRecentEventOnKeyDown.KeyboardEventArgs,
                    KeyboardEventArgs
                },
                ResourceUri,
                ViewModelKey);
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
        throw new NotImplementedException();
    }

    private IThrottleEvent? BatchOnKeyDown(IThrottleEvent oldEvent, IThrottleEvent recentEvent, ResourceUri resourceUri, Key<TextEditorViewModel> viewModelKey)
    {
        if (oldEvent is ThrottleEvent<(Key<TextEditorViewModel> viewModelKey, List<KeyboardEventArgs> keyboardEventsList)> oldEventWithType &&
            recentEvent is ThrottleEvent<(Key<TextEditorViewModel> viewModelKey, List<KeyboardEventArgs> keyboardEventsList)> recentEventWithType)
        {
            // Avoid taking keypresses such as 'Backspace' and treating them as text.
            if (recentEventWithType.Item.keyboardEventsList.Last().Key.Length != 1)
                return null;

            // Avoid external state mutations with local variables.
            var oldEventWithTypeViewModelKey = oldEventWithType.Item.viewModelKey;
            var recentEventWithTypeViewModelKey = recentEventWithType.Item.viewModelKey;

            if (viewModelKey != oldEventWithType.Item.viewModelKey ||
                oldEventWithType.Item.viewModelKey != recentEventWithType.Item.viewModelKey)
            {
                return null;
            }

            var badViewModel = _textEditorService.ViewModelApi.GetOrDefault(viewModelKey);

            if (badViewModel is null)
                return null;

            var hasSelection = TextEditorSelectionHelper.HasSelectedText(badViewModel.PrimaryCursor.Selection);

            var oldKeyboardEventArgs = oldEventWithType.Item.keyboardEventsList.First();
            var recentKeyboardEventArgs = oldEventWithType.Item.keyboardEventsList.First();

            var oldEventOnKeyDownKind = GetOnKeyDownKind(
                oldKeyboardEventArgs,
                hasSelection,
                out var oldEventCommand);

            var recentEventOnKeyDownKind = GetOnKeyDownKind(
                recentKeyboardEventArgs,
                hasSelection,
                out var recentEventCommand);

            if (oldEventOnKeyDownKind == OnKeyDownKind.Movement)
            {
                // TODO: Batch 'movement'
            }
            else if (oldEventOnKeyDownKind == OnKeyDownKind.ContextMenu)
            {
                // TODO: Decide what 'context menu' means in the context of 'batching'
            }
            else if (oldEventOnKeyDownKind == OnKeyDownKind.Command)
            {
                // TODO: Decide what 'command' means in the context of 'batching'
            }
            else if (oldEventOnKeyDownKind == OnKeyDownKind.None)
            {
                var combinedKeyboardEventArgs = new List<KeyboardEventArgs>();
                combinedKeyboardEventArgs.AddRange(oldEventWithType.Item.keyboardEventsList);
                combinedKeyboardEventArgs.AddRange(recentEventWithType.Item.keyboardEventsList);

                var combinedEvent = new ThrottleEvent<(Key<TextEditorViewModel> viewModelKey, List<KeyboardEventArgs> keyboardEventsList)>(
                    oldEventWithType.Id,
                    oldEventWithType.ThrottleTimeSpan,
                    (viewModelKey, combinedKeyboardEventArgs),
                    (combinedThrottleEvent, throttleCancellationToken) => HandleOnKeyDown(combinedThrottleEvent, resourceUri, viewModelKey, true, throttleCancellationToken),
                    tuple => BatchOnKeyDown(tuple.OldEvent, tuple.RecentEvent, resourceUri, viewModelKey));

                return (IThrottleEvent?)combinedEvent;
            }
        }

        return null;
    }

    private Task HandleOnKeyDown(
        IThrottleEvent throttleEvent,
        ResourceUri resourceUri,
        Key<TextEditorViewModel> viewModelKey,
        bool isBatched,
        CancellationToken cancellationToken)
    {
        _textEditorService.Post(
            nameof(HandleOnKeyDown),
            async editContext =>
            {
                var modelModifier = editContext.GetModelModifier(resourceUri);
                var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);

                if (modelModifier is null || viewModelModifier is null)
                    return;

                var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier.ViewModel);

                if (cursorModifierBag is null)
                    return;

                var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

                if (primaryCursorModifier is null)
                    return;

                var hasSelection = TextEditorSelectionHelper.HasSelectedText(primaryCursorModifier);
                var throttleEventWithType = (ThrottleEvent<(Key<TextEditorViewModel> viewModelKey, List<KeyboardEventArgs> keyboardEventsList)>)throttleEvent;
                var keyboardEventArgs = throttleEventWithType.Item.keyboardEventsList.First();
                var onKeyDownKind = GetOnKeyDownKind(keyboardEventArgs, hasSelection, out var command);

                if (onKeyDownKind == OnKeyDownKind.Movement)
                {
                    if ((KeyboardKeyFacts.MovementKeys.ARROW_DOWN == keyboardEventArgs.Key ||
                            KeyboardKeyFacts.MovementKeys.ARROW_UP == keyboardEventArgs.Key) &&
                        CursorDisplay is not null &&
                        CursorDisplay.MenuKind == TextEditorMenuKind.AutoCompleteMenu)
                    {
                        await CursorDisplay.SetFocusToActiveMenuAsync().ConfigureAwait(false);
                    }
                    else
                    {
                        await _textEditorService.ViewModelApi.MoveCursorFactory(
                                keyboardEventArgs,
                                modelModifier.ResourceUri,
                                viewModelModifier.ViewModel.ViewModelKey)
                            .Invoke(editContext)
                            .ConfigureAwait(false);

                        CursorDisplay?.SetShouldDisplayMenuAsync(TextEditorMenuKind.None);
                    }
                }
                else if (onKeyDownKind == OnKeyDownKind.ContextMenu)
                {
                    CursorDisplay?.SetShouldDisplayMenuAsync(TextEditorMenuKind.ContextMenu);
                }
                else if (onKeyDownKind == OnKeyDownKind.Command || onKeyDownKind == OnKeyDownKind.None)
                {
                    if (onKeyDownKind == OnKeyDownKind.Command)
                    {
                        await command.CommandFunc.Invoke(new TextEditorCommandArgs(
                                modelModifier.ResourceUri,
                                viewModelModifier.ViewModel.ViewModelKey,
                                hasSelection,
                                ClipboardService,
                                TextEditorService,
                                HandleMouseStoppedMovingEventAsync,
                                JsRuntime,
                                Dispatcher,
                                ServiceProvider,
                                TextEditorConfig))
                            .ConfigureAwait(false);
                    }
                    else if (onKeyDownKind == OnKeyDownKind.None)
                    {
                        if (!IsAutocompleteMenuInvoker(keyboardEventArgs))
                        {
                            if (!KeyboardKeyFacts.IsMetaKey(keyboardEventArgs)
                                || KeyboardKeyFacts.MetaKeys.ESCAPE == keyboardEventArgs.Key ||
                                    KeyboardKeyFacts.MetaKeys.BACKSPACE == keyboardEventArgs.Key ||
                                    KeyboardKeyFacts.MetaKeys.DELETE == keyboardEventArgs.Key)
                            {
                                CursorDisplay?.SetShouldDisplayMenuAsync(TextEditorMenuKind.None);
                            }
                        }

                        _tooltipViewModel = null;

                        if (throttleEvent is ThrottleEvent<(Key<TextEditorViewModel> viewModelKey, List<KeyboardEventArgs> keyboardEventsList)> throttleEventKeyboardEventArgsList)
                        {
                            if (throttleEventKeyboardEventArgsList.Item.keyboardEventsList.Count > 1)
                            {
                                modelModifier.EditByInsertion(
                                    string.Join(string.Empty, throttleEventKeyboardEventArgsList.Item.keyboardEventsList.Select(x => x.Key)),
                                    cursorModifierBag,
                                    CancellationToken.None);
                            }
                            else
                            {
                                await _textEditorService.ModelApi.HandleKeyboardEventFactory(
                                        resourceUri,
                                        viewModelKey,
                                        keyboardEventArgs,
                                        CancellationToken.None)
                                    .Invoke(editContext)
                                    .ConfigureAwait(false);
                            }
                        }
                    }

                    if (keyboardEventArgs.Key != "Shift" &&
                        keyboardEventArgs.Key != "Control" &&
                        keyboardEventArgs.Key != "Alt")
                    {
                        if (command is null ||
                            command is TextEditorCommand commandTextEditor &&
                            commandTextEditor.ShouldScrollCursorIntoView)
                        {
                            primaryCursorModifier.ShouldRevealCursor = true;
                        }
                    }

                    var cursorDisplay = CursorDisplay;

                    if (cursorDisplay is not null)
                    {
                        if (throttleEvent is ThrottleEvent<(Key<TextEditorViewModel> viewModelKey, List<KeyboardEventArgs> keyboardEventsList)> throttleEventKeyboardEventArgsList)
                        {
                            if (throttleEventKeyboardEventArgsList.Item.keyboardEventsList.Count > 1)
                            {
                                var afterOnKeyDownRangeAsyncFactory = ViewModelDisplayOptions.AfterOnKeyDownRangeAsyncFactory ?? HandleAfterOnKeyDownRangeAsyncFactory;

                                await afterOnKeyDownRangeAsyncFactory.Invoke(
                                        modelModifier.ResourceUri,
                                        viewModelModifier.ViewModel.ViewModelKey,
                                        throttleEventKeyboardEventArgsList.Item.keyboardEventsList,
                                        cursorDisplay.SetShouldDisplayMenuAsync)
                                    .Invoke(editContext)
                                    .ConfigureAwait(false);
                            }
                            else
                            {
                                var afterOnKeyDownAsyncFactory = ViewModelDisplayOptions.AfterOnKeyDownAsyncFactory ?? HandleAfterOnKeyDownAsyncFactory;

                                await afterOnKeyDownAsyncFactory.Invoke(
                                        modelModifier.ResourceUri,
                                        viewModelModifier.ViewModel.ViewModelKey,
                                        keyboardEventArgs,
                                        cursorDisplay.SetShouldDisplayMenuAsync)
                                    .Invoke(editContext)
                                    .ConfigureAwait(false);
                            }
                        }
                    }
                }
            });

        return Task.CompletedTask;
    }
}
