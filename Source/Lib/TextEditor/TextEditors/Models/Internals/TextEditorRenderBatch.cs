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
	    
	    Validate();
	}

    public TextEditorModel? Model { get; set; }
    public TextEditorViewModel? ViewModel { get; set; }
    public TextEditorRenderBatchConstants TextEditorRenderBatchConstants { get; set; }
    
    public bool IsValid { get; private set; }
        
    public bool Validate()
    {
    	IsValid = Model is not null &&
			      ViewModel is not null &&
			      TextEditorRenderBatchConstants.TextEditorOptions is not null;
	    
	    return IsValid;
    }
}
