using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;

public partial class TextEditorModelModifier : ITextEditorModel
{
    public ImmutableList<char> CharList => _charList is null ? _textEditorModel.CharList : _charList;
    public ImmutableList<byte> DecorationByteList => _decorationByteList is null ? _textEditorModel.DecorationByteList : _decorationByteList;
	public ImmutableList<TextEditorPartition> PartitionList => _partitionList is null ? _textEditorModel.PartitionList : _partitionList;
	
	public IList<EditBlock> EditBlockList => _editBlocksList is null ? _textEditorModel.EditBlocksList : _editBlocksList;
	public IList<LineEnd> LineEndPositionList => _lineEndingPositionsList is null ? _textEditorModel.LineEndPositionList : _lineEndingPositionsList;
	public IList<(LineEndKind lineEndingKind, int count)> LineEndingKindCountsList => _lineEndingKindCountsList is null ? _textEditorModel.LineEndKindCountList : _lineEndingKindCountsList;
	public IList<TextEditorPresentationModel> PresentationModelsList => _presentationModelsList is null ? _textEditorModel.PresentationModelList : _presentationModelsList;
	public IList<int> TabKeyPositionsList => _tabKeyPositionsList is null ? _textEditorModel.TabKeyPositionsList : _tabKeyPositionsList;
	public LineEndKind? OnlyLineEndKind => _onlyLineEndKindWasModified ? _onlyLineEndKind : _textEditorModel.OnlyLineEndKind;
	public LineEndKind UsingLineEndKind => _usingLineEndKind ?? _textEditorModel.UsingLineEndKind;
	public ResourceUri ResourceUri => _resourceUri ?? _textEditorModel.ResourceUri;
	public DateTime ResourceLastWriteTime => _resourceLastWriteTime ?? _textEditorModel.ResourceLastWriteTime;
	public string FileExtension => _fileExtension ?? _textEditorModel.FileExtension;
	public IDecorationMapper DecorationMapper => _decorationMapper ?? _textEditorModel.DecorationMapper;
	public ILuthCompilerService CompilerService => _compilerService ?? _textEditorModel.CompilerService;
	public TextEditorSaveFileHelper TextEditorSaveFileHelper => _textEditorSaveFileHelper ?? _textEditorModel.TextEditorSaveFileHelper;
	public int EditBlockIndex => _editBlockIndex ?? _textEditorModel.EditBlockIndex;
	public bool IsDirty => _isDirty;
    public (int lineIndex, int lineLength) MostCharactersOnASingleLineTuple => _mostCharactersOnASingleLineTuple ?? _textEditorModel.MostCharactersOnASingleLineTuple;
	public Key<RenderState> RenderStateKey => _renderStateKey ?? _textEditorModel.RenderStateKey;

    public int LineCount => LineEndPositionList.Count;
    public int DocumentLength => _charList.Count;


}