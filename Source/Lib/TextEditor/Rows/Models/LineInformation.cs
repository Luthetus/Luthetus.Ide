namespace Luthetus.TextEditor.RazorLib.Rows.Models;

public record LineInformation(
    int Index,
    int StartPositionIndexInclusive,
    int EndPositionIndexExclusive,
    LineEnd LowerLineEnd,
    LineEnd UpperLineEnd);