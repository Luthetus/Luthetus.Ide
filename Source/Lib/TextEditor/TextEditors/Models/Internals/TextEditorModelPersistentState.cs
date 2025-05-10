using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Lines.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

/// <summary>
/// This type reduces the amount of properties that need to be copied from one TextEditorModel instance to another
/// by chosing to have some of the state shared between instances.
/// </summary>
public class TextEditorModelPersistentState
{
	public TextEditorModelPersistentState(
		ITextEditorService textEditorService,
		List<Key<TextEditorViewModel>> viewModelKeyList,
		string fileExtension,
	    IDecorationMapper decorationMapper,
	    ICompilerService compilerService,
	    SaveFileHelper textEditorSaveFileHelper,
	    int partitionSize,
	    ResourceUri resourceUri,
	    int editBlockIndex,
    	List<TextEditorEdit> editBlockList)
	{
		__LocalLineEndList = textEditorService.__LocalLineEndList;
        __LocalTabPositionList = textEditorService.__LocalTabPositionList;
        __TextEditorViewModelLiason = textEditorService.__TextEditorViewModelLiason;
        
        ViewModelKeyList = viewModelKeyList;
        
        FileExtension = fileExtension;
	    DecorationMapper = decorationMapper;
	    CompilerService = compilerService;
	    TextEditorSaveFileHelper = textEditorSaveFileHelper;
	    PartitionSize = partitionSize;
	    ResourceUri = resourceUri;
	    
	    EditBlockIndex = editBlockIndex;
    	EditBlockList = editBlockList;
	}

	/// <summary>
	/// Do not touch this property, it is used for the 'TextEditorModel.InsertMetadata(...)' method.
	/// </summary>
    public List<LineEnd> __LocalLineEndList { get; }
    /// <summary>
	/// Do not touch this property, it is used for the 'TextEditorModel.InsertMetadata(...)' method.
	/// </summary>
    public List<int> __LocalTabPositionList { get; }
    /// <summary>
	/// Do not touch this property, it is used for the 'TextEditorModel.InsertMetadata(...)' method.
	/// </summary>
    public TextEditorViewModelLiason __TextEditorViewModelLiason { get; }
    public List<Key<TextEditorViewModel>> ViewModelKeyList { get; set; }
    
    public string FileExtension { get; set; }
    public IDecorationMapper DecorationMapper { get; set; }
    public ICompilerService CompilerService { get; set; }
    public SaveFileHelper TextEditorSaveFileHelper { get; set; }
    public int PartitionSize { get; }
    public ResourceUri ResourceUri { get; set; }
    
    public int EditBlockIndex { get; set; }
    public List<TextEditorEdit> EditBlockList { get; set; }
}
