using Microsoft.AspNetCore.Components;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.Common.RazorLib.Dimensions.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class TextSelectionGroup : ComponentBase
{
    [CascadingParameter]
    public TextEditorRenderBatch RenderBatch { get; set; } = null!;

    [Parameter, EditorRequired]
    public TextEditorCursorSnapshot PrimaryCursorSnapshot { get; set; } = null!;

    private string GetTextSelectionStyleCss(
        int lowerPositionIndexInclusive,
        int upperPositionIndexExclusive,
        int rowIndex)
    {
        if (rowIndex >= RenderBatch.Model!.RowEndingPositionsBag.Length)
            return string.Empty;

        var startOfRowTuple = RenderBatch.Model!.GetStartOfRowTuple(rowIndex);
        var endOfRowTuple = RenderBatch.Model!.RowEndingPositionsBag[rowIndex];

        var selectionStartingColumnIndex = 0;
        var selectionEndingColumnIndex = endOfRowTuple.positionIndex - 1;

        var fullWidthOfRowIsSelected = true;

        if (lowerPositionIndexInclusive > startOfRowTuple.positionIndex)
        {
            selectionStartingColumnIndex = lowerPositionIndexInclusive - startOfRowTuple.positionIndex;
            fullWidthOfRowIsSelected = false;
        }

        if (upperPositionIndexExclusive < endOfRowTuple.positionIndex)
        {
            selectionEndingColumnIndex = upperPositionIndexExclusive - startOfRowTuple.positionIndex;
            fullWidthOfRowIsSelected = false;
        }

        var charMeasurements = RenderBatch.ViewModel!.VirtualizationResult.CharacterWidthAndRowHeight;

        var topInPixelsInvariantCulture = (rowIndex * charMeasurements.RowHeightInPixels).ToCssValue();
        var top = $"top: {topInPixelsInvariantCulture}px;";

        var heightInPixelsInvariantCulture = charMeasurements.RowHeightInPixels.ToCssValue();
        var height = $"height: {heightInPixelsInvariantCulture}px;";

        var selectionStartInPixels = selectionStartingColumnIndex * charMeasurements.CharacterWidthInPixels;

        // selectionStartInPixels offset from Tab keys a width of many characters
        {
            var tabsOnSameRowBeforeCursor = RenderBatch.Model.GetTabsCountOnSameRowBeforeCursor(
                rowIndex,
                selectionStartingColumnIndex);

            // 1 of the character width is already accounted for
            var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;

            selectionStartInPixels += extraWidthPerTabKey *
                tabsOnSameRowBeforeCursor *
                charMeasurements.CharacterWidthInPixels;
        }

        var selectionStartInPixelsInvariantCulture = selectionStartInPixels.ToCssValue();
        var left = $"left: {selectionStartInPixelsInvariantCulture}px;";

        var selectionWidthInPixels = selectionEndingColumnIndex *
            charMeasurements.CharacterWidthInPixels -
            selectionStartInPixels;

        // Tab keys a width of many characters
        {
            var tabsOnSameRowBeforeCursor = RenderBatch.Model.GetTabsCountOnSameRowBeforeCursor(
                rowIndex,
                selectionEndingColumnIndex);

            // 1 of the character width is already accounted for
            var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;

            selectionWidthInPixels += extraWidthPerTabKey * tabsOnSameRowBeforeCursor * charMeasurements.CharacterWidthInPixels;
        }

        var widthCssStyleString = "width: ";
        var fullWidthValue = RenderBatch.ViewModel.VirtualizationResult.ElementMeasurementsInPixels.ScrollWidth;

        if (RenderBatch.ViewModel.VirtualizationResult.ElementMeasurementsInPixels.Width >
            RenderBatch.ViewModel.VirtualizationResult.ElementMeasurementsInPixels.ScrollWidth)
        {
            // If content does not fill the viewable width of the Text Editor User Interface
            fullWidthValue = RenderBatch.ViewModel.VirtualizationResult.ElementMeasurementsInPixels.Width;
        }

        var fullWidthValueInPixelsInvariantCulture = fullWidthValue.ToCssValue();

        var selectionWidthInPixelsInvariantCulture = selectionWidthInPixels.ToCssValue();

        if (fullWidthOfRowIsSelected)
            widthCssStyleString += $"{fullWidthValueInPixelsInvariantCulture}px;";
        else if (selectionStartingColumnIndex != 0 &&
                 upperPositionIndexExclusive > endOfRowTuple.positionIndex - 1)
            widthCssStyleString += $"calc({fullWidthValueInPixelsInvariantCulture}px - {selectionStartInPixelsInvariantCulture}px);";
        else
            widthCssStyleString += $"{selectionWidthInPixelsInvariantCulture}px;";

        return $"{top} {height} {left} {widthCssStyleString}";
    }
}