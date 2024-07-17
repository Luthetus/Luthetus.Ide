using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
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
public partial class TextEditorModelModifier : ITextEditorModel
{
    private readonly TextEditorModel _textEditorModel;

    public TextEditorModelModifier(TextEditorModel model)
    {
        if (model.PartitionSize < 2)
            throw new LuthetusTextEditorException($"{nameof(model)}.{nameof(PartitionSize)} must be >= 2");

        PartitionSize = model.PartitionSize;
        WasDirty = model.IsDirty;

        _isDirty = model.IsDirty;

        _textEditorModel = model;
        _partitionList = _textEditorModel.PartitionList;
    }

    public ImmutableList<RichCharacter> RichCharacterList => _richCharacterList is null ? _textEditorModel.RichCharacterList : _richCharacterList;
    public ImmutableList<TextEditorPartition> PartitionList => _partitionList is null ? _textEditorModel.PartitionList : _partitionList;

    public IList<ITextEditorEdit> EditBlockList => _editBlocksList is null ? _textEditorModel.EditBlockList : _editBlocksList;
    public IList<LineEnd> LineEndList => _lineEndList is null ? _textEditorModel.LineEndList : _lineEndList;
    public IList<(LineEndKind lineEndKind, int count)> LineEndKindCountList => _lineEndKindCountList is null ? _textEditorModel.LineEndKindCountList : _lineEndKindCountList;
    public IList<TextEditorPresentationModel> PresentationModelList => _presentationModelsList is null ? _textEditorModel.PresentationModelList : _presentationModelsList;
    public IList<int> TabKeyPositionList => _tabKeyPositionsList is null ? _textEditorModel.TabKeyPositionList : _tabKeyPositionsList;
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
    public Key<RenderState> RenderStateKey => _renderStateKey ?? _textEditorModel.RenderStateKey;

    public int LineCount => LineEndList.Count;
    public int CharCount => _richCharacterList.Count;

	/// <summary>
	/// The <see cref="TextEditorEditOther"/> works by invoking 'Open' then when finished invoking 'Close'.
	/// </summary>
	public Stack<TextEditorEditOther> OtherEditStack { get; } = new();

    /// <summary>
    /// TODO: Awkward naming convention is being used here. This is an expression bound property,...
    ///       ...with the naming conventions of a private field.
    ///       |
    ///       The reason for breaking convention here is that every other purpose of this type is done
    ///       through a private field.
    ///       |
    ///       Need to revisit naming this.
    /// </summary>
    private ImmutableList<RichCharacter> _richCharacterList => _partitionList.SelectMany(x => x.RichCharacterList).ToImmutableList();
    /// <summary>
    /// TODO: Awkward naming convention is being used here. This is an expression bound property,...
    ///       ...with the naming conventions of a private field.
    ///       |
    ///       The reason for breaking convention here is that every other purpose of this type is done
    ///       through a private field.
    ///       |
    ///       Need to revisit naming this.
    /// </summary>
    private ImmutableList<TextEditorPartition> _partitionList = new TextEditorPartition[] { new(Array.Empty<RichCharacter>().ToImmutableList()) }.ToImmutableList();

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
            _richCharacterList is null ? _textEditorModel.RichCharacterList : _richCharacterList,
            PartitionSize,
            _partitionList is null ? _textEditorModel.PartitionList : _partitionList,
            _editBlocksList is null ? _textEditorModel.EditBlockList : _editBlocksList.ToImmutableList(),
            _lineEndList is null ? _textEditorModel.LineEndList : _lineEndList.ToImmutableList(),
            _lineEndKindCountList is null ? _textEditorModel.LineEndKindCountList : _lineEndKindCountList.ToImmutableList(),
            _presentationModelsList is null ? _textEditorModel.PresentationModelList : _presentationModelsList.ToImmutableList(),
            _tabKeyPositionsList is null ? _textEditorModel.TabKeyPositionList : _tabKeyPositionsList.ToImmutableList(),
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