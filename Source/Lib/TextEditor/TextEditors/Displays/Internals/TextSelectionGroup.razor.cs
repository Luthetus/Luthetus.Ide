﻿using Microsoft.AspNetCore.Components;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class TextSelectionGroup : ComponentBase
{
    [CascadingParameter]
    public RenderBatch RenderBatch { get; set; } = null!;

    [Parameter, EditorRequired]
    public TextEditorCursor PrimaryCursor { get; set; } = null!;

    private string GetTextSelectionStyleCss(
        int lowerPositionIndexInclusive,
        int upperPositionIndexExclusive,
        int rowIndex)
    {
        if (rowIndex >= RenderBatch.Model!.LineEndPositionList.Count)
            return string.Empty;

        var startOfRowTuple = RenderBatch.Model!.GetLineOpening(rowIndex);
        var endOfRowTuple = RenderBatch.Model!.LineEndPositionList[rowIndex];

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

        var charMeasurements = RenderBatch.ViewModel!.VirtualizationResult.CharAndLineMeasurements;

        var topInPixelsInvariantCulture = (rowIndex * charMeasurements.LineHeight).ToCssValue();
        var top = $"top: {topInPixelsInvariantCulture}px;";

        var heightInPixelsInvariantCulture = charMeasurements.LineHeight.ToCssValue();
        var height = $"height: {heightInPixelsInvariantCulture}px;";

        var selectionStartInPixels = selectionStartingColumnIndex * charMeasurements.CharacterWidth;

        // selectionStartInPixels offset from Tab keys a width of many characters
        {
            var tabsOnSameRowBeforeCursor = RenderBatch.Model.GetTabsCountOnSameLineBeforeCursor(
                rowIndex,
                selectionStartingColumnIndex);

            // 1 of the character width is already accounted for
            var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;

            selectionStartInPixels += 
                extraWidthPerTabKey * tabsOnSameRowBeforeCursor * charMeasurements.CharacterWidth;
        }

        var selectionStartInPixelsInvariantCulture = selectionStartInPixels.ToCssValue();
        var left = $"left: {selectionStartInPixelsInvariantCulture}px;";

        var selectionWidthInPixels = 
            selectionEndingColumnIndex * charMeasurements.CharacterWidth - selectionStartInPixels;

        // Tab keys a width of many characters
        {
            var tabsOnSameRowBeforeCursor = RenderBatch.Model.GetTabsCountOnSameLineBeforeCursor(
                rowIndex,
                selectionEndingColumnIndex);

            // 1 of the character width is already accounted for
            var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;

            selectionWidthInPixels += extraWidthPerTabKey * tabsOnSameRowBeforeCursor * charMeasurements.CharacterWidth;
        }

        var widthCssStyleString = "width: ";
        var fullWidthValue = RenderBatch.ViewModel.VirtualizationResult.TextEditorMeasurements.ScrollWidth;

        if (RenderBatch.ViewModel.VirtualizationResult.TextEditorMeasurements.Width >
            RenderBatch.ViewModel.VirtualizationResult.TextEditorMeasurements.ScrollWidth)
        {
            // If content does not fill the viewable width of the Text Editor User Interface
            fullWidthValue = RenderBatch.ViewModel.VirtualizationResult.TextEditorMeasurements.Width;
        }

        var fullWidthValueInPixelsInvariantCulture = fullWidthValue.ToCssValue();

        var selectionWidthInPixelsInvariantCulture = selectionWidthInPixels.ToCssValue();

        if (fullWidthOfRowIsSelected)
            widthCssStyleString += $"{fullWidthValueInPixelsInvariantCulture}px;";
        else if (selectionStartingColumnIndex != 0 &&
                 upperPositionIndexExclusive > endOfRowTuple.EndPositionIndexExclusive - 1)
            widthCssStyleString += $"calc({fullWidthValueInPixelsInvariantCulture}px - {selectionStartInPixelsInvariantCulture}px);";
        else
            widthCssStyleString += $"{selectionWidthInPixelsInvariantCulture}px;";

        return $"{top} {height} {left} {widthCssStyleString}";
    }
}