using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

/// <summary>
/// Any modified state needs to be 'null coallesce assigned' to the existing TextEditorModel's value
///
/// When reading state, if the state had been 'null coallesce assigned' then the field will
/// be read. Otherwise, the existing TextEditorModel's value will be read.
/// <br/><br/>
/// <inheritdoc cref="ITextEditorModel"/>
/// </summary>
public sealed partial class TextEditorModel : ITextEditorModel
{
    /// <summary>
    /// <see cref="__SplitIntoTwoPartitions(int)"/> will divide by 2 and give the first split the remainder,
    /// then add 1 to the first split if there is a multibyte scenario.
    /// Therefore partition size of 3 would infinitely try to split itself.
    /// </summary>
    public const int MINIMUM_PARTITION_SIZE = 4;

	/// <summary>
	/// The first time a model is constructed it will throw an exception when accessing AllText,
	/// therefore pass it in as an argument.
	/// </summary>
    public TextEditorModel(TextEditorModel model, string? allText)
    {
        if (model.PartitionSize < MINIMUM_PARTITION_SIZE)
            throw new LuthetusTextEditorException($"{nameof(model)}.{nameof(PartitionSize)} must be >= {MINIMUM_PARTITION_SIZE}");

        PartitionSize = model.PartitionSize;
        WasDirty = model.IsDirty;

        IsDirty = model.IsDirty;

        _partitionList = model.PartitionList;
        _richCharacterList = model.RichCharacterList;
        
        EditBlockList = model.EditBlockList;
	    LineEndList = model.LineEndList;
	    LineEndKindCountList = model.LineEndKindCountList;
	    PresentationModelList = model.PresentationModelList;
	    TabKeyPositionList = model.TabKeyPositionList;
        
        OnlyLineEndKind = model.OnlyLineEndKind;
	    LineEndKindPreference = model.LineEndKindPreference;
	    ResourceUri = model.ResourceUri;
	    ResourceLastWriteTime = model.ResourceLastWriteTime;
	    FileExtension = model.FileExtension;
	    DecorationMapper = model.DecorationMapper;
	    CompilerService = model.CompilerService;
	    TextEditorSaveFileHelper = model.TextEditorSaveFileHelper;
	    EditBlockIndex = model.EditBlockIndex;
	    IsDirty = model.IsDirty;
	    MostCharactersOnASingleLineTuple = model.MostCharactersOnASingleLineTuple;
	    _allText = allText;
	    RenderStateKey = Key<RenderState>.NewKey();
	    
	    PreviousLineCount = model.LineEndList.Count;
    }

    public RichCharacter[] _richCharacterList;
    public RichCharacter[] RichCharacterList
    {
    	get
    	{
    		if (_partitionListChanged)
    		{
    			_partitionListChanged = false;
    			_richCharacterList = PartitionList.SelectMany(x => x.RichCharacterList).ToArray();
    		}
    		
    		return _richCharacterList;
    	}
    	set
    	{
    		_partitionListChanged = false;
    		_richCharacterList = value;
    	}
    }
    
    private bool _partitionListChanged;
    private bool _partitionListIsShallowCopy = false;
    public List<TextEditorPartition> _partitionList;
    public List<TextEditorPartition> PartitionList
    {
    	get
    	{
    		return _partitionList;
    	}
    	set
    	{
    		_partitionListChanged = true;
    		_partitionList = value;
    	}
    }

    public List<ITextEditorEdit> EditBlockList { get; set; }
    public List<LineEnd> LineEndList { get; set; }
    public List<(LineEndKind lineEndKind, int count)> LineEndKindCountList { get; set; }
    public List<TextEditorPresentationModel> PresentationModelList { get; set; }
    public List<int> TabKeyPositionList { get; set; }
    public LineEndKind OnlyLineEndKind { get; set; }
    public LineEndKind LineEndKindPreference { get; set; }
    public ResourceUri ResourceUri { get; set; }
    public DateTime ResourceLastWriteTime { get; set; }
    public string FileExtension { get; set; }
    public IDecorationMapper DecorationMapper { get; set; }
    public ICompilerService CompilerService { get; set; }
    public SaveFileHelper TextEditorSaveFileHelper { get; set; }
    public int EditBlockIndex { get; set; }
    public bool IsDirty { get; set; }
    public (int lineIndex, int lineLength) MostCharactersOnASingleLineTuple { get; set; }
    public (int lineIndex, int lineLength) PreviousMostCharactersOnASingleLineTuple { get; set; }
    public Key<RenderState> RenderStateKey { get; set; }

    public int LineCount => LineEndList.Count;
    public int PreviousLineCount { get; set; }
    
    // TODO: Remove Linq?
    public int CharCount => PartitionList.Sum(x => x.Count);

	/// <summary>
	/// The <see cref="TextEditorEditOther"/> works by invoking 'Open' then when finished invoking 'Close'.
	/// </summary>
	public Stack<TextEditorEditOther> OtherEditStack { get; } = new();

    /// <summary>
    /// This property optimizes the dirty state tracking. If _wasDirty != _isDirty then track the state change.
    /// This involves writing to dependency injectable state, then triggering a re-render in the <see cref="Edits.Displays.DirtyResourceUriInteractiveIconDisplay"/>
    /// </summary>
    public bool WasDirty { get; }

    private int PartitionSize { get; }

	/// <summary>
	/// This property decides whether or not to replace the existing model in IState<TextEditorState> with
	/// the instance that comes from this modifier.
	/// </summary>
    public bool WasModified { get; internal set; }
	
	/// <summary>
	/// This property decides whether or not to re-calculate the virtualization result that gets displayed on the UI.
	/// </summary>
    public bool ShouldReloadVirtualizationResult { get; internal set; }

	private string _allText;
    public string AllText => _allText ??= new string(RichCharacterList.Select(x => x.Value).ToArray());

    public TextEditorModel ToModel()
    {
        return new TextEditorModel(
            AllText,
            RichCharacterList,
            PartitionSize,
            PartitionList,
            EditBlockList,
            LineEndList,
            LineEndKindCountList,
            PresentationModelList,
            TabKeyPositionList,
            OnlyLineEndKind,
            LineEndKindPreference,
            ResourceUri,
            ResourceLastWriteTime,
            FileExtension,
            DecorationMapper,
            CompilerService,
            TextEditorSaveFileHelper,
            EditBlockIndex,
            IsDirty,
            MostCharactersOnASingleLineTuple,
            RenderStateKey);
    }

    public enum DeleteKind
    {
        Backspace,
        Delete,
    }
}