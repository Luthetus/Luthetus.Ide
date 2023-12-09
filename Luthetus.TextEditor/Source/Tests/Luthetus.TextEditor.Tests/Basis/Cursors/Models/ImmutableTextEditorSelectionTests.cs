namespace Luthetus.TextEditor.RazorLib.Cursors.Models;

public record ImmutableTextEditorSelectionTests(int? AnchorPositionIndex, int EndingPositionIndex)
{
    public ImmutableTextEditorSelection(TextEditorSelection textEditorSelection)
        : this(textEditorSelection.AnchorPositionIndex, textEditorSelection.EndingPositionIndex)
    {
    }
}