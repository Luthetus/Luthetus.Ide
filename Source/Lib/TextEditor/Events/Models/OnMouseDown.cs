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
        var modelModifier = editContext.GetModelModifier(viewModel.ResourceUri, isReadOnly: true);
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
        var inLineIndex = primaryCursorModifier.LineIndex;
        var inColumnIndex = primaryCursorModifier.ColumnIndex;

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
			var shouldGotoFinalize = TextEditorCommandDefaultFunctions.ToggleCollapsePoint(lineAndColumnIndex.LineIndex, modelModifier, viewModel, primaryCursorModifier);
			if (shouldGotoFinalize)
				goto finalize;
		}
		else
		{
			var lineInformation = modelModifier.GetLineInformation(lineAndColumnIndex.LineIndex);
			
			if (lineAndColumnIndex.PositionX > lineInformation.LastValidColumnIndex * viewModel.CharAndLineMeasurements.CharacterWidth + viewModel.CharAndLineMeasurements.CharacterWidth * 0.2)
			{
				// Check for collision with non-tab inline UI
				foreach (var collapsePoint in viewModel.AllCollapsePointList)
				{
					if (collapsePoint.AppendToLineIndex != lineAndColumnIndex.LineIndex ||
					    !collapsePoint.IsCollapsed)
					{
						continue;
				    }
				
					if (lineAndColumnIndex.PositionX > lineInformation.LastValidColumnIndex * viewModel.CharAndLineMeasurements.CharacterWidth + viewModel.CharAndLineMeasurements.CharacterWidth * 0.2)
					{
						if (lineAndColumnIndex.PositionX < (lineInformation.LastValidColumnIndex + 3) * viewModel.CharAndLineMeasurements.CharacterWidth)
						{
							var shouldGotoFinalize = TextEditorCommandDefaultFunctions.ToggleCollapsePoint(lineAndColumnIndex.LineIndex, modelModifier, viewModel, primaryCursorModifier);
							if (shouldGotoFinalize)
								goto finalize;
						}
						else
						{
							var lastHiddenLineInformation = modelModifier.GetLineInformation(collapsePoint.EndExclusiveLineIndex - 1);
							primaryCursorModifier.LineIndex = lastHiddenLineInformation.Index;
							primaryCursorModifier.SetColumnIndexAndPreferred(lastHiddenLineInformation.LastValidColumnIndex);
							goto finalize;
						}
					}
				}
			}
		}

        primaryCursorModifier.LineIndex = lineAndColumnIndex.LineIndex;
        primaryCursorModifier.ColumnIndex = lineAndColumnIndex.ColumnIndex;
        primaryCursorModifier.PreferredColumnIndex = lineAndColumnIndex.ColumnIndex;

        var cursorPositionIndex = modelModifier.GetPositionIndex(new TextEditorCursor(
            lineAndColumnIndex.LineIndex,
            lineAndColumnIndex.ColumnIndex,
            true));

        if (MouseEventArgs.ShiftKey)
        {
            if (!hasSelectedText)
            {
                // If user does not yet have a selection then place the text selection anchor were they were
                primaryCursorModifier.SelectionAnchorPositionIndex = modelModifier
                    .GetPositionIndex(inLineIndex, inColumnIndex);
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
}
