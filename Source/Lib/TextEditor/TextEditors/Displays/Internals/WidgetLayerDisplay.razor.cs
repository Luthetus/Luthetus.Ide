using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class WidgetLayerDisplay : ComponentBase
{
	[Inject]
	private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;
	
    [Parameter, EditorRequired]
    public TextEditorRenderBatch RenderBatch { get; set; }
    
    /// <summary>
    /// This does not change per WidgetBlock rendered.
    /// So, it should be invoked once and then re-used.
    /// </summary>
    private string GetBlockWidth(TextEditorRenderBatch localRenderBatch)
    {    	
    	var widthInPixels = RenderBatch.ViewModel.TextEditorDimensions.Width -
            ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS;

        var widthInPixelsInvariantCulture = widthInPixels.ToCssValue();
        var width = $"width: {widthInPixelsInvariantCulture}px;";

        return width;
    }
    
    private string GetBlockCssStyle(
    	TextEditorRenderBatch localRenderBatch,
    	int lineIndex)
    {
    	var topInPixels = lineIndex * localRenderBatch.ViewModel.CharAndLineMeasurements.LineHeight;
        var topInPixelsInvariantCulture = topInPixels.ToCssValue();
        var top = $"top: {topInPixelsInvariantCulture}px;";
        
		var heightInPixels = localRenderBatch.ViewModel.TextEditorDimensions.Height -
			localRenderBatch.ViewModel.CharAndLineMeasurements.LineHeight;
			
        var heightInPixelsInvariantCulture = heightInPixels.ToCssValue();
        var height = $"height: {heightInPixelsInvariantCulture}px;";

        return height + ' ' + top;
    }
    
    /// <summary>
    /// TODO: Anonymous lambda as an eventhandler warning...
    ///       ...so long as the amount of widgets rendered isn't large
    ///       this shouldn't be an issue.
    /// </summary>
    private void CloseWidgetOnClick(TextEditorRenderBatch localRenderBatch, WidgetBlock widget)
    {
    	/*TextEditorService.TextEditorWorker.PostUnique(
    		nameof(CloseWidgetOnClick),
    		editContext =>
    		{
    			var viewModelModifier = editContext.GetViewModelModifier(localRenderBatch.ViewModel.ViewModelKey);

                var index = viewModelModifier.WidgetBlockList.FindIndex(x => x.Key == widget.Key);

				var outWidgetBlockList = viewModelModifier.WidgetBlockList;
				outWidgetBlockList.RemoveAt(index);

				viewModelModifier.WidgetBlockList = outWidgetBlockList;
    			
    			return ValueTask.CompletedTask;
    		});*/
    }
}