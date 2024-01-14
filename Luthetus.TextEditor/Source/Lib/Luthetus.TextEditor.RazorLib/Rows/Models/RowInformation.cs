namespace Luthetus.TextEditor.RazorLib.Rows.Models;

public record RowInformation(
    int RowIndex,
    int RowStartPositionIndexInclusive,
    RowEnding RowEnding);