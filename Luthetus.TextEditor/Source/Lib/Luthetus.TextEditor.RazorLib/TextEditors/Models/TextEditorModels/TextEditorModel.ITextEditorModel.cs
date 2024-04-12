using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;

public partial class TextEditorModel : ITextEditorModel
{
    ImmutableList<char> ITextEditorModel.CharList => CharList;
    ImmutableList<byte> ITextEditorModel.DecorationByteList => DecorationByteList;

    ImmutableList<TextEditorPartition> ITextEditorModel.PartitionList => PartitionList;
    
    IList<EditBlock> ITextEditorModel.EditBlockList => EditBlocksList;
	IList<LineEnd> ITextEditorModel.LineEndPositionList => RowEndingPositionsList;
	IList<(RowEndingKind rowEndingKind, int count)> ITextEditorModel.RowEndingKindCountsList => RowEndingKindCountsList;
	IList<TextEditorPresentationModel> ITextEditorModel.PresentationModelsList => PresentationModelsList;
	IList<int> ITextEditorModel.TabKeyPositionsList => TabKeyPositionsList;

    int ITextEditorModel.RowCount => RowCount;
    int ITextEditorModel.DocumentLength => DocumentLength;
}