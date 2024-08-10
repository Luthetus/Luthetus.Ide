using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Virtualizations.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public class GutterDriver
{
	private readonly TextEditorViewModelDisplay _root;

	public GutterDriver(TextEditorViewModelDisplay textEditorViewModelDisplay)
	{
		_root = textEditorViewModelDisplay;
	}

	// Odd public but am middle of thinking
	public TextEditorRenderBatchValidated _renderBatch;

	public RenderFragment GetRenderFragment(TextEditorRenderBatchValidated renderBatch)
	{
		// Dangerous state can change mid run possible?
		_renderBatch = renderBatch;
		return GutterStaticRenderFragments.GetRenderFragment(this);
	}

    public string GetGutterStyleCss(int index)
    {
    	var renderBatchLocal = _renderBatch;
    	if (renderBatchLocal is null)
    		return string.Empty;
    		
        var measurements = renderBatchLocal.ViewModel.CharAndLineMeasurements;

        var topInPixelsInvariantCulture = (index * measurements.LineHeight).ToCssValue();
        var top = $"top: {topInPixelsInvariantCulture}px;";

        var heightInPixelsInvariantCulture = measurements.LineHeight.ToCssValue();
        var height = $"height: {heightInPixelsInvariantCulture}px;";

        var widthInPixelsInvariantCulture = renderBatchLocal.GutterWidthInPixels.ToCssValue();
        var width = $"width: {widthInPixelsInvariantCulture}px;";

        var paddingLeftInPixelsInvariantCulture = TextEditorModel.GUTTER_PADDING_LEFT_IN_PIXELS.ToCssValue();
        var paddingLeft = $"padding-left: {paddingLeftInPixelsInvariantCulture}px;";

        var paddingRightInPixelsInvariantCulture = TextEditorModel.GUTTER_PADDING_RIGHT_IN_PIXELS.ToCssValue();
        var paddingRight = $"padding-right: {paddingRightInPixelsInvariantCulture}px;";

        return $"{width} {height} {top} {paddingLeft} {paddingRight}";
    }

    public string GetGutterSectionStyleCss()
    {
    	var renderBatchLocal = _renderBatch;
    	if (renderBatchLocal is null)
    		return string.Empty;
    	
        var widthInPixelsInvariantCulture = renderBatchLocal.GutterWidthInPixels.ToCssValue();
        var width = $"width: {widthInPixelsInvariantCulture}px;";

        return width;
    }

	/// <summary>Why does this method exist? (2024-08-09)</summary>
    public IVirtualizationResultWithoutTypeMask GetVirtualizationResult()
    {
    	var renderBatchLocal = _renderBatch;
    	if (renderBatchLocal is null)
    		return VirtualizationResult<VirtualizationResult<List<RichCharacter>>>.GetEmptyRichCharacters();
    	
        var topBoundaryNarrow = renderBatchLocal.ViewModel.VirtualizationResult.TopVirtualizationBoundary with
        {
            WidthInPixels = renderBatchLocal.GutterWidthInPixels
        };

        var bottomBoundaryNarrow = renderBatchLocal.ViewModel.VirtualizationResult.BottomVirtualizationBoundary with
        {
            WidthInPixels = renderBatchLocal.GutterWidthInPixels
        };

        return renderBatchLocal.ViewModel.VirtualizationResult with
        {
            TopVirtualizationBoundary = topBoundaryNarrow,
            BottomVirtualizationBoundary = bottomBoundaryNarrow
        };
    }
}