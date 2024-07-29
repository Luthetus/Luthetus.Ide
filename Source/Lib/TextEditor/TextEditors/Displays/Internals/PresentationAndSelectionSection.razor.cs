using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Htmls.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class PresentationAndSelectionSection : ComponentBase
{
	[Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;
	[Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    
	[CascadingParameter]
    public TextEditorRenderBatchValidated RenderBatch { get; set; } = null!;
    [CascadingParameter(Name = "ProportionalFontMeasurementsContainerElementId")]
    public string ProportionalFontMeasurementsContainerElementId { get; set; } = null!;

    [Parameter, EditorRequired]
    public TextEditorCursor PrimaryCursor { get; set; } = null!;

    private List<TextEditorPresentationModel> GetTextEditorPresentationModels(
    	ImmutableList<Key<TextEditorPresentationModel>> textEditorPresentationKeys)
    {
        var textEditorPresentationModelList = new List<TextEditorPresentationModel>();

        foreach (var presentationKey in textEditorPresentationKeys)
        {
            var textEditorPresentationModel = RenderBatch.Model.PresentationModelList.FirstOrDefault(x =>
                x.TextEditorPresentationKey == presentationKey);

            if (textEditorPresentationModel is not null)
                textEditorPresentationModelList.Add(textEditorPresentationModel);
        }

        return textEditorPresentationModelList;
    }

    private string PresentationGetCssStyleString(
        int lowerPositionIndexInclusive,
        int upperPositionIndexExclusive,
        int rowIndex)
    {
        try
        {
            var charMeasurements = RenderBatch.ViewModel.CharAndLineMeasurements;
			var textEditorDimensions = RenderBatch.ViewModel.TextEditorDimensions;
            var scrollbarDimensions = RenderBatch.ViewModel.ScrollbarDimensions;

            if (rowIndex >= RenderBatch.Model.LineEndList.Count)
                return string.Empty;

            var line = RenderBatch.Model.GetLineInformation(rowIndex);

            var startingColumnIndex = 0;
            var endingColumnIndex = line.EndPositionIndexExclusive - 1;

            var fullWidthOfRowIsSelected = true;

            if (lowerPositionIndexInclusive > line.StartPositionIndexInclusive)
            {
                startingColumnIndex = lowerPositionIndexInclusive - line.StartPositionIndexInclusive;
                fullWidthOfRowIsSelected = false;
            }

            if (upperPositionIndexExclusive < line.EndPositionIndexExclusive)
            {
                endingColumnIndex = upperPositionIndexExclusive - line.StartPositionIndexInclusive;
                fullWidthOfRowIsSelected = false;
            }

            var topInPixelsInvariantCulture = (rowIndex * charMeasurements.LineHeight).ToCssValue();

            var top = $"top: {topInPixelsInvariantCulture}px;";

            var heightInPixelsInvariantCulture = charMeasurements.LineHeight.ToCssValue();

            var height = $"height: {heightInPixelsInvariantCulture}px;";

            var startInPixels = startingColumnIndex * charMeasurements.CharacterWidth;

            // startInPixels offset from Tab keys a width of many characters
            {
                var tabsOnSameRowBeforeCursor = RenderBatch.Model.GetTabCountOnSameLineBeforeCursor(
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
                var tabsOnSameRowBeforeCursor = RenderBatch.Model.GetTabCountOnSameLineBeforeCursor(
                    rowIndex,
                    endingColumnIndex);

                // 1 of the character width is already accounted for
                var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;

                widthInPixels += extraWidthPerTabKey * tabsOnSameRowBeforeCursor * charMeasurements.CharacterWidth;
            }

            var widthCssStyleString = "width: ";

            var fullWidthValue = scrollbarDimensions.ScrollWidth;

            if (textEditorDimensions.Width > scrollbarDimensions.ScrollWidth)
                fullWidthValue = textEditorDimensions.Width; // If content does not fill the viewable width of the Text Editor User Interface

            var fullWidthValueInPixelsInvariantCulture = fullWidthValue.ToCssValue();

            var widthInPixelsInvariantCulture = widthInPixels.ToCssValue();

            if (fullWidthOfRowIsSelected)
                widthCssStyleString += $"{fullWidthValueInPixelsInvariantCulture}px;";
            else if (startingColumnIndex != 0 && upperPositionIndexExclusive > line.EndPositionIndexExclusive - 1)
                widthCssStyleString += $"calc({fullWidthValueInPixelsInvariantCulture}px - {startInPixelsInvariantCulture}px);";
            else
                widthCssStyleString += $"{widthInPixelsInvariantCulture}px;";

            return $"position: absolute; {top} {height} {left} {widthCssStyleString}";
        }
        catch (LuthetusTextEditorException)
        {
            return string.Empty;
        }
    }

    private string PresentationGetCssClass(TextEditorPresentationModel presentationModel, byte decorationByte)
    {
        return presentationModel.DecorationMapper.Map(decorationByte);
    }

    private ImmutableArray<TextEditorTextSpan> PresentationVirtualizeAndShiftTextSpans(
        TextEditorTextModification[] textModifications,
        ImmutableArray<TextEditorTextSpan> inTextSpanList)
    {
        try
        {
            // Virtualize the text spans
            var virtualizedTextSpanList = new List<TextEditorTextSpan>();
            if (RenderBatch.ViewModel.VirtualizationResult?.EntryList.Any() ?? false)
            {
                var lowerLineIndexInclusive = RenderBatch.ViewModel.VirtualizationResult.EntryList.First().Index;
                var upperLineIndexInclusive = RenderBatch.ViewModel.VirtualizationResult.EntryList.Last().Index;

                var lowerLine = RenderBatch.Model.GetLineInformation(lowerLineIndexInclusive);
                var upperLine = RenderBatch.Model.GetLineInformation(upperLineIndexInclusive);

                foreach (var textSpan in inTextSpanList)
                {
                    if (lowerLine.StartPositionIndexInclusive <= textSpan.StartingIndexInclusive &&
                        upperLine.EndPositionIndexExclusive >= textSpan.StartingIndexInclusive)
                    {
                        virtualizedTextSpanList.Add(textSpan);
                    }
                }
            }
            else
            {
                // No 'VirtualizationResult', so don't render any text spans.
                return ImmutableArray<TextEditorTextSpan>.Empty;
            }

            var outTextSpansList = new List<TextEditorTextSpan>();
            // Shift the text spans
            {
                foreach (var textSpan in virtualizedTextSpanList)
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
            }

            return outTextSpansList.ToImmutableArray();
        }
        catch (LuthetusTextEditorException)
        {
            return ImmutableArray<TextEditorTextSpan>.Empty;
        }
    }

    private (int FirstRowToSelectDataInclusive, int LastRowToSelectDataExclusive) PresentationGetBoundsInRowIndexUnits(TextEditorModel model, (int StartingIndexInclusive, int EndingIndexExclusive) boundsInPositionIndexUnits)
    {
        try
        {
            var firstRowToSelectDataInclusive = RenderBatch.Model
                .GetLineInformationFromPositionIndex(boundsInPositionIndexUnits.StartingIndexInclusive)
                .Index;

            var lastRowToSelectDataExclusive = RenderBatch.Model
                .GetLineInformationFromPositionIndex(boundsInPositionIndexUnits.EndingIndexExclusive)
                .Index +
                1;

            return (firstRowToSelectDataInclusive, lastRowToSelectDataExclusive);
        }
        catch (LuthetusTextEditorException)
        {
            return (0, 0);
        }
    }
    
    private string GetTextSelectionStyleCss(
        int lowerPositionIndexInclusive,
        int upperPositionIndexExclusive,
        int rowIndex)
    {
		try
		{
	        if (rowIndex >= RenderBatch.Model.LineEndList.Count)
	            return string.Empty;
	
	        var line = RenderBatch.Model.GetLineInformation(rowIndex);
	
	        var selectionStartingColumnIndex = 0;
	        var selectionEndingColumnIndex = line.EndPositionIndexExclusive - 1;
	
	        var fullWidthOfRowIsSelected = true;
	
	        if (lowerPositionIndexInclusive > line.StartPositionIndexInclusive)
	        {
	            selectionStartingColumnIndex = lowerPositionIndexInclusive - line.StartPositionIndexInclusive;
	            fullWidthOfRowIsSelected = false;
	        }
	
	        if (upperPositionIndexExclusive < line.EndPositionIndexExclusive)
	        {
	            selectionEndingColumnIndex = upperPositionIndexExclusive - line.StartPositionIndexInclusive;
	            fullWidthOfRowIsSelected = false;
	        }
	
	        var charMeasurements = RenderBatch.ViewModel.CharAndLineMeasurements;
	
	        var topInPixelsInvariantCulture = (rowIndex * charMeasurements.LineHeight).ToCssValue();
	        var top = $"top: {topInPixelsInvariantCulture}px;";
	
	        var heightInPixelsInvariantCulture = charMeasurements.LineHeight.ToCssValue();
	        var height = $"height: {heightInPixelsInvariantCulture}px;";
	
	        var selectionStartInPixels = selectionStartingColumnIndex * charMeasurements.CharacterWidth;
	
	        // selectionStartInPixels offset from Tab keys a width of many characters
	        {
	            var tabsOnSameRowBeforeCursor = RenderBatch.Model.GetTabCountOnSameLineBeforeCursor(
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
	            var lineInformation = RenderBatch.Model.GetLineInformation(rowIndex);
	
	            selectionEndingColumnIndex = Math.Min(
	                selectionEndingColumnIndex,
	                lineInformation.LastValidColumnIndex);
	
	            var tabsOnSameRowBeforeCursor = RenderBatch.Model.GetTabCountOnSameLineBeforeCursor(
	                rowIndex,
	                selectionEndingColumnIndex);
	
	            // 1 of the character width is already accounted for
	            var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;
	
	            selectionWidthInPixels += extraWidthPerTabKey * tabsOnSameRowBeforeCursor * charMeasurements.CharacterWidth;
	        }
	
	        var widthCssStyleString = "width: ";
	        var fullWidthValue = RenderBatch.ViewModel.ScrollbarDimensions.ScrollWidth;
	
	        if (RenderBatch.ViewModel.TextEditorDimensions.Width >
	            RenderBatch.ViewModel.ScrollbarDimensions.ScrollWidth)
	        {
	            // If content does not fill the viewable width of the Text Editor User Interface
	            fullWidthValue = RenderBatch.ViewModel.TextEditorDimensions.Width;
	        }
	
	        var fullWidthValueInPixelsInvariantCulture = fullWidthValue.ToCssValue();
	
	        var selectionWidthInPixelsInvariantCulture = selectionWidthInPixels.ToCssValue();
	
	        if (fullWidthOfRowIsSelected)
	            widthCssStyleString += $"{fullWidthValueInPixelsInvariantCulture}px;";
	        else if (selectionStartingColumnIndex != 0 &&
	                 upperPositionIndexExclusive > line.EndPositionIndexExclusive - 1)
	            widthCssStyleString += $"calc({fullWidthValueInPixelsInvariantCulture}px - {selectionStartInPixelsInvariantCulture}px);";
	        else
	            widthCssStyleString += $"{selectionWidthInPixelsInvariantCulture}px;";
	
	        return $"{top} {height} {left} {widthCssStyleString}";
		}
		catch (LuthetusTextEditorException e)
		{
			Console.WriteLine(e);
			return "display: none;";
		}
    }

    private (int lowerRowIndexInclusive, int upperRowIndexExclusive) GetSelectionBoundsInRowIndexUnits((int lowerPositionIndexInclusive, int upperPositionIndexExclusive) selectionBoundsInPositionIndexUnits)
    {
        try
        {
            return TextEditorSelectionHelper.ConvertSelectionOfPositionIndexUnitsToRowIndexUnits(
                RenderBatch.Model,
                selectionBoundsInPositionIndexUnits);
        }
        catch (LuthetusTextEditorException)
        {
            return (0, 0);
        }
    }
}