using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Virtualizations.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class GutterSection : ComponentBase
{
    [CascadingParameter]
    public TextEditorRenderBatchValidated RenderBatch { get; set; } = null!;

    private string GetGutterStyleCss(int index)
    {
        var measurements = RenderBatch.ViewModel.VirtualizationResult.CharAndLineMeasurements;

        var topInPixelsInvariantCulture = (index * measurements.LineHeight).ToCssValue();
        var top = $"top: {topInPixelsInvariantCulture}px;";

        var heightInPixelsInvariantCulture = measurements.LineHeight.ToCssValue();
        var height = $"height: {heightInPixelsInvariantCulture}px;";

        var widthInPixelsInvariantCulture = RenderBatch.GutterWidthInPixels.ToCssValue();
        var width = $"width: {widthInPixelsInvariantCulture}px;";

        var paddingLeftInPixelsInvariantCulture = TextEditorModel.GUTTER_PADDING_LEFT_IN_PIXELS.ToCssValue();
        var paddingLeft = $"padding-left: {paddingLeftInPixelsInvariantCulture}px;";

        var paddingRightInPixelsInvariantCulture = TextEditorModel.GUTTER_PADDING_RIGHT_IN_PIXELS.ToCssValue();
        var paddingRight = $"padding-right: {paddingRightInPixelsInvariantCulture}px;";

        return $"{width} {height} {top} {paddingLeft} {paddingRight}";
    }

    private string GetGutterSectionStyleCss()
    {
        var widthInPixelsInvariantCulture = RenderBatch.GutterWidthInPixels.ToCssValue();
        var width = $"width: {widthInPixelsInvariantCulture}px;";

        return width;
    }

    private IVirtualizationResultWithoutTypeMask GetVirtualizationResult()
    {
        var topBoundaryNarrow = RenderBatch.ViewModel.VirtualizationResult.TopVirtualizationBoundary with
        {
            WidthInPixels = RenderBatch.GutterWidthInPixels
        };

        var bottomBoundaryNarrow = RenderBatch.ViewModel.VirtualizationResult.BottomVirtualizationBoundary with
        {
            WidthInPixels = RenderBatch.GutterWidthInPixels
        };

        return RenderBatch.ViewModel.VirtualizationResult with
        {
            TopVirtualizationBoundary = topBoundaryNarrow,
            BottomVirtualizationBoundary = bottomBoundaryNarrow
        };
    }
}