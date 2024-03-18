using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals.UiEvent;

public class ThrottleEventOnDoubleClick : ITextEditorTask
{
    private readonly TextEditorViewModelDisplay.TextEditorEvents _events;

    public ThrottleEventOnDoubleClick(
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
    public string Name { get; } = nameof(ThrottleEventOnDoubleClick);
    public Task? WorkProgress { get; }
    public MouseEventArgs MouseEventArgs { get; }
    public ResourceUri ResourceUri { get; }
    public Key<TextEditorViewModel> ViewModelKey { get; }

    public TimeSpan ThrottleTimeSpan => _events.ThrottleDelayDefault;

	public async Task InvokeWithEditContext(ITextEditorEditContext editContext)
	{
		var modelModifier = editContext.GetModelModifier(ResourceUri, true);
        var viewModelModifier = editContext.GetViewModelModifier(ViewModelKey);
        var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
        var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

        if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
            return;

        var hasSelectedText = TextEditorSelectionHelper.HasSelectedText(primaryCursorModifier);

        if ((MouseEventArgs.Buttons & 1) != 1 && hasSelectedText)
            return; // Not pressing the left mouse button so assume ContextMenu is desired result.

        if (MouseEventArgs.ShiftKey)
            return; // Do not expand selection if user is holding shift

        var rowAndColumnIndex = await _events.CalculateRowAndColumnIndex(MouseEventArgs).ConfigureAwait(false);

        var lowerColumnIndexExpansion = modelModifier.GetColumnIndexOfCharacterWithDifferingKind(
            rowAndColumnIndex.rowIndex,
            rowAndColumnIndex.columnIndex,
            true);

        lowerColumnIndexExpansion = lowerColumnIndexExpansion == -1
            ? 0
            : lowerColumnIndexExpansion;

        var higherColumnIndexExpansion = modelModifier.GetColumnIndexOfCharacterWithDifferingKind(
            rowAndColumnIndex.rowIndex,
            rowAndColumnIndex.columnIndex,
            false);

        higherColumnIndexExpansion = higherColumnIndexExpansion == -1
            ? modelModifier.GetLengthOfRow(rowAndColumnIndex.rowIndex)
            : higherColumnIndexExpansion;

        // Move user's cursor position to the higher expansion
        {
            primaryCursorModifier.RowIndex = rowAndColumnIndex.rowIndex;
            primaryCursorModifier.ColumnIndex = higherColumnIndexExpansion;
            primaryCursorModifier.PreferredColumnIndex = rowAndColumnIndex.columnIndex;
        }

        // Set text selection ending to higher expansion
        {
            var cursorPositionOfHigherExpansion = modelModifier.GetPositionIndex(
                rowAndColumnIndex.rowIndex,
                higherColumnIndexExpansion);

            primaryCursorModifier.SelectionEndingPositionIndex = cursorPositionOfHigherExpansion;
        }

        // Set text selection anchor to lower expansion
        {
            var cursorPositionOfLowerExpansion = modelModifier.GetPositionIndex(
                rowAndColumnIndex.rowIndex,
                lowerColumnIndexExpansion);

            primaryCursorModifier.SelectionAnchorPositionIndex = cursorPositionOfLowerExpansion;
        }
	}

    public IBackgroundTask? BatchOrDefault(IBackgroundTask oldEvent)
    {
        return this;
    }

    public Task HandleEvent(CancellationToken cancellationToken)
    {
		throw new NotImplementedException($"{nameof(ITextEditorTask)} should not implement {nameof(HandleEvent)}" +
			"because they instead are contained within an 'IBackgroundTask' that came from the 'TextEditorService'");
    }
}
