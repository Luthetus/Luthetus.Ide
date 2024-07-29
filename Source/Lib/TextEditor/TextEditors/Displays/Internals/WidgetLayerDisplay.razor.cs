using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class WidgetLayerDisplay : ComponentBase
{
    [CascadingParameter]
    public TextEditorRenderBatchValidated RenderBatch { get; set; } = null!;
    
    private string GetWidgetCssStyleBlock(TextEditorRenderBatchValidated localRenderBatch)
    {
    	var widthInPixels = RenderBatch.ViewModel.TextEditorDimensions.Width -
            ScrollbarFacts.SCROLLBAR_SIZE_IN_PIXELS;

        var widthInPixelsInvariantCulture = widthInPixels.ToCssValue();
        var width = $"width: {widthInPixelsInvariantCulture}px;";

        return width;
    }
}