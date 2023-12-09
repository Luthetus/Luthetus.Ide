using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;

namespace Luthetus.TextEditor.Tests.Basis.TextEditors.Models.TextEditorModels;

public partial class TextEditorModelTests
{
	IList<RichCharacter> ITextEditorModel.ContentBag => ContentBag;
	IList<EditBlock> ITextEditorModel.EditBlocksBag => EditBlocksBag;
	IList<(int positionIndex, RowEndingKind rowEndingKind)> ITextEditorModel.RowEndingPositionsBag => RowEndingPositionsBag;
	IList<(RowEndingKind rowEndingKind, int count)> ITextEditorModel.RowEndingKindCountsBag => RowEndingKindCountsBag;
	IList<TextEditorPresentationModel> ITextEditorModel.PresentationModelsBag => PresentationModelsBag;
	IList<int> ITextEditorModel.TabKeyPositionsBag => TabKeyPositionsBag;
}