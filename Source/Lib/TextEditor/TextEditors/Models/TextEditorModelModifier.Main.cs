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
public sealed partial class TextEditorModelModifier : ITextEditorModel
{
    /// <summary>
    /// <see cref="__SplitIntoTwoPartitions(int)"/> will divide by 2 and give the first split the remainder,
    /// then add 1 to the first split if there is a multibyte scenario.
    /// Therefore partition size of 3 would infinitely try to split itself.
    /// </summary>
    public const int MINIMUM_PARTITION_SIZE = 4;

    private readonly TextEditorModel _textEditorModel;

    public TextEditorModelModifier(TextEditorModel model)
    {
        if (model.PartitionSize < MINIMUM_PARTITION_SIZE)
            throw new LuthetusTextEditorException($"{nameof(model)}.{nameof(PartitionSize)} must be >= {MINIMUM_PARTITION_SIZE}");

        PartitionSize = model.PartitionSize;
        WasDirty = model.IsDirty;

        _isDirty = model.IsDirty;

        _textEditorModel = model;
        _partitionList = _textEditorModel.PartitionList;
        _richCharacterList = _textEditorModel.RichCharacterList;
    }
    
    private bool _partitionListChanged;

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
    
    private bool _partitionListIsShallowCopy = false;

    public List<ITextEditorEdit> EditBlockList => _editBlocksList is null ? _textEditorModel.EditBlockList : _editBlocksList;
    public List<LineEnd> LineEndList => _lineEndList is null ? _textEditorModel.LineEndList : _lineEndList;
    public List<(LineEndKind lineEndKind, int count)> LineEndKindCountList => _lineEndKindCountList is null ? _textEditorModel.LineEndKindCountList : _lineEndKindCountList;
    public List<TextEditorPresentationModel> PresentationModelList => _presentationModelsList is null ? _textEditorModel.PresentationModelList : _presentationModelsList;
    public List<int> TabKeyPositionList => _tabKeyPositionsList is null ? _textEditorModel.TabKeyPositionList : _tabKeyPositionsList;
    public LineEndKind? OnlyLineEndKind => _onlyLineEndKindWasModified ? _onlyLineEndKind : _textEditorModel.OnlyLineEndKind;
    public LineEndKind LineEndKindPreference => _usingLineEndKind ?? _textEditorModel.LineEndKindPreference;
    public ResourceUri ResourceUri => _resourceUri ?? _textEditorModel.ResourceUri;
    public DateTime ResourceLastWriteTime => _resourceLastWriteTime ?? _textEditorModel.ResourceLastWriteTime;
    public string FileExtension => _fileExtension ?? _textEditorModel.FileExtension;
    public IDecorationMapper DecorationMapper => _decorationMapper ?? _textEditorModel.DecorationMapper;
    public ICompilerService CompilerService => _compilerService ?? _textEditorModel.CompilerService;
    public SaveFileHelper TextEditorSaveFileHelper => _textEditorSaveFileHelper ?? _textEditorModel.TextEditorSaveFileHelper;
    public int EditBlockIndex => _editBlockIndex ?? _textEditorModel.EditBlockIndex;
    public bool IsDirty => _isDirty;
    public (int lineIndex, int lineLength) MostCharactersOnASingleLineTuple => _mostCharactersOnASingleLineTuple ?? _textEditorModel.MostCharactersOnASingleLineTuple;
    public (int lineIndex, int lineLength) PreviousMostCharactersOnASingleLineTuple => _textEditorModel.MostCharactersOnASingleLineTuple;
    public Key<RenderState> RenderStateKey => _renderStateKey ?? _textEditorModel.RenderStateKey;

    public int LineCount => LineEndList.Count;
    public int PreviousLineCount => _textEditorModel.LineEndList.Count;
    public int CharCount => PartitionList.Sum(x => x.Count);

	/// <summary>
	/// The <see cref="TextEditorEditOther"/> works by invoking 'Open' then when finished invoking 'Close'.
	/// </summary>
	public Stack<TextEditorEditOther> OtherEditStack { get; } = new();

    private List<ITextEditorEdit>? _editBlocksList;
    private List<LineEnd>? _lineEndList;
    private List<(LineEndKind lineEndKind, int count)>? _lineEndKindCountList;
    private List<TextEditorPresentationModel>? _presentationModelsList;
    private List<int>? _tabKeyPositionsList;

    private LineEndKind? _onlyLineEndKind;
    /// <summary>
    /// Awkward special case here: <see cref="_onlyLineEndKind"/> is allowed to be null.
    /// So, the design of this class where null means unmodified, doesn't work well here.
    /// </summary>
    private bool _onlyLineEndKindWasModified;

    private LineEndKind? _usingLineEndKind;
    private ResourceUri? _resourceUri;
    private DateTime? _resourceLastWriteTime;
    private string? _fileExtension;
    private IDecorationMapper? _decorationMapper;
    private ICompilerService? _compilerService;
    private SaveFileHelper? _textEditorSaveFileHelper;
    private int? _editBlockIndex;
    private bool _isDirty;
    private (int rowIndex, int rowLength)? _mostCharactersOnASingleLineTuple;
    private Key<RenderState>? _renderStateKey = Key<RenderState>.NewKey();
    private string? _allText;

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

    public string AllText => _allText ??= new string(RichCharacterList.Select(x => x.Value).ToArray());

    public TextEditorModel ToModel()
    {
        return new TextEditorModel(
            AllText,
            RichCharacterList,
            PartitionSize,
            PartitionList,
            _editBlocksList is null ? _textEditorModel.EditBlockList : _editBlocksList,
            _lineEndList is null ? _textEditorModel.LineEndList : _lineEndList,
            _lineEndKindCountList is null ? _textEditorModel.LineEndKindCountList : _lineEndKindCountList,
            _presentationModelsList is null ? _textEditorModel.PresentationModelList : _presentationModelsList,
            _tabKeyPositionsList is null ? _textEditorModel.TabKeyPositionList : _tabKeyPositionsList,
            _onlyLineEndKindWasModified ? _onlyLineEndKind : _textEditorModel.OnlyLineEndKind,
            _usingLineEndKind ?? _textEditorModel.LineEndKindPreference,
            _resourceUri ?? _textEditorModel.ResourceUri,
            _resourceLastWriteTime ?? _textEditorModel.ResourceLastWriteTime,
            _fileExtension ?? _textEditorModel.FileExtension,
            _decorationMapper ?? _textEditorModel.DecorationMapper,
            _compilerService ?? _textEditorModel.CompilerService,
            _textEditorSaveFileHelper ?? _textEditorModel.TextEditorSaveFileHelper,
            _editBlockIndex ?? _textEditorModel.EditBlockIndex,
            IsDirty,
            _mostCharactersOnASingleLineTuple ?? _textEditorModel.MostCharactersOnASingleLineTuple,
            _renderStateKey ?? _textEditorModel.RenderStateKey);
    }

    public enum DeleteKind
    {
        Backspace,
        Delete,
    }
}