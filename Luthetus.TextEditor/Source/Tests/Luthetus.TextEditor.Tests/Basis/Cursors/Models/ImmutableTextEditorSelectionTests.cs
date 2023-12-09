namespace Luthetus.TextEditor.Tests.Basis.Cursors.Models;

public record ImmutableTextEditorSelectionTests(int? AnchorPositionIndex, int EndingPositionIndex)
{
    public ImmutableTextEditorSelection(TextEditorSelection textEditorSelection)
        : this(textEditorSelection.AnchorPositionIndex, textEditorSelection.EndingPositionIndex)
    {
    }
}