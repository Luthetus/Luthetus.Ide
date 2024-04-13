using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;

public partial class TextEditorModel : IModelTextEditor
{
    ImmutableList<char> IModelTextEditor.CharList => CharList;
    ImmutableList<byte> IModelTextEditor.DecorationByteList => DecorationByteList;

    ImmutableList<TextEditorPartition> IModelTextEditor.PartitionList => PartitionList;
    
    IList<EditBlock> IModelTextEditor.EditBlockList => EditBlocksList;
	IList<LineEnd> IModelTextEditor.LineEndPositionList => LineEndPositionList;
	IList<(LineEndKind lineEndingKind, int count)> IModelTextEditor.LineEndKindCountsList => LineEndKindCountList;
	IList<TextEditorPresentationModel> IModelTextEditor.PresentationModelsList => PresentationModelList;
	IList<int> IModelTextEditor.TabKeyPositionsList => TabKeyPositionsList;

    int IModelTextEditor.LineCount => LineCount;
    int IModelTextEditor.DocumentLength => DocumentLength;
}