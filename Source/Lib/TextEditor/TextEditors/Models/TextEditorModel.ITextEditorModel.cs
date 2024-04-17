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
    
    IList<EditBlock> ITextEditorModel.EditBlockList => EditBlockList;
	IList<LineEnd> ITextEditorModel.LineEndPositionList => LineEndPositionList;
	IList<(LineEndKind lineEndKind, int count)> ITextEditorModel.LineEndKindCountList => LineEndKindCountList;
	IList<TextEditorPresentationModel> ITextEditorModel.PresentationModelList => PresentationModelList;
	IList<int> ITextEditorModel.TabKeyPositionList => TabKeyPositionList;

    int ITextEditorModel.LineCount => LineCount;
    int ITextEditorModel.DocumentLength => DocumentLength;
}