using System.Text;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.TextEditor.RazorLib.Options.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

public struct TextEditorRenderBatch
{
	public const string DEFAULT_FONT_FAMILY = "monospace";
	
	public TextEditorRenderBatch(
	    TextEditorModel? model,
	    TextEditorViewModel? viewModel,
	    TextEditorRenderBatchConstants textEditorRenderBatchConstants)
	{
		Model = model;
	    ViewModel = viewModel;
	    TextEditorRenderBatchConstants = textEditorRenderBatchConstants;
	}

    public TextEditorModel? Model { get; set; }
    public TextEditorViewModel? ViewModel { get; set; }
    public TextEditorRenderBatchConstants TextEditorRenderBatchConstants { get; set; }
    
    public bool ConstructorWasInvoked => Model is not null &&
								         ViewModel is not null &&
								         TextEditorRenderBatchConstants.TextEditorOptions is not null;

    public double GutterWidthInPixels { get; private set; }

    public bool IsValid { get; private set; }
        
    public bool Validate()
    {
    	IsValid = Model is not null &&
	        ViewModel is not null &&
	        TextEditorRenderBatchConstants.TextEditorOptions is not null;
	    
	    if (IsValid)
	    	GutterWidthInPixels = GetGutterWidthInPixels();
	    
	    return IsValid;
    }
    
    private static int CountDigits(int argumentNumber)
    {
    	var digitCount = 1;
    	var runningNumber = argumentNumber;
    	
    	while ((runningNumber /= 10) > 0)
    	{
    		digitCount++;
    	}
    	
    	return digitCount;
    }

    private double GetGutterWidthInPixels()
    {
        if (!TextEditorRenderBatchConstants.ViewModelDisplayOptions.IncludeGutterComponent)
            return 0;

        var mostDigitsInARowLineNumber = CountDigits(Model!.LineCount);

        var gutterWidthInPixels = mostDigitsInARowLineNumber *
            ViewModel!.CharAndLineMeasurements.CharacterWidth;

        gutterWidthInPixels += TextEditorModel.GUTTER_PADDING_LEFT_IN_PIXELS + TextEditorModel.GUTTER_PADDING_RIGHT_IN_PIXELS;

        return gutterWidthInPixels;
    }
}
