using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class PresentationLayerGroup : ComponentBase
{
    [CascadingParameter]
    public TextEditorRenderBatch RenderBatch { get; set; } = null!;

    [Parameter, EditorRequired]
    public TextEditorCursorSnapshot PrimaryCursorSnapshot { get; set; } = null!;
    [Parameter, EditorRequired]
    public string CssClassString { get; set; } = string.Empty;
    [Parameter, EditorRequired]
    public ImmutableList<Key<TextEditorPresentationModel>> TextEditorPresentationKeys { get; set; } = ImmutableList<Key<TextEditorPresentationModel>>.Empty;

    private List<TextEditorPresentationModel> GetTextEditorPresentationModels()
    {
        var textEditorPresentationModelBag = new List<TextEditorPresentationModel>();

        foreach (var presentationKey in TextEditorPresentationKeys)
        {
            var textEditorPresentationModel = RenderBatch.Model!.PresentationModelsBag.FirstOrDefault(x =>
                x.TextEditorPresentationKey == presentationKey);

            if (textEditorPresentationModel is not null)
                textEditorPresentationModelBag.Add(textEditorPresentationModel);
        }

        return textEditorPresentationModelBag;
    }

    private string GetPresentationCssStyleString(
        int lowerPositionIndexInclusive,
        int upperPositionIndexExclusive,
        int rowIndex)
    {
        var charMeasurements = RenderBatch.ViewModel!.VirtualizationResult.CharacterWidthAndRowHeight;
        var elementMeasurements = RenderBatch.ViewModel!.VirtualizationResult.ElementMeasurementsInPixels;

        if (rowIndex >= RenderBatch.Model!.RowEndingPositionsBag.Length)
            return string.Empty;

        var startOfRowTuple = RenderBatch.Model!.GetStartOfRowTuple(rowIndex);
        var endOfRowTuple = RenderBatch.Model!.RowEndingPositionsBag[rowIndex];

        var startingColumnIndex = 0;
        var endingColumnIndex = endOfRowTuple.positionIndex - 1;

        var fullWidthOfRowIsSelected = true;

        if (lowerPositionIndexInclusive > startOfRowTuple.positionIndex)
        {
            startingColumnIndex = lowerPositionIndexInclusive - startOfRowTuple.positionIndex;
            fullWidthOfRowIsSelected = false;
        }

        if (upperPositionIndexExclusive < endOfRowTuple.positionIndex)
        {
            endingColumnIndex = upperPositionIndexExclusive - startOfRowTuple.positionIndex;
            fullWidthOfRowIsSelected = false;
        }

        var topInPixelsInvariantCulture = (rowIndex * charMeasurements.RowHeightInPixels).ToCssValue();

        var top = $"top: {topInPixelsInvariantCulture}px;";

        var heightInPixelsInvariantCulture = charMeasurements.RowHeightInPixels.ToCssValue();

        var height = $"height: {heightInPixelsInvariantCulture}px;";

        var startInPixels = startingColumnIndex * charMeasurements.CharacterWidthInPixels;

        // startInPixels offset from Tab keys a width of many characters
        {
            var tabsOnSameRowBeforeCursor = RenderBatch.Model!.GetTabsCountOnSameRowBeforeCursor(
                rowIndex,
                startingColumnIndex);

            // 1 of the character width is already accounted for
            var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;

            startInPixels += extraWidthPerTabKey * tabsOnSameRowBeforeCursor * charMeasurements.CharacterWidthInPixels;
        }

        var startInPixelsInvariantCulture = startInPixels.ToCssValue();
        var left = $"left: {startInPixelsInvariantCulture}px;";

        var widthInPixels = endingColumnIndex * charMeasurements.CharacterWidthInPixels - startInPixels;

        // Tab keys a width of many characters
        {
            var tabsOnSameRowBeforeCursor = RenderBatch.Model!.GetTabsCountOnSameRowBeforeCursor(
                rowIndex,
                endingColumnIndex);

            // 1 of the character width is already accounted for
            var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;

            widthInPixels += extraWidthPerTabKey * tabsOnSameRowBeforeCursor * charMeasurements.CharacterWidthInPixels;
        }

        var widthCssStyleString = "width: ";

        var fullWidthValue = elementMeasurements.ScrollWidth;

        if (elementMeasurements.Width > elementMeasurements.ScrollWidth)
            fullWidthValue = elementMeasurements.Width; // If content does not fill the viewable width of the Text Editor User Interface

        var fullWidthValueInPixelsInvariantCulture = fullWidthValue.ToCssValue();

        var widthInPixelsInvariantCulture = widthInPixels.ToCssValue();

        if (fullWidthOfRowIsSelected)
            widthCssStyleString += $"{fullWidthValueInPixelsInvariantCulture}px;";
        else if (startingColumnIndex != 0 && upperPositionIndexExclusive > endOfRowTuple.positionIndex - 1)
            widthCssStyleString += $"calc({fullWidthValueInPixelsInvariantCulture}px - {startInPixelsInvariantCulture}px);";
        else
            widthCssStyleString += $"{widthInPixelsInvariantCulture}px;";

        return $"position: absolute; {top} {height} {left} {widthCssStyleString}";
    }

    private string GetCssClass(TextEditorPresentationModel presentationModel, byte decorationByte)
    {
        return presentationModel.DecorationMapper.Map(decorationByte);
    }

    /// <summary>
    /// TODO: I don't think this logic is correct (ShiftTextSpans). I'm tired at this point, so I will leave this comment so I remember to look at this again. (2023-09-07)
    /// </summary>
    private ImmutableArray<TextEditorTextSpan> ShiftTextSpans(
        List<TextEditorTextModification> textModifications,
        ImmutableArray<TextEditorTextSpan> textSpans)
    {
        var outTextSpansBag = new List<TextEditorTextSpan>();

        foreach (var textSpan in textSpans)
        {
            var startingIndexInclusive = textSpan.StartingIndexInclusive;
            var endingIndexExclusive = textSpan.EndingIndexExclusive;

            foreach (var textModification in textModifications)
            {
                if (textModification.WasInsertion)
                {
                    if (startingIndexInclusive >= textModification.TextEditorTextSpan.StartingIndexInclusive)
                    {
                        startingIndexInclusive += textModification.TextEditorTextSpan.Length;
                        endingIndexExclusive += textModification.TextEditorTextSpan.Length;
                    }
                }
                else // was deletion
                {
                    if (startingIndexInclusive >= textModification.TextEditorTextSpan.StartingIndexInclusive)
                    {
                        startingIndexInclusive -= textModification.TextEditorTextSpan.Length;
                        endingIndexExclusive -= textModification.TextEditorTextSpan.Length;
                    }
                }
            }

            outTextSpansBag.Add(textSpan with
            {
                StartingIndexInclusive = startingIndexInclusive,
                EndingIndexExclusive = endingIndexExclusive
            });
        }

        return outTextSpansBag.ToImmutableArray();
    }
}