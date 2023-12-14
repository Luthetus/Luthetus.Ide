using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;

public partial class TextEditorModelModifier : ITextEditorModel
{
	public IList<RichCharacter> ContentBag => _contentBag is null ? _textEditorModel.ContentBag : _contentBag;
	public IList<EditBlock> EditBlocksBag => _editBlocksBag is null ? _textEditorModel.EditBlocksBag : _editBlocksBag;
	public IList<(int positionIndex, RowEndingKind rowEndingKind)> RowEndingPositionsBag => _rowEndingPositionsBag is null ? _textEditorModel.RowEndingPositionsBag : _rowEndingPositionsBag;
	public IList<(RowEndingKind rowEndingKind, int count)> RowEndingKindCountsBag => _rowEndingKindCountsBag is null ? _textEditorModel.RowEndingKindCountsBag : _rowEndingKindCountsBag;
	public IList<TextEditorPresentationModel> PresentationModelsBag => _presentationModelsBag is null ? _textEditorModel.PresentationModelsBag : _presentationModelsBag;
	public IList<int> TabKeyPositionsBag => _tabKeyPositionsBag is null ? _textEditorModel.TabKeyPositionsBag : _tabKeyPositionsBag;
	public RowEndingKind? OnlyRowEndingKind => _onlyRowEndingKindWasModified ? _onlyRowEndingKind : _textEditorModel.OnlyRowEndingKind;
	public RowEndingKind UsingRowEndingKind => _usingRowEndingKind ?? _textEditorModel.UsingRowEndingKind;
	public ResourceUri ResourceUri => _resourceUri ?? _textEditorModel.ResourceUri;
	public DateTime ResourceLastWriteTime => _resourceLastWriteTime ?? _textEditorModel.ResourceLastWriteTime;
	public string FileExtension => _fileExtension ?? _textEditorModel.FileExtension;
	public IDecorationMapper DecorationMapper => _decorationMapper ?? _textEditorModel.DecorationMapper;
	public ICompilerService CompilerService => _compilerService ?? _textEditorModel.CompilerService;
	public TextEditorSaveFileHelper TextEditorSaveFileHelper => _textEditorSaveFileHelper ?? _textEditorModel.TextEditorSaveFileHelper;
	public int EditBlockIndex => _editBlockIndex ?? _textEditorModel.EditBlockIndex;
	public (int rowIndex, int rowLength) MostCharactersOnASingleRowTuple => _mostCharactersOnASingleRowTuple ?? _textEditorModel.MostCharactersOnASingleRowTuple;
	public Key<RenderState> RenderStateKey => _renderStateKey ?? _textEditorModel.RenderStateKey;
}