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
        var modelModifier = editContext.GetModelModifier(viewModel.ResourceUri, true);
        var cursorModifierBag = editContext.GetCursorModifierBag(viewModel);
        var primaryCursorModifier = cursorModifierBag.CursorModifier;

        if (modelModifier is null || viewModel is null || !cursorModifierBag.ConstructorWasInvoked || primaryCursorModifier is null)
            return;

        viewModel.ShouldRevealCursor = false;

        var hasSelectedText = TextEditorSelectionHelper.HasSelectedText(primaryCursorModifier);

        if ((MouseEventArgs.Buttons & 1) != 1 && hasSelectedText)
            return; // Not pressing the left mouse button so assume ContextMenu is desired result.

		if (viewModel.MenuKind != MenuKind.None)
		{
			TextEditorCommandDefaultFunctions.RemoveDropdown(
		        editContext,
		        viewModel,
		        ComponentData.DropdownService);
		}

        // Remember the current cursor position prior to doing anything
        var inRowIndex = primaryCursorModifier.LineIndex;
        var inColumnIndex = primaryCursorModifier.ColumnIndex;

        // Move the cursor position
		//
		// Labeling any ITextEditorEditContext -> JavaScript interop or Blazor StateHasChanged.
		// Reason being, these are likely to be huge optimizations (2024-05-29).
        var rowAndColumnIndex = await EventUtils.CalculateRowAndColumnIndex(
				viewModel.ResourceUri,
				ViewModelKey,
				MouseEventArgs,
				ComponentData,
				editContext)
			.ConfigureAwait(false);
			
		if (rowAndColumnIndex.positionX < 0)
		{
			var shouldGotoFinalize = Toggle(rowAndColumnIndex, modelModifier, viewModel);
			if (shouldGotoFinalize)
				goto finalize;
		}
		else
		{
			// Check for collision with non-tab inline UI
			foreach (var entry in viewModel.InlineUiList)
			{
				if (entry.InlineUi.InlineUiKind == InlineUiKind.Tab)
					continue;
			
				var lineAndColumnIndices = modelModifier.GetLineAndColumnIndicesFromPositionIndex(entry.InlineUi.PositionIndex);
				var lineInformation = modelModifier.GetLineInformation(lineAndColumnIndices.lineIndex);
			
				if (rowAndColumnIndex.rowIndex == lineInformation.Index)
				{
					if (rowAndColumnIndex.positionX > lineInformation.LastValidColumnIndex * viewModel.CharAndLineMeasurements.CharacterWidth + viewModel.CharAndLineMeasurements.CharacterWidth * 0.2)
					{
						var shouldGotoFinalize = Toggle(rowAndColumnIndex, modelModifier, viewModel);
						if (shouldGotoFinalize)
							goto finalize;
					}
				}
			}
		}

        primaryCursorModifier.LineIndex = rowAndColumnIndex.rowIndex;
        primaryCursorModifier.ColumnIndex = rowAndColumnIndex.columnIndex;
        primaryCursorModifier.PreferredColumnIndex = rowAndColumnIndex.columnIndex;

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
        
        finalize:
        
        editContext.TextEditorService.ViewModelApi.SetCursorShouldBlink(false);
        
        await editContext.TextEditorService
        	.FinalizePost(editContext)
        	.ConfigureAwait(false);
    }
    
    /// <summary>
    /// Returns whether you should goto finalize.
    /// </summary>
    private bool Toggle(
    	(int rowIndex, int columnIndex, double positionX, double positionY) rowAndColumnIndex,
    	TextEditorModel modelModifier,
    	TextEditorViewModel viewModel)
    {
    	var virtualizedIndexCollapsePoint = viewModel.VirtualizedCollapsePointList.FindIndex(x => x.AppendToLineIndex == rowAndColumnIndex.rowIndex);
		if (virtualizedIndexCollapsePoint != -1)
		{
			var allIndexCollapsePoint = viewModel.AllCollapsePointList.FindIndex(x => x.AppendToLineIndex == rowAndColumnIndex.rowIndex);
			if (allIndexCollapsePoint != -1)
			{
				var virtualizedCollapsePoint = viewModel.VirtualizedCollapsePointList[virtualizedIndexCollapsePoint];
				virtualizedCollapsePoint.IsCollapsed = !virtualizedCollapsePoint.IsCollapsed;
				viewModel.VirtualizedCollapsePointList[virtualizedIndexCollapsePoint] = virtualizedCollapsePoint;
				
				var allCollapsePoint = viewModel.AllCollapsePointList[allIndexCollapsePoint];
				allCollapsePoint.IsCollapsed = virtualizedCollapsePoint.IsCollapsed;
				viewModel.AllCollapsePointList[allIndexCollapsePoint] = allCollapsePoint;
				
				if (allCollapsePoint.IsCollapsed)
				{
					var firstToHideLineIndex = allCollapsePoint.AppendToLineIndex + 1;
					for (var lineOffset = 0; lineOffset < allCollapsePoint.EndExclusiveLineIndex - allCollapsePoint.AppendToLineIndex - 1; lineOffset++)
					{
						viewModel.HiddenLineIndexHashSet.Add(firstToHideLineIndex + lineOffset);
					}
				}
				else
				{
					viewModel.HiddenLineIndexHashSet.Clear();
					foreach (var collapsePoint in viewModel.AllCollapsePointList)
					{
						var firstToHideLineIndex = collapsePoint.AppendToLineIndex + 1;
						for (var lineOffset = 0; lineOffset < collapsePoint.EndExclusiveLineIndex - collapsePoint.AppendToLineIndex - 1; lineOffset++)
						{
							viewModel.HiddenLineIndexHashSet.Add(firstToHideLineIndex + lineOffset);
						}
					}
				}
				
				
				if (virtualizedCollapsePoint.IsCollapsed)
    			{
    				virtualizedIndexCollapsePoint = viewModel.VirtualizedCollapsePointList.FindIndex(x => x.AppendToLineIndex == rowAndColumnIndex.rowIndex);
    				
    				var lineInformation = modelModifier.GetLineInformation(virtualizedCollapsePoint.AppendToLineIndex);
    				
    				var inlineUi = new InlineUi(
    					positionIndex: lineInformation.UpperLineEnd.StartPositionIndexInclusive,
    					InlineUiKind.ThreeDotsExpandInlineUiThing);
    				
    				modelModifier.InlineUiList.Add(inlineUi);
    				viewModel.InlineUiList.Add(
    					(
    						inlineUi,
            				Tag: virtualizedCollapsePoint.Identifier
            			));
    			}
    			else
    			{
    				// TODO: Bad, this only permits one name regardless of scope
    				var indexTagMatchedInlineUi = viewModel.InlineUiList.FindIndex(
    					x => x.Tag == virtualizedCollapsePoint.Identifier);
    					
    				if (indexTagMatchedInlineUi != -1)
    				{
        				var indexModelInlineUi = modelModifier.InlineUiList.FindIndex(
    						x => x.PositionIndex == viewModel.InlineUiList[indexTagMatchedInlineUi].InlineUi.PositionIndex);
    					modelModifier.InlineUiList.RemoveAt(indexModelInlineUi);
        				
        				viewModel.InlineUiList.RemoveAt(indexTagMatchedInlineUi);
    				}
    			}
				
				viewModel.ShouldReloadVirtualizationResult = true;
				return true;
			}
		}
		
		return false;
    }
}
