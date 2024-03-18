using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals.UiEvent;

public class ThrottleEventOnKeyDownBatch : ITextEditorTask
{
    private readonly TextEditorViewModelDisplay.TextEditorEvents _events;

    public ThrottleEventOnKeyDownBatch(
        TextEditorViewModelDisplay.TextEditorEvents events,
        List<ThrottleEventOnKeyDown> throttleEventOnKeyDownList,
        KeyboardEventArgsKind keyboardEventArgsKind,
        ResourceUri resourceUri,
        Key<TextEditorViewModel> viewModelKey)
    {
        _events = events;

        ThrottleEventOnKeyDownList = throttleEventOnKeyDownList;
        KeyboardEventArgsKind = keyboardEventArgsKind;
        ResourceUri = resourceUri;
        ViewModelKey = viewModelKey;
    }

    public Key<BackgroundTask> BackgroundTaskKey { get; } = Key<BackgroundTask>.NewKey();
    public Key<BackgroundTaskQueue> QueueKey { get; } = ContinuousBackgroundTaskWorker.GetQueueKey();
    public string Name => nameof(ThrottleEventOnKeyDownBatch) + $"_{ThrottleEventOnKeyDownList.Count}";
    public Task? WorkProgress { get; }
    public TimeSpan ThrottleTimeSpan => _events.ThrottleDelayDefault;
    public List<ThrottleEventOnKeyDown> ThrottleEventOnKeyDownList { get; }
    public KeyboardEventArgsKind KeyboardEventArgsKind { get; }
    public ResourceUri ResourceUri { get; }
    public Key<TextEditorViewModel> ViewModelKey { get; }

    
	public async Task InvokeWithEditContext(ITextEditorEditContext editContext)
	{
		var eventName = string.Empty;
        {
            var firstEntry = ThrottleEventOnKeyDownList.First();
            if (firstEntry.TentativeKeyboardEventArgsKind == KeyboardEventArgsKind.Command &&
                firstEntry.Command is not null)
            {
                eventName = firstEntry.Command.InternalIdentifier;
            }
            else
            {
                eventName = firstEntry.KeyboardEventArgs.Key;

                if (string.IsNullOrWhiteSpace(eventName))
                    eventName = firstEntry.KeyboardEventArgs.Code;
            }
        }

		var modelModifier = editContext.GetModelModifier(ResourceUri);
        var viewModelModifier = editContext.GetViewModelModifier(ViewModelKey);
        var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
        var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

        if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
            return;

        var hasSelection = TextEditorSelectionHelper.HasSelectedText(primaryCursorModifier);

        var shouldInvokeAfterOnKeyDownAsync = false;
        
        if (KeyboardEventArgsKind == KeyboardEventArgsKind.Command)
        {
            shouldInvokeAfterOnKeyDownAsync = true;
            var inspectThrottleEventOnKeyDown = ThrottleEventOnKeyDownList.First();

            if (inspectThrottleEventOnKeyDown.Command is not null)
            {
                await inspectThrottleEventOnKeyDown.Command.CommandFunc.Invoke(new TextEditorCommandArgs(
                        modelModifier.ResourceUri,
                        viewModelModifier.ViewModel.ViewModelKey,
                        hasSelection,
                        _events.ClipboardService,
                        _events.TextEditorService,
                        _events.HandleMouseStoppedMovingEventAsync,
                        _events.JsRuntime,
                        _events.Dispatcher,
                        _events.ServiceProvider,
                        _events.TextEditorConfig))
                    .ConfigureAwait(false);
            }
        }
        else if (KeyboardEventArgsKind == KeyboardEventArgsKind.Movement)
        {
            if (_events.CursorDisplay is null || _events.CursorDisplay.MenuKind != TextEditorMenuKind.AutoCompleteMenu)
            {
                // Don't do this foreach loop if the autocomplete menu is showing.
                foreach (var throttleEventOnKeyDown in ThrottleEventOnKeyDownList)
                {
                    await _events.TextEditorService.ViewModelApi.MoveCursorFactory(
                            throttleEventOnKeyDown.KeyboardEventArgs,
                            modelModifier.ResourceUri,
                            viewModelModifier.ViewModel.ViewModelKey)
                        .Invoke(editContext)
                        .ConfigureAwait(false);
                }
            }

            await (_events.CursorDisplay?.SetShouldDisplayMenuAsync(TextEditorMenuKind.None) ?? Task.CompletedTask).ConfigureAwait(false);
        }
        else if (KeyboardEventArgsKind == KeyboardEventArgsKind.ContextMenu)
        {
            // TODO: Batch context menu
        }
        else if (KeyboardEventArgsKind == KeyboardEventArgsKind.Text)
        {
            shouldInvokeAfterOnKeyDownAsync = true;
            _events.TooltipViewModel = null;

            modelModifier.EditByInsertion(
                string.Join(string.Empty, ThrottleEventOnKeyDownList.Select(x => x.KeyboardEventArgs.Key)),
                cursorModifierBag,
                CancellationToken.None);
        }
        else if (KeyboardEventArgsKind == KeyboardEventArgsKind.Other)
        {
            shouldInvokeAfterOnKeyDownAsync = true;
            _events.TooltipViewModel = null;

            var inspectThrottleEventOnKeyDown = ThrottleEventOnKeyDownList.First();
            if (KeyboardKeyFacts.IsMetaKey(inspectThrottleEventOnKeyDown.KeyboardEventArgs))
            {
                if (KeyboardKeyFacts.MetaKeys.BACKSPACE == inspectThrottleEventOnKeyDown.KeyboardEventArgs.Key)
                {
                    modelModifier.PerformDeletions(
                        inspectThrottleEventOnKeyDown.KeyboardEventArgs,
                        cursorModifierBag,
                        ThrottleEventOnKeyDownList.Count,
                        CancellationToken.None);
                }

                if (KeyboardKeyFacts.MetaKeys.DELETE == inspectThrottleEventOnKeyDown.KeyboardEventArgs.Key)
                {
                    modelModifier.PerformDeletions(
                        inspectThrottleEventOnKeyDown.KeyboardEventArgs,
                        cursorModifierBag,
                        ThrottleEventOnKeyDownList.Count,
                        CancellationToken.None);
                }
            }
            else if (KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE == inspectThrottleEventOnKeyDown.KeyboardEventArgs.Code ||
                     KeyboardKeyFacts.WhitespaceCodes.TAB_CODE == inspectThrottleEventOnKeyDown.KeyboardEventArgs.Code)
            {
                foreach (var throttleEventOnKeyDown in ThrottleEventOnKeyDownList)
                {
                    modelModifier.HandleKeyboardEvent(
                        throttleEventOnKeyDown.KeyboardEventArgs,
                        cursorModifierBag,
                        CancellationToken.None);
                }
            }
        }

        if (shouldInvokeAfterOnKeyDownAsync)
        {
            var inspectThrottleEventOnKeyDown = ThrottleEventOnKeyDownList.First();

            if (inspectThrottleEventOnKeyDown.Command is null ||
                inspectThrottleEventOnKeyDown.Command is TextEditorCommand commandTextEditor && commandTextEditor.ShouldScrollCursorIntoView)
            {
                primaryCursorModifier.ShouldRevealCursor = true;
            }

            var cursorDisplay = _events.CursorDisplay;

            if (cursorDisplay is not null)
            {
                await _events.HandleAfterOnKeyDownRangeAsyncFactory(
                        modelModifier.ResourceUri,
                        viewModelModifier.ViewModel.ViewModelKey,
                        ThrottleEventOnKeyDownList.Select(x => x.KeyboardEventArgs).ToList(),
                        cursorDisplay.SetShouldDisplayMenuAsync)
                    .Invoke(editContext)
                    .ConfigureAwait(false);
            }
        }
	}

    public IBackgroundTask? BatchOrDefault(IBackgroundTask oldEvent)
    {
        // TODO: Should this type implement BatchOrDefault, beyond just returning null?
        return null;
    }

    public Task HandleEvent(CancellationToken cancellationToken)
    {
		throw new NotImplementedException($"{nameof(ITextEditorTask)} should not implement {nameof(HandleEvent)}" +
			"because they instead are contained within an 'IBackgroundTask' that came from the 'TextEditorService'");
    }
}
