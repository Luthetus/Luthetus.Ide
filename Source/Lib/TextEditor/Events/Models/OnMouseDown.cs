using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Commands.Models.Defaults;

namespace Luthetus.TextEditor.RazorLib.Events.Models;

public struct OnMouseDown
{
    public OnMouseDown(
        MouseEventArgs mouseEventArgs,
		TextEditorComponentData componentData,
        Key<TextEditorViewModel> viewModelKey)
    {
    	MouseEventArgs = mouseEventArgs;
		ComponentData = componentData;
        ViewModelKey = viewModelKey;
    }

    public MouseEventArgs MouseEventArgs { get; }
    public TextEditorComponentData ComponentData { get; }
    public Key<TextEditorViewModel> ViewModelKey { get; }

    public async ValueTask HandleEvent(CancellationToken cancellationToken)
    {
    	var editContext = new TextEditorEditContext(ComponentData.TextEditorViewModelSlimDisplay.TextEditorService);
    
    	var viewModel = editContext.GetViewModelModifier(ViewModelKey);
        var modelModifier = editContext.GetModelModifier(viewModel.PersistentState.ResourceUri, isReadOnly: true);

        if (modelModifier is null || viewModel is null)
            return;

        viewModel.PersistentState.ShouldRevealCursor = false;

        var hasSelectedText = TextEditorSelectionHelper.HasSelectedText(viewModel);

        if ((MouseEventArgs.Buttons & 1) != 1 && hasSelectedText)
            return; // Not pressing the left mouse button so assume ContextMenu is desired result.

		if (viewModel.PersistentState.MenuKind != MenuKind.None)
		{
			TextEditorCommandDefaultFunctions.RemoveDropdown(
		        editContext,
		        viewModel,
		        ComponentData.DropdownService);
		}

        // Remember the current cursor position prior to doing anything
        var inLineIndex = viewModel.LineIndex;
        var inColumnIndex = viewModel.ColumnIndex;

        // Move the cursor position
		//
		// Labeling any ITextEditorEditContext -> JavaScript interop or Blazor StateHasChanged.
		// Reason being, these are likely to be huge optimizations (2024-05-29).
        var lineAndColumnIndex = await EventUtils.CalculateLineAndColumnIndex(
				modelModifier,
				viewModel,
				MouseEventArgs,
				ComponentData,
				editContext)
			.ConfigureAwait(false);
			
		if (lineAndColumnIndex.PositionX < -4 &&
			lineAndColumnIndex.PositionX > -2 * viewModel.CharAndLineMeasurements.CharacterWidth)
		{
			var shouldGotoFinalize = TextEditorCommandDefaultFunctions.ToggleCollapsePoint(lineAndColumnIndex.LineIndex, modelModifier, viewModel);
			if (shouldGotoFinalize)
				goto finalize;
		}
		else
		{
			var lineInformation = modelModifier.GetLineInformation(lineAndColumnIndex.LineIndex);
			
			var lastValidColumnLeft = lineInformation.LastValidColumnIndex * viewModel.CharAndLineMeasurements.CharacterWidth;
			
			// Tab key column offset
	        {
	            var tabsOnSameLineBeforeCursor = modelModifier.GetTabCountOnSameLineBeforeCursor(
	                lineAndColumnIndex.LineIndex,
	                lineInformation.LastValidColumnIndex);
	
	            // 1 of the character width is already accounted for
	
	            var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;
	
	            lastValidColumnLeft += extraWidthPerTabKey *
	                tabsOnSameLineBeforeCursor *
	                viewModel.CharAndLineMeasurements.CharacterWidth;
	        }
			
			if (lineAndColumnIndex.PositionX > lastValidColumnLeft + viewModel.CharAndLineMeasurements.CharacterWidth * 0.2)
			{
				// Check for collision with non-tab inline UI
				foreach (var collapsePoint in viewModel.AllCollapsePointList)
				{
					if (collapsePoint.AppendToLineIndex != lineAndColumnIndex.LineIndex ||
					    !collapsePoint.IsCollapsed)
					{
						continue;
				    }
				
					if (lineAndColumnIndex.PositionX > lastValidColumnLeft + viewModel.CharAndLineMeasurements.CharacterWidth * 0.2)
					{
						if (lineAndColumnIndex.PositionX < lastValidColumnLeft + 3 * viewModel.CharAndLineMeasurements.CharacterWidth)
						{
							var shouldGotoFinalize = TextEditorCommandDefaultFunctions.ToggleCollapsePoint(lineAndColumnIndex.LineIndex, modelModifier, viewModel);
							if (shouldGotoFinalize)
								goto finalize;
						}
						else
						{
							var lastHiddenLineInformation = modelModifier.GetLineInformation(collapsePoint.EndExclusiveLineIndex - 1);
							viewModel.LineIndex = lastHiddenLineInformation.Index;
							viewModel.SetColumnIndexAndPreferred(lastHiddenLineInformation.LastValidColumnIndex);
							goto finalize;
						}
					}
				}
			}
		}

        viewModel.LineIndex = lineAndColumnIndex.LineIndex;
        viewModel.ColumnIndex = lineAndColumnIndex.ColumnIndex;
        viewModel.PreferredColumnIndex = lineAndColumnIndex.ColumnIndex;

        var cursorPositionIndex = modelModifier.GetPositionIndex(
            lineAndColumnIndex.LineIndex,
            lineAndColumnIndex.ColumnIndex);

        if (MouseEventArgs.ShiftKey)
        {
            if (!hasSelectedText)
            {
                // If user does not yet have a selection then place the text selection anchor were they were
                viewModel.SelectionAnchorPositionIndex = modelModifier
                    .GetPositionIndex(inLineIndex, inColumnIndex);
            }

            // If user ALREADY has a selection then do not modify the text selection anchor
        }
        else
        {
            viewModel.SelectionAnchorPositionIndex = cursorPositionIndex;
        }

        viewModel.SelectionEndingPositionIndex = cursorPositionIndex;
        
        finalize:
        
        editContext.TextEditorService.ViewModelApi.StopCursorBlinking();
        
        await editContext.TextEditorService
        	.FinalizePost(editContext)
        	.ConfigureAwait(false);
    }
}
