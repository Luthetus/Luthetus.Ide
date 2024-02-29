using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;

public partial class TextEditorModel : ITextEditorModel
{
    // (2024-02-29) Plan to add text editor partitioning #Step 100:
    // --------------------------------------------------
    // Change 'ContentList' from 'List<RichCharacter>?' to 'List<List<RichCharacter>>?
    IList<RichCharacter> ITextEditorModel.ContentList => ContentList;
	IList<EditBlock> ITextEditorModel.EditBlocksList => EditBlocksList;
	IList<RowEnding> ITextEditorModel.RowEndingPositionsList => RowEndingPositionsList;
	IList<(RowEndingKind rowEndingKind, int count)> ITextEditorModel.RowEndingKindCountsList => RowEndingKindCountsList;
	IList<TextEditorPresentationModel> ITextEditorModel.PresentationModelsList => PresentationModelsList;
	IList<int> ITextEditorModel.TabKeyPositionsList => TabKeyPositionsList;

    public int RowCount => RowEndingPositionsList.Count;
    public int DocumentLength => ContentList.Count;
}