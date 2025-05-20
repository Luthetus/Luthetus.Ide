using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

namespace Luthetus.TextEditor.RazorLib.Events.Models;

public struct OnMouseMove
{
    public OnMouseMove(
        MouseEventArgs mouseEventArgs,
		TextEditorComponentData componentData,
        Key<TextEditorViewModel> viewModelKey)
    {
    	MouseEventArgs = mouseEventArgs;
		ComponentData = componentData;
        ViewModelKey = viewModelKey;
    }

    public MouseEventArgs MouseEventArgs { get; }
    public Key<TextEditorViewModel> ViewModelKey { get; }
	public TextEditorComponentData ComponentData { get; }

    public async ValueTask HandleEvent(CancellationToken cancellationToken)
    {
    	var editContext = new TextEditorEditContext(ComponentData.TextEditorViewModelSlimDisplay.TextEditorService);
    
        var viewModel = editContext.GetViewModelModifier(ViewModelKey);
        var modelModifier = editContext.GetModelModifier(viewModel.PersistentState.ResourceUri, isReadOnly: true);

        if (modelModifier is null || viewModel is null)
            return;

		// Labeling any ITextEditorEditContext -> JavaScript interop or Blazor StateHasChanged.
		// Reason being, these are likely to be huge optimizations (2024-05-29).
        var rowAndColumnIndex = await EventUtils.CalculateLineAndColumnIndex(
				modelModifier,
				viewModel,
				MouseEventArgs,
				ComponentData,
				editContext)
			.ConfigureAwait(false);
			
		if (rowAndColumnIndex.PositionX < -4 &&
			rowAndColumnIndex.PositionX > -2 * viewModel.CharAndLineMeasurements.CharacterWidth)
		{
			var virtualizedIndexCollapsePoint = viewModel.VirtualizedCollapsePointList.FindIndex(x => x.AppendToLineIndex == rowAndColumnIndex.LineIndex);
			
			if (virtualizedIndexCollapsePoint != -1)
				goto finalize;
		}
		else
		{
			var lineInformation = modelModifier.GetLineInformation(rowAndColumnIndex.LineIndex);
			
			if (rowAndColumnIndex.PositionX > lineInformation.LastValidColumnIndex * viewModel.CharAndLineMeasurements.CharacterWidth + viewModel.CharAndLineMeasurements.CharacterWidth * 0.2)
			{
				// Check for collision with non-tab inline UI
				foreach (var collapsePoint in viewModel.AllCollapsePointList)
				{
					if (collapsePoint.AppendToLineIndex != rowAndColumnIndex.LineIndex ||
					    !collapsePoint.IsCollapsed)
					{
						continue;
				    }
				
					if (rowAndColumnIndex.PositionX > lineInformation.LastValidColumnIndex * viewModel.CharAndLineMeasurements.CharacterWidth + viewModel.CharAndLineMeasurements.CharacterWidth * 0.2)
					{
						var lastHiddenLineInformation = modelModifier.GetLineInformation(collapsePoint.EndExclusiveLineIndex - 1);
						viewModel.LineIndex = lastHiddenLineInformation.Index;
						viewModel.SetColumnIndexAndPreferred(lastHiddenLineInformation.LastValidColumnIndex);
						goto finalize;
					}
				}
			}
		}

        viewModel.LineIndex = rowAndColumnIndex.LineIndex;
        viewModel.ColumnIndex = rowAndColumnIndex.ColumnIndex;
        viewModel.PreferredColumnIndex = rowAndColumnIndex.ColumnIndex;

		// editContext.TextEditorService.ViewModelApi.SetCursorShouldBlink(false);

        viewModel.SelectionEndingPositionIndex = modelModifier.GetPositionIndex(viewModel);
        
        finalize:
	
		editContext.TextEditorService.ViewModelApi.StopCursorBlinking();
	
		await editContext.TextEditorService
			.FinalizePost(editContext)
			.ConfigureAwait(false);
    }
}
