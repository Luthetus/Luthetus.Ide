using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;

public partial class TextEditorModel : ITextEditorModel
{
    IReadOnlyList<RichCharacter> ITextEditorModel.ContentList => ContentList;
    ImmutableList<ImmutableList<RichCharacter>> ITextEditorModel.PartitionList => PartitionList;
	IList<EditBlock> ITextEditorModel.EditBlocksList => EditBlocksList;
	IList<RowEnding> ITextEditorModel.RowEndingPositionsList => RowEndingPositionsList;
	IList<(RowEndingKind rowEndingKind, int count)> ITextEditorModel.RowEndingKindCountsList => RowEndingKindCountsList;
	IList<TextEditorPresentationModel> ITextEditorModel.PresentationModelsList => PresentationModelsList;
	IList<int> ITextEditorModel.TabKeyPositionsList => TabKeyPositionsList;

    public int RowCount => RowEndingPositionsList.Count;
    public int DocumentLength => ContentList.Count;
}