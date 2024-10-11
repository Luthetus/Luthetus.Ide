using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public class PresentationAndSelectionDriver
{
	public readonly TextEditorViewModelDisplay _root;

	public PresentationAndSelectionDriver(TextEditorViewModelDisplay textEditorViewModelDisplay)
	{
		_root = textEditorViewModelDisplay;
	}

	// Odd public but am middle of thinking
	public TextEditorRenderBatchValidated _renderBatch;

	public RenderFragment GetRenderFragment(TextEditorRenderBatchValidated renderBatch)
	{
		// Dangerous state can change mid run possible?
		_renderBatch = renderBatch;
		return PresentationAndSelectionStaticRenderFragments.GetRenderFragment(this);
	}
    
    public List<TextEditorPresentationModel> GetTextEditorPresentationModels(
    	ImmutableList<Key<TextEditorPresentationModel>> textEditorPresentationKeys)
    {
    	var renderBatchLocal = _renderBatch;
    	if (renderBatchLocal is null)
    		return new();
    	
        var textEditorPresentationModelList = new List<TextEditorPresentationModel>();

        foreach (var presentationKey in textEditorPresentationKeys)
        {
            var textEditorPresentationModel = renderBatchLocal.Model.PresentationModelList.FirstOrDefault(x =>
                x.TextEditorPresentationKey == presentationKey);

            if (textEditorPresentationModel is not null)
                textEditorPresentationModelList.Add(textEditorPresentationModel);
        }

        return textEditorPresentationModelList;
    }

    public string PresentationGetCssStyleString(
        int lowerPositionIndexInclusive,
        int upperPositionIndexExclusive,
        int rowIndex)
    {
    	var renderBatchLocal = _renderBatch;
    	if (renderBatchLocal is null)
    		return string.Empty;
    	
        try
        {
            var charMeasurements = renderBatchLocal.ViewModel.CharAndLineMeasurements;
			var textEditorDimensions = renderBatchLocal.ViewModel.TextEditorDimensions;
            var scrollbarDimensions = renderBatchLocal.ViewModel.ScrollbarDimensions;

            if (rowIndex >= renderBatchLocal.Model.LineEndList.Count)
                return string.Empty;

            var line = renderBatchLocal.Model.GetLineInformation(rowIndex);

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
                var tabsOnSameRowBeforeCursor = renderBatchLocal.Model.GetTabCountOnSameLineBeforeCursor(
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
                var tabsOnSameRowBeforeCursor = renderBatchLocal.Model.GetTabCountOnSameLineBeforeCursor(
                    rowIndex,
                    line.LastValidColumnIndex);

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

    public string PresentationGetCssClass(TextEditorPresentationModel presentationModel, byte decorationByte)
    {
        return presentationModel.DecorationMapper.Map(decorationByte);
    }

    public ImmutableArray<TextEditorTextSpan> PresentationVirtualizeAndShiftTextSpans(
        TextEditorTextModification[] textModifications,
        ImmutableArray<TextEditorTextSpan> inTextSpanList)
    {
    	var renderBatchLocal = _renderBatch;
    	if (renderBatchLocal is null)
    		return ImmutableArray<TextEditorTextSpan>.Empty;
    	
        try
        {
            // Virtualize the text spans
            var virtualizedTextSpanList = new List<TextEditorTextSpan>();
            if (renderBatchLocal.ViewModel.VirtualizationResult?.EntryList.Any() ?? false)
            {
                var lowerLineIndexInclusive = renderBatchLocal.ViewModel.VirtualizationResult.EntryList.First().Index;
                var upperLineIndexInclusive = renderBatchLocal.ViewModel.VirtualizationResult.EntryList.Last().Index;

                var lowerLine = renderBatchLocal.Model.GetLineInformation(lowerLineIndexInclusive);
                var upperLine = renderBatchLocal.Model.GetLineInformation(upperLineIndexInclusive);

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

    public (int FirstRowToSelectDataInclusive, int LastRowToSelectDataExclusive) PresentationGetBoundsInRowIndexUnits(TextEditorModel model, (int StartingIndexInclusive, int EndingIndexExclusive) boundsInPositionIndexUnits)
    {
    	var renderBatchLocal = _renderBatch;
    	if (renderBatchLocal is null)
    		return (0, 0);
    	
        try
        {
            var firstRowToSelectDataInclusive = renderBatchLocal.Model
                .GetLineInformationFromPositionIndex(boundsInPositionIndexUnits.StartingIndexInclusive)
                .Index;

            var lastRowToSelectDataExclusive = renderBatchLocal.Model
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
    
    public string GetTextSelectionStyleCss(
        int lowerPositionIndexInclusive,
        int upperPositionIndexExclusive,
        int rowIndex)
    {
    	var renderBatchLocal = _renderBatch;
    	if (renderBatchLocal is null)
    		return "display: none;";
    	
		try
		{
	        if (rowIndex >= renderBatchLocal.Model.LineEndList.Count)
	            return string.Empty;
	
	        var line = renderBatchLocal.Model.GetLineInformation(rowIndex);
	
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
	
	        var charMeasurements = renderBatchLocal.ViewModel.CharAndLineMeasurements;
	
	        var topInPixelsInvariantCulture = (rowIndex * charMeasurements.LineHeight).ToCssValue();
	        var top = $"top: {topInPixelsInvariantCulture}px;";
	
	        var heightInPixelsInvariantCulture = charMeasurements.LineHeight.ToCssValue();
	        var height = $"height: {heightInPixelsInvariantCulture}px;";
	
	        var selectionStartInPixels = selectionStartingColumnIndex * charMeasurements.CharacterWidth;
	
	        // selectionStartInPixels offset from Tab keys a width of many characters
	        {
	            var tabsOnSameRowBeforeCursor = renderBatchLocal.Model.GetTabCountOnSameLineBeforeCursor(
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
	            var lineInformation = renderBatchLocal.Model.GetLineInformation(rowIndex);
	
	            selectionEndingColumnIndex = Math.Min(
	                selectionEndingColumnIndex,
	                lineInformation.LastValidColumnIndex);
	
	            var tabsOnSameRowBeforeCursor = renderBatchLocal.Model.GetTabCountOnSameLineBeforeCursor(
	                rowIndex,
	                selectionEndingColumnIndex);
	
	            // 1 of the character width is already accounted for
	            var extraWidthPerTabKey = TextEditorModel.TAB_WIDTH - 1;
	
	            selectionWidthInPixels += extraWidthPerTabKey * tabsOnSameRowBeforeCursor * charMeasurements.CharacterWidth;
	        }
	
	        var widthCssStyleString = "width: ";
	        var fullWidthValue = renderBatchLocal.ViewModel.ScrollbarDimensions.ScrollWidth;
	
	        if (renderBatchLocal.ViewModel.TextEditorDimensions.Width >
	            renderBatchLocal.ViewModel.ScrollbarDimensions.ScrollWidth)
	        {
	            // If content does not fill the viewable width of the Text Editor User Interface
	            fullWidthValue = renderBatchLocal.ViewModel.TextEditorDimensions.Width;
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

    public (int lowerRowIndexInclusive, int upperRowIndexExclusive) GetSelectionBoundsInRowIndexUnits((int lowerPositionIndexInclusive, int upperPositionIndexExclusive) selectionBoundsInPositionIndexUnits)
    {
    	var renderBatchLocal = _renderBatch;
    	if (renderBatchLocal is null)
    		return (0, 0);
    	
        try
        {
            return TextEditorSelectionHelper.ConvertSelectionOfPositionIndexUnitsToRowIndexUnits(
                renderBatchLocal.Model,
                selectionBoundsInPositionIndexUnits);
        }
        catch (LuthetusTextEditorException)
        {
            return (0, 0);
        }
    }
}
