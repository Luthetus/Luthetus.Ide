using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;

namespace Luthetus.TextEditor.RazorLib.Events.Models;

public class OnMouseDown : ITextEditorTask
{
    private readonly TextEditorViewModelDisplay.TextEditorEvents _events;

    public OnMouseDown(
        MouseEventArgs mouseEventArgs,
        TextEditorViewModelDisplay.TextEditorEvents events,
        ResourceUri resourceUri,
        Key<TextEditorViewModel> viewModelKey)
    {
        _events = events;

        MouseEventArgs = mouseEventArgs;
        ResourceUri = resourceUri;
        ViewModelKey = viewModelKey;
    }

    public Key<BackgroundTask> BackgroundTaskKey { get; } = Key<BackgroundTask>.NewKey();
    public Key<BackgroundTaskQueue> QueueKey { get; } = ContinuousBackgroundTaskWorker.GetQueueKey();
    public string Name { get; } = nameof(OnMouseDown);
    public Task? WorkProgress { get; }
    public MouseEventArgs MouseEventArgs { get; }
    public ResourceUri ResourceUri { get; }
    public Key<TextEditorViewModel> ViewModelKey { get; }

    public TimeSpan ThrottleTimeSpan => TextEditorViewModelDisplay.TextEditorEvents.ThrottleDelayDefault;

    public async Task InvokeWithEditContext(IEditContext editContext)
    {
        var modelModifier = editContext.GetModelModifier(ResourceUri, true);
        var viewModelModifier = editContext.GetViewModelModifier(ViewModelKey);
        var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
        var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

        if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
            return;

        viewModelModifier.ViewModel.UnsafeState.ShouldRevealCursor = false;

        var hasSelectedText = TextEditorSelectionHelper.HasSelectedText(primaryCursorModifier);

        if ((MouseEventArgs.Buttons & 1) != 1 && hasSelectedText)
            return; // Not pressing the left mouse button so assume ContextMenu is desired result.

        await _events.CursorSetShouldDisplayMenuAsyncFunc.Invoke(MenuKind.None, false).ConfigureAwait(false);

        // Remember the current cursor position prior to doing anything
        var inRowIndex = primaryCursorModifier.LineIndex;
        var inColumnIndex = primaryCursorModifier.ColumnIndex;

        // Move the cursor position
        var rowAndColumnIndex = await _events.CalculateRowAndColumnIndex(MouseEventArgs).ConfigureAwait(false);
        primaryCursorModifier.LineIndex = rowAndColumnIndex.rowIndex;
        primaryCursorModifier.ColumnIndex = rowAndColumnIndex.columnIndex;
        primaryCursorModifier.PreferredColumnIndex = rowAndColumnIndex.columnIndex;

        _events.CursorPauseBlinkAnimationAction.Invoke();

        var cursorPositionIndex = modelModifier.GetPositionIndex(new TextEditorCursor(
            rowAndColumnIndex.rowIndex,
            rowAndColumnIndex.columnIndex,
            true));

        if (MouseEventArgs.ShiftKey)
        {
            if (!hasSelectedText)
            {
                // If user does not yet have a selection then place the text selection anchor were they were
                primaryCursorModifier.SelectionAnchorPositionIndex = modelModifier
                    .GetPositionIndex(inRowIndex, inColumnIndex);
            }

            // If user ALREADY has a selection then do not modify the text selection anchor
        }
        else
        {
            primaryCursorModifier.SelectionAnchorPositionIndex = cursorPositionIndex;
        }

        primaryCursorModifier.SelectionEndingPositionIndex = cursorPositionIndex;
    }

    public IBackgroundTask? BatchOrDefault(IBackgroundTask oldEvent)
    {
        if (oldEvent is OnMouseDown)
		{
			// Replace the upstream event with this one,
			// because unhandled-consecutive events of this type are redundant.
			return this;
		}
        
		// Keep both events, because they are not able to be batched.
		return null;
    }

    public Task HandleEvent(CancellationToken cancellationToken)
    {
        throw new NotImplementedException($"{nameof(ITextEditorTask)} should not implement {nameof(HandleEvent)}" +
            "because they instead are contained within an 'IBackgroundTask' that came from the 'TextEditorService'");
    }
}
