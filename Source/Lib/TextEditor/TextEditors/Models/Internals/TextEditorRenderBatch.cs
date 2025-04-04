using System.Text;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.TextEditor.RazorLib.Options.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

public class TextEditorRenderBatch
{
	public const string DEFAULT_FONT_FAMILY = "monospace";
	
	public TextEditorRenderBatch(
	    TextEditorModel? model,
	    TextEditorViewModel? viewModel,
	    TextEditorOptions options,
	    string fontFamily,
	    int fontSizeInPixels,
	    ViewModelDisplayOptions viewModelDisplayOptions,
		TextEditorComponentData componentData)
	{
		Model = model;
	    ViewModel = viewModel;
	    Options = options;
	    FontFamily = fontFamily;
	    FontSizeInPixels = fontSizeInPixels;
	    ViewModelDisplayOptions = viewModelDisplayOptions;
		ComponentData = componentData;
	}

	// Don't ??= because it only should be set inside 'Validate()'.
    private double? _gutterWidthInPixels;
    
    public TextEditorModel? Model { get; set; }
    public TextEditorViewModel? ViewModel { get; set; }
    public TextEditorOptions? Options { get; set; }
    public string FontFamily { get; set; }
    public int FontSizeInPixels { get; set; }
    public ViewModelDisplayOptions ViewModelDisplayOptions { get; set; }
	public TextEditorComponentData ComponentData { get; set; }

    public double GutterWidthInPixels => _gutterWidthInPixels ?? GetGutterWidthInPixels();

    public bool IsValid { get; private set; }
        
    public bool Validate()
    {
    	IsValid = Model is not null &&
	        ViewModel is not null &&
	        Options is not null;
	    
	    if (IsValid)
	    {
	    	_gutterWidthInPixels = GutterWidthInPixels;
	    }
	    
	    return IsValid;
    }

    private double GetGutterWidthInPixels()
    {
        if (!ViewModelDisplayOptions.IncludeGutterComponent)
            return 0;

        var mostDigitsInARowLineNumber = Model!.LineCount.ToString().Length;

        var gutterWidthInPixels = mostDigitsInARowLineNumber *
            ViewModel!.CharAndLineMeasurements.CharacterWidth;

        gutterWidthInPixels += TextEditorModel.GUTTER_PADDING_LEFT_IN_PIXELS + TextEditorModel.GUTTER_PADDING_RIGHT_IN_PIXELS;

        return gutterWidthInPixels;
    }
}
