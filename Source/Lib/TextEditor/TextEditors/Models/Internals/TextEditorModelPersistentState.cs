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
		TextEditorService textEditorService,
		List<Key<TextEditorViewModel>> viewModelKeyList,
		string fileExtension,
	    IDecorationMapper decorationMapper,
	    ICompilerService compilerService,
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
    public int PartitionSize { get; }
    public ResourceUri ResourceUri { get; set; }
    
    public int EditBlockIndex { get; set; }
    public List<TextEditorEdit> EditBlockList { get; set; }
    
    #region SaveFileHelper
    /*
    // (2025-05-28)
    // I'm not quite sure what the 'SaveFileHelper' is doing. It probably should be removed.
    // But at the very least I think "inlining" the class defintion onto the TextEditorModelPersistentState
    // directly could reduce the overhead of this (by removing an object allocation for the SaveFileHelper itself).
    // ============================================================================================================
    /// <summary>
	/// TODO: (2023-06-09) This class is a hacky way to track a cancellation token source per each text editor model. As of this comment, TextEditorModel is a class which is frequently re-constructed with new(). A CancellationTokenSource as a property would therefore not work. One would Cancel() then new() up another CancellationTokenSource and update the property. Yet its possible that instance of TextEditorModel isn't even being used anymore by that point. So a readonly object wrapper which then contains the CancellationTokenSource ensures it doesn't get lost.
	/// </summary>
	public sealed class SaveFileHelper
	{
	    private object _saveLock = new();
	    private CancellationTokenSource _saveCancellationTokenSource = new();
	
	    public CancellationToken GetCancellationToken()
	    {
	        lock (_saveLock)
	        {
	            _saveCancellationTokenSource.Cancel();
	            _saveCancellationTokenSource = new();
	
	            return _saveCancellationTokenSource.Token;
	        }
	    }
	}
    */
    
	private object _saveLock = new();
    private CancellationTokenSource _saveCancellationTokenSource = new();

    public CancellationToken GetCancellationToken()
    {
        lock (_saveLock)
        {
            _saveCancellationTokenSource.Cancel();
            _saveCancellationTokenSource = new();

            return _saveCancellationTokenSource.Token;
        }
    }
    #endregion
}
