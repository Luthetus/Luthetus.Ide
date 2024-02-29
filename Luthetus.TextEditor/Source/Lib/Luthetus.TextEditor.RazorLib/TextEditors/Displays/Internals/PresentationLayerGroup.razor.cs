﻿using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels.Internals;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class PresentationLayerGroup : ComponentBase
{
    [CascadingParameter]
    public TextEditorRenderBatch RenderBatch { get; set; } = null!;

    [Parameter, EditorRequired]
    public TextEditorCursor PrimaryCursor { get; set; } = null!;
    [Parameter, EditorRequired]
    public string CssClassString { get; set; } = string.Empty;
    [Parameter, EditorRequired]
    public ImmutableList<Key<TextEditorPresentationModel>> TextEditorPresentationKeys { get; set; } = ImmutableList<Key<TextEditorPresentationModel>>.Empty;

    private List<TextEditorPresentationModel> GetTextEditorPresentationModels()
    {
        var textEditorPresentationModelList = new List<TextEditorPresentationModel>();

        foreach (var presentationKey in TextEditorPresentationKeys)
        {
            var textEditorPresentationModel = RenderBatch.Model!.PresentationModelList.FirstOrDefault(x =>
                x.TextEditorPresentationKey == presentationKey);

            if (textEditorPresentationModel is not null)
                textEditorPresentationModelList.Add(textEditorPresentationModel);
        }

        return textEditorPresentationModelList;
    }

    private string GetPresentationCssStyleString(
        int lowerPositionIndexInclusive,
        int upperPositionIndexExclusive,
        int rowIndex)
    {
        var charMeasurements = RenderBatch.ViewModel!.VirtualizationResult.CharAndRowMeasurements;
        var elementMeasurements = RenderBatch.ViewModel!.VirtualizationResult.TextEditorMeasurements;

        if (rowIndex >= RenderBatch.Model!.RowEndingPositionsList.Count)
            return string.Empty;

        var startOfRowTuple = RenderBatch.Model!.GetRowEndingThatCreatedRow(rowIndex);
        var endOfRowTuple = RenderBatch.Model!.RowEndingPositionsList[rowIndex];

        var startingColumnIndex = 0;
        var endingColumnIndex = endOfRowTuple.EndPositionIndexExclusive - 1;

        var fullWidthOfRowIsSelected = true;

        if (lowerPositionIndexInclusive > startOfRowTuple.EndPositionIndexExclusive)
        {
            startingColumnIndex = lowerPositionIndexInclusive - startOfRowTuple.EndPositionIndexExclusive;
            fullWidthOfRowIsSelected = false;
        }

        if (upperPositionIndexExclusive < endOfRowTuple.EndPositionIndexExclusive)
        {
            endingColumnIndex = upperPositionIndexExclusive - startOfRowTuple.EndPositionIndexExclusive;
            fullWidthOfRowIsSelected = false;
        }

        var topInPixelsInvariantCulture = (rowIndex * charMeasurements.RowHeight).ToCssValue();

        var top = $"top: {topInPixelsInvariantCulture}px;";

        var heightInPixelsInvariantCulture = charMeasurements.RowHeight.ToCssValue();

        var height = $"height: {heightInPixelsInvariantCulture}px;";

        var startInPixels = startingColumnIndex * charMeasurements.CharacterWidth;

        // startInPixels offset from Tab keys a width of many characters
        {
            var tabsOnSameRowBeforeCursor = RenderBatch.Model!.GetTabsCountOnSameRowBeforeCursor(
                rowIndex,
                startingColumnIndex);

            // 1 of the character width is already accounted for
            var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;

            startInPixels += extraWidthPerTabKey * tabsOnSameRowBeforeCursor * charMeasurements.CharacterWidth;
        }

        var startInPixelsInvariantCulture = startInPixels.ToCssValue();
        var left = $"left: {startInPixelsInvariantCulture}px;";

        var widthInPixels = endingColumnIndex * charMeasurements.CharacterWidth - startInPixels;

        // Tab keys a width of many characters
        {
            var tabsOnSameRowBeforeCursor = RenderBatch.Model!.GetTabsCountOnSameRowBeforeCursor(
                rowIndex,
                endingColumnIndex);

            // 1 of the character width is already accounted for
            var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;

            widthInPixels += extraWidthPerTabKey * tabsOnSameRowBeforeCursor * charMeasurements.CharacterWidth;
        }

        var widthCssStyleString = "width: ";

        var fullWidthValue = elementMeasurements.ScrollWidth;

        if (elementMeasurements.Width > elementMeasurements.ScrollWidth)
            fullWidthValue = elementMeasurements.Width; // If content does not fill the viewable width of the Text Editor User Interface

        var fullWidthValueInPixelsInvariantCulture = fullWidthValue.ToCssValue();

        var widthInPixelsInvariantCulture = widthInPixels.ToCssValue();

        if (fullWidthOfRowIsSelected)
            widthCssStyleString += $"{fullWidthValueInPixelsInvariantCulture}px;";
        else if (startingColumnIndex != 0 && upperPositionIndexExclusive > endOfRowTuple.EndPositionIndexExclusive - 1)
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
        TextEditorTextModification[] textModifications,
        ImmutableArray<TextEditorTextSpan> textSpans)
    {
        var outTextSpansList = new List<TextEditorTextSpan>();

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

            outTextSpansList.Add(textSpan with
            {
                StartingIndexInclusive = startingIndexInclusive,
                EndingIndexExclusive = endingIndexExclusive
            });
        }

        return outTextSpansList.ToImmutableArray();
    }
}