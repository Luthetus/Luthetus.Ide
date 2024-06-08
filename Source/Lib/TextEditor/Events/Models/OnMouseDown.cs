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

	public IEditContext EditContext { get; set; }

    public TimeSpan ThrottleTimeSpan => TextEditorViewModelDisplay.TextEditorEvents.ThrottleDelayDefault;

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

    public async Task HandleEvent(CancellationToken cancellationToken)
    {
		try
		{
            var modelModifier = EditContext.GetModelModifier(ResourceUri, true);
            var viewModelModifier = EditContext.GetViewModelModifier(ViewModelKey);
            var cursorModifierBag = EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
            var primaryCursorModifier = EditContext.GetPrimaryCursorModifier(cursorModifierBag);

            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
                return;

            viewModelModifier.ViewModel.UnsafeState.ShouldRevealCursor = false;

            var hasSelectedText = TextEditorSelectionHelper.HasSelectedText(primaryCursorModifier);

            if ((MouseEventArgs.Buttons & 1) != 1 && hasSelectedText)
                return; // Not pressing the left mouse button so assume ContextMenu is desired result.

			viewModelModifier.ViewModel = viewModelModifier.ViewModel with
			{
				MenuKind = MenuKind.None
			};

            // Remember the current cursor position prior to doing anything
            var inRowIndex = primaryCursorModifier.LineIndex;
            var inColumnIndex = primaryCursorModifier.ColumnIndex;

            // Move the cursor position
			//
			// Labeling any IEditContext -> JavaScript interop or Blazor StateHasChanged.
			// Reason being, these are likely to be huge optimizations (2024-05-29).
            var rowAndColumnIndex = await EventUtils.CalculateRowAndColumnIndex(
					ResourceUri,
					ViewModelKey,
					MouseEventArgs,
					EditContext)
				.ConfigureAwait(false);

            primaryCursorModifier.LineIndex = rowAndColumnIndex.rowIndex;
            primaryCursorModifier.ColumnIndex = rowAndColumnIndex.columnIndex;
            primaryCursorModifier.PreferredColumnIndex = rowAndColumnIndex.columnIndex;

			EditContext.TextEditorService.ViewModelApi.SetCursorShouldBlink(false);

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
		finally
		{
			await EditContext.TextEditorService.FinalizePost(EditContext);
		}
    }
}
