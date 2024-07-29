using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.TextEditor.RazorLib.Htmls.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public partial class TextSelectionGroup : ComponentBase
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