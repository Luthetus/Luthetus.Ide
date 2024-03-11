using Fluxor;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.Virtualizations.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class GutterSection : ComponentBase
{
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private LuthetusTextEditorConfig TextEditorConfig { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IServiceProvider ServiceProvider { get; set; } = null!;

    [CascadingParameter]
    public TextEditorRenderBatch RenderBatch { get; set; } = null!;

    // private double _scrollTop;

    //private readonly IThrottle _throttleSetGutterScrollTopFactory = new Throttle(TimeSpan.FromMilliseconds(500));

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        //var viewModel = RenderBatch.ViewModel!;
        //
        //if (_scrollTop != viewModel.VirtualizationResult.TextEditorMeasurements.ScrollTop)
        //{
        //    _throttleSetGutterScrollTopFactory.FireAndForget(_ =>
        //    {
        //        _scrollTop = viewModel.VirtualizationResult.TextEditorMeasurements.ScrollTop;
        //
        //        // TODO: Does 'SetGutterScrollTopAsync' need to be throttled? 
        //        TextEditorService.Post(
        //            nameof(TextEditorService.ViewModelApi.SetGutterScrollTopFactory),
        //            TextEditorService.ViewModelApi.SetGutterScrollTopFactory(
        //                viewModel.GutterElementId,
        //                viewModel.VirtualizationResult.TextEditorMeasurements.ScrollTop));
        //
        //        return Task.CompletedTask;
        //    });
        //}

        return base.OnAfterRenderAsync(firstRender);
    }

    private string GetGutterStyleCss(int index)
    {
        var measurements = RenderBatch.ViewModel!.VirtualizationResult.CharAndRowMeasurements;

        var topInPixelsInvariantCulture = (index * measurements.RowHeight).ToCssValue();
        var top = $"top: {topInPixelsInvariantCulture}px;";

        var heightInPixelsInvariantCulture = measurements.RowHeight.ToCssValue();
        var height = $"height: {heightInPixelsInvariantCulture}px;";

        var mostDigitsInARowLineNumber = RenderBatch.Model!.RowCount.ToString().Length;

        var widthInPixels = mostDigitsInARowLineNumber * measurements.CharacterWidth;
        widthInPixels += TextEditorModel.GUTTER_PADDING_LEFT_IN_PIXELS + TextEditorModel.GUTTER_PADDING_RIGHT_IN_PIXELS;

        var widthInPixelsInvariantCulture = widthInPixels.ToCssValue();
        var width = $"width: {widthInPixelsInvariantCulture}px;";

        var paddingLeftInPixelsInvariantCulture = TextEditorModel.GUTTER_PADDING_LEFT_IN_PIXELS.ToCssValue();
        var paddingLeft = $"padding-left: {paddingLeftInPixelsInvariantCulture}px;";

        var paddingRightInPixelsInvariantCulture = TextEditorModel.GUTTER_PADDING_RIGHT_IN_PIXELS.ToCssValue();
        var paddingRight = $"padding-right: {paddingRightInPixelsInvariantCulture}px;";

        return $"{width} {height} {top} {paddingLeft} {paddingRight}";
    }

    private string GetGutterSectionStyleCss()
    {
        var mostDigitsInARowLineNumber = RenderBatch.Model!.RowCount.ToString().Length;

        var widthInPixels = mostDigitsInARowLineNumber *
            RenderBatch.ViewModel!.VirtualizationResult.CharAndRowMeasurements.CharacterWidth;

        widthInPixels += TextEditorModel.GUTTER_PADDING_LEFT_IN_PIXELS + TextEditorModel.GUTTER_PADDING_RIGHT_IN_PIXELS;

        var widthInPixelsInvariantCulture = widthInPixels.ToCssValue();
        var width = $"width: {widthInPixelsInvariantCulture}px;";

        return width;
    }

    private IVirtualizationResultWithoutTypeMask GetVirtualizationResult()
    {
        var mostDigitsInARowLineNumber = RenderBatch.Model!.RowCount.ToString().Length;

        var widthOfGutterInPixels = mostDigitsInARowLineNumber *
            RenderBatch.ViewModel!.VirtualizationResult.CharAndRowMeasurements.CharacterWidth;

        widthOfGutterInPixels += TextEditorModel.GUTTER_PADDING_LEFT_IN_PIXELS + TextEditorModel.GUTTER_PADDING_RIGHT_IN_PIXELS;

        var topBoundaryNarrow = RenderBatch.ViewModel!.VirtualizationResult.TopVirtualizationBoundary with
        {
            WidthInPixels = widthOfGutterInPixels
        };

        var bottomBoundaryNarrow = RenderBatch.ViewModel!.VirtualizationResult.BottomVirtualizationBoundary with
        {
            WidthInPixels = widthOfGutterInPixels
        };

        return RenderBatch.ViewModel!.VirtualizationResult with
        {
            TopVirtualizationBoundary = topBoundaryNarrow,
            BottomVirtualizationBoundary = bottomBoundaryNarrow
        };
    }
}