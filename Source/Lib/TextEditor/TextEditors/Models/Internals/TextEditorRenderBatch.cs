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
	    TextEditorRenderBatchPersistentState textEditorRenderBatchPersistentState)
	{
		Model = model;
	    ViewModel = viewModel;
	    TextEditorRenderBatchPersistentState = textEditorRenderBatchPersistentState;
	    
	    Validate();
	}

    public TextEditorModel? Model { get; set; }
    public TextEditorViewModel? ViewModel { get; set; }
    public TextEditorRenderBatchPersistentState TextEditorRenderBatchPersistentState { get; set; }
    
    public bool IsValid { get; private set; }
        
    public bool Validate()
    {
    	IsValid = Model is not null &&
			      ViewModel is not null &&
			      TextEditorRenderBatchPersistentState.TextEditorOptions is not null;
	    
	    return IsValid;
    }
}
