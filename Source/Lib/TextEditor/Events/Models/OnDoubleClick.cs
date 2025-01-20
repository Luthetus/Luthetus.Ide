using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

namespace Luthetus.TextEditor.RazorLib.Events.Models;

public struct OnDoubleClick : ITextEditorWork
{
    public OnDoubleClick(
        MouseEventArgs mouseEventArgs,
		TextEditorComponentData componentData,
        ResourceUri resourceUri,
        Key<TextEditorViewModel> viewModelKey)
    {
		ComponentData = componentData;

        MouseEventArgs = mouseEventArgs;
        ResourceUri = resourceUri;
        ViewModelKey = viewModelKey;
    }

    public Key<IBackgroundTask> BackgroundTaskKey => Key<IBackgroundTask>.Empty;
    public Key<IBackgroundTaskQueue> QueueKey { get; } = BackgroundTaskFacts.ContinuousQueueKey;
    public bool EarlyBatchEnabled { get; set; } = true;
    public bool __TaskCompletionSourceWasCreated { get; set; }
    // TODO: I'm uncomfortable as to whether "luth_{nameof(Abc123)}" is a constant interpolated string so I'm just gonna hardcode it.
    public string Name => "luth_OnDoubleClick";
    public MouseEventArgs MouseEventArgs { get; }
    public ResourceUri ResourceUri { get; }
    public Key<TextEditorViewModel> ViewModelKey { get; }
    public TextEditorComponentData ComponentData { get; }

	public ITextEditorEditContext? EditContext { get; private set; }

    public IBackgroundTask? EarlyBatchOrDefault(IBackgroundTask oldEvent)
    {
		if (oldEvent.Name == Name)
		{
			// Replace the upstream event with this one,
			// because unhandled-consecutive events of this type are redundant.
			return this;
		}
        
		// Keep both events, because they are not able to be batched.
		return null;
    }
    
    public IBackgroundTask? LateBatchOrDefault(IBackgroundTask oldEvent)
    {
    	return null;
    }

    public async ValueTask HandleEvent(CancellationToken cancellationToken)
    {
    	EditContext = new TextEditorEditContext(
            ComponentData.TextEditorViewModelDisplay.TextEditorService,
            TextEditorService.AuthenticatedActionKey);
    
        var modelModifier = EditContext.GetModelModifier(ResourceUri, true);
        var viewModelModifier = EditContext.GetViewModelModifier(ViewModelKey);
        var cursorModifierBag = EditContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
        var primaryCursorModifier = EditContext.GetPrimaryCursorModifier(cursorModifierBag);

        if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
            return;

        var hasSelectedText = TextEditorSelectionHelper.HasSelectedText(primaryCursorModifier);

        if ((MouseEventArgs.Buttons & 1) != 1 && hasSelectedText)
            return; // Not pressing the left mouse button so assume ContextMenu is desired result.

        if (MouseEventArgs.ShiftKey)
            return; // Do not expand selection if user is holding shift

		// Labeling any ITextEditorEditContext -> JavaScript interop or Blazor StateHasChanged.
		// Reason being, these are likely to be huge optimizations (2024-05-29).
        var rowAndColumnIndex = await EventUtils.CalculateRowAndColumnIndex(
				ResourceUri,
				ViewModelKey,
				MouseEventArgs,
				ComponentData,
				EditContext)
			.ConfigureAwait(false);

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
            ? modelModifier.GetLineLength(rowAndColumnIndex.rowIndex)
            : higherColumnIndexExpansion;

        // Move user's cursor position to the higher expansion
        {
            primaryCursorModifier.LineIndex = rowAndColumnIndex.rowIndex;
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
        
        await EditContext.TextEditorService
			.FinalizePost(EditContext)
			.ConfigureAwait(false);
			
		// await Task.Delay(ThrottleFacts.TwentyFour_Frames_Per_Second).ConfigureAwait(false);
    }
}
