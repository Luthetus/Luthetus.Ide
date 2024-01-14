using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Htmls.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class TextSelectionRow : ComponentBase
{
    [Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

    [CascadingParameter]
    public TextEditorRenderBatch RenderBatch { get; set; } = null!;
    [CascadingParameter(Name = "ProportionalFontMeasurementsContainerElementId")]
    public string ProportionalFontMeasurementsContainerElementId { get; set; } = null!;

    [Parameter, EditorRequired]
    public string TextSelectionStyleCss { get; set; } = null!;
    [Parameter, EditorRequired]
    public int LowerPositionIndexInclusive { get; set; }
    [Parameter, EditorRequired]
    public int UpperPositionIndexExclusive { get; set; }
    [Parameter, EditorRequired]
    public int RowIndex { get; set; }

    private double _selectionStartingLeftRelativeToParentInPixels;
    private double _selectionWidthInPixels;
    private string _proportionalTextSelectionStyleCss = string.Empty;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!RenderBatch.Options!.UseMonospaceOptimizations)
            await GetTextSelectionStyleCssAsync();

        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task GetTextSelectionStyleCssAsync()
    {
        int lowerPositionIndexInclusive = LowerPositionIndexInclusive;
        int upperPositionIndexExclusive = UpperPositionIndexExclusive;
        int rowIndex = RowIndex;

        if (rowIndex >= RenderBatch.Model!.RowEndingPositionsList.Count)
            return;

        bool stateHasChanged = false;

        var startOfRowTuple = RenderBatch.Model!.GetRowEndingThatCreatedRow(rowIndex);
        var endOfRowTuple = RenderBatch.Model!.RowEndingPositionsList[rowIndex];

        var selectionStartingColumnIndex = 0;
        var selectionEndingColumnIndex = endOfRowTuple.EndPositionIndexExclusive - 1;

        var fullWidthOfRowIsSelected = true;

        if (lowerPositionIndexInclusive > startOfRowTuple.EndPositionIndexExclusive)
        {
            selectionStartingColumnIndex = lowerPositionIndexInclusive - startOfRowTuple.EndPositionIndexExclusive;
            fullWidthOfRowIsSelected = false;
        }

        if (upperPositionIndexExclusive < endOfRowTuple.EndPositionIndexExclusive)
        {
            selectionEndingColumnIndex = upperPositionIndexExclusive - startOfRowTuple.EndPositionIndexExclusive;
            fullWidthOfRowIsSelected = false;
        }

        var charMeasurements = RenderBatch.ViewModel!.VirtualizationResult.CharAndRowMeasurements;

        var topInPixelsInvariantCulture = (rowIndex * charMeasurements.RowHeight).ToCssValue();
        var top = $"top: {topInPixelsInvariantCulture}px;";

        var heightInPixelsInvariantCulture = charMeasurements.RowHeight.ToCssValue();
        var height = $"height: {heightInPixelsInvariantCulture}px;";

        // _selectionStartingLeftRelativeToParentInPixels
        {
            var selectionStartingCursor = new TextEditorCursor(
                rowIndex,
                selectionStartingColumnIndex,
                true);

            var textOffsettingCursor = RenderBatch.Model!
                .GetTextOffsettingCursor(selectionStartingCursor)
                .EscapeHtml();

            var guid = Guid.NewGuid();

            var nextSelectionStartingLeftRelativeToParentInPixels = await JsRuntime.InvokeAsync<double>(
                "luthetusTextEditor.calculateProportionalLeftOffset",
                ProportionalFontMeasurementsContainerElementId,
                $"luth_te_proportional-font-measurement-parent_{RenderBatch.ViewModel!.ViewModelKey.Guid}_selection_{guid}",
                $"luth_te_proportional-font-measurement-cursor_{RenderBatch.ViewModel!.ViewModelKey.Guid}_selection_{guid}",
                textOffsettingCursor,
                true);

            var previousSelectionStartingLeftRelativeToParentInPixels = _selectionStartingLeftRelativeToParentInPixels;

            _selectionStartingLeftRelativeToParentInPixels = nextSelectionStartingLeftRelativeToParentInPixels;

            if ((int)nextSelectionStartingLeftRelativeToParentInPixels !=
                (int)previousSelectionStartingLeftRelativeToParentInPixels)
            {
                stateHasChanged = true;
            }
        }

        var selectionStartInPixelsInvariantCulture = _selectionStartingLeftRelativeToParentInPixels.ToCssValue();
        var left = $"left: {selectionStartInPixelsInvariantCulture}px;";

        // _selectionWidthInPixels
        {
            var selectionEndingCursor = new TextEditorCursor(
                rowIndex,
                selectionEndingColumnIndex,
                true);

            var textOffsettingCursor = RenderBatch.Model!
                .GetTextOffsettingCursor(selectionEndingCursor)
                .EscapeHtml();

            var guid = Guid.NewGuid();

            var selectionEndingLeftRelativeToParentInPixels = await JsRuntime.InvokeAsync<double>(
                "luthetusTextEditor.calculateProportionalLeftOffset",
                ProportionalFontMeasurementsContainerElementId,
                $"luth_te_proportional-font-measurement-parent_{RenderBatch.ViewModel!.ViewModelKey.Guid}_selection_{guid}",
                $"luth_te_proportional-font-measurement-cursor_{RenderBatch.ViewModel!.ViewModelKey.Guid}_selection_{guid}",
                textOffsettingCursor,
                true);

            var nextSelectionWidthInPixels = selectionEndingLeftRelativeToParentInPixels -
                _selectionStartingLeftRelativeToParentInPixels;

            var previousSelectionWidthInPixels = _selectionWidthInPixels;

            _selectionWidthInPixels = nextSelectionWidthInPixels;

            if ((int)nextSelectionWidthInPixels != (int)previousSelectionWidthInPixels)
                stateHasChanged = true;
        }

        var widthCssStyleString = "width: ";

        var elementMeasurements = RenderBatch.ViewModel!.VirtualizationResult.TextEditorMeasurements;

        var fullWidthValue = elementMeasurements.ScrollWidth;

        if (elementMeasurements.Width > elementMeasurements.ScrollWidth)
            fullWidthValue = elementMeasurements.Width; // If content does not fill the viewable width of the Text Editor User Interface

        var fullWidthValueInPixelsInvariantCulture = fullWidthValue.ToCssValue();

        var selectionWidthInPixelsInvariantCulture = _selectionWidthInPixels.ToCssValue();

        if (fullWidthOfRowIsSelected)
            widthCssStyleString += $"{fullWidthValueInPixelsInvariantCulture}px;";
        else if (selectionStartingColumnIndex != 0 && upperPositionIndexExclusive > endOfRowTuple.EndPositionIndexExclusive - 1)
            widthCssStyleString += $"calc({fullWidthValueInPixelsInvariantCulture}px - {selectionStartInPixelsInvariantCulture}px);";
        else
            widthCssStyleString += $"{selectionWidthInPixelsInvariantCulture}px;";

        if (stateHasChanged)
            await InvokeAsync(StateHasChanged);

        _proportionalTextSelectionStyleCss = $"{top} {height} {left} {widthCssStyleString}";
    }
}