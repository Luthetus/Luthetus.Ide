using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.Options.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;

public interface ITextEditorModel
{
	public IList<RichCharacter> ContentBag { get; }
	public IList<EditBlock> EditBlocksBag { get; }
	public IList<(int positionIndex, RowEndingKind rowEndingKind)> RowEndingPositionsBag { get; }
	public IList<(RowEndingKind rowEndingKind, int count)> RowEndingKindCountsBag { get; }
	public IList<TextEditorPresentationModel> PresentationModelsBag { get; }
	public IList<int> TabKeyPositionsBag { get; }
	public RowEndingKind? OnlyRowEndingKind { get; }
	public RowEndingKind UsingRowEndingKind { get; }
	public ResourceUri ResourceUri { get; }
	public DateTime ResourceLastWriteTime { get; }
	public string FileExtension { get; }
	public IDecorationMapper DecorationMapper { get; }
	public ICompilerService CompilerService { get; }
	public TextEditorSaveFileHelper TextEditorSaveFileHelper { get; }
	public int EditBlockIndex { get; }
	public (int rowIndex, int rowLength) MostCharactersOnASingleRowTuple { get; }
	public Key<RenderState> RenderStateKey { get; }
	public Keymap TextEditorKeymap { get; }
	public TextEditorOptions? TextEditorOptions { get; }
}
