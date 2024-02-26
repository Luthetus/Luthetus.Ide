using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.Partitions.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;

public partial class TextEditorModel : ITextEditorModel
{
    PartitionContainer ITextEditorModel.ContentList => ContentList;
	IList<EditBlock> ITextEditorModel.EditBlocksList => EditBlocksList;
	IList<RowEnding> ITextEditorModel.RowEndingPositionsList => RowEndingPositionsList;
	IList<(RowEndingKind rowEndingKind, int count)> ITextEditorModel.RowEndingKindCountsList => RowEndingKindCountsList;
	IList<TextEditorPresentationModel> ITextEditorModel.PresentationModelsList => PresentationModelsList;
    ImmutableList<int> ITextEditorModel.TabKeyPositionsList => TabKeyPositionsList;

    public int RowCount => RowEndingPositionsList.Count;
    public int DocumentLength => ContentList.GlobalCharacterCount;
}