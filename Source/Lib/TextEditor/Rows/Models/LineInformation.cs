namespace Luthetus.TextEditor.RazorLib.Rows.Models;

public record LineInformation(
    int LineIndex,
    int LineStartPositionIndexInclusive,
    LineEnd LineEnd);