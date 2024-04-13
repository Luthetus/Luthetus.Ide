namespace Luthetus.TextEditor.RazorLib.Rows.Models;

public record RowInformation(
    int LineIndex,
    int LineStartPositionIndexInclusive,
    LineEnd RowEnding);