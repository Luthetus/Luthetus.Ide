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
			var virtualizedIndexGutterChevron = viewModel.VirtualizedGutterChevronList.FindIndex(x => x.LineIndex == rowAndColumnIndex.rowIndex);
			if (virtualizedIndexGutterChevron != -1)
			{
				var allIndexGutterChevron = viewModel.AllGutterChevronList.FindIndex(x => x.LineIndex == rowAndColumnIndex.rowIndex);
				if (allIndexGutterChevron != -1)
				{
					var virtualizedGutterChevron = viewModel.VirtualizedGutterChevronList[virtualizedIndexGutterChevron];
					virtualizedGutterChevron.IsExpanded = !virtualizedGutterChevron.IsExpanded;
					viewModel.VirtualizedGutterChevronList[virtualizedIndexGutterChevron] = virtualizedGutterChevron;
					
					var allGutterChevron = viewModel.AllGutterChevronList[allIndexGutterChevron];
					allGutterChevron.IsExpanded = virtualizedGutterChevron.IsExpanded;
					viewModel.AllGutterChevronList[allIndexGutterChevron] = allGutterChevron;
					
					viewModel.ShouldReloadVirtualizationResult = true;
					goto finalize;
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
}
