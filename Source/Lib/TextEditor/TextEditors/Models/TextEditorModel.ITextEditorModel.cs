using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Edits.Models;
using Luthetus.TextEditor.RazorLib.Rows.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public partial class TextEditorModel : ITextEditorModel
{
    RichCharacter[] ITextEditorModel.RichCharacterList => RichCharacterList;

    ImmutableList<TextEditorPartition> ITextEditorModel.PartitionList => PartitionList;
    
    IList<ITextEditorEdit> ITextEditorModel.EditBlockList => EditBlockList;
	IList<LineEnd> ITextEditorModel.LineEndList => LineEndList;
	IList<(LineEndKind lineEndKind, int count)> ITextEditorModel.LineEndKindCountList => LineEndKindCountList;
	IList<TextEditorPresentationModel> ITextEditorModel.PresentationModelList => PresentationModelList;
	IList<int> ITextEditorModel.TabKeyPositionList => TabKeyPositionList;

    int ITextEditorModel.LineCount => LineCount;
    int ITextEditorModel.CharCount => DocumentLength;
}