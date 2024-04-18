namespace Luthetus.TextEditor.RazorLib.Rows.Models;

/// <param name="LowerLineEnd">
/// The smaller positionIndex.
/// </param>
/// <param name="UpperLineEnd">
/// The larger positionIndex.
/// </param>
public record LineInformation(
    int Index,
    int StartPositionIndexInclusive,
    int EndPositionIndexExclusive,
    LineEnd LowerLineEnd,
    LineEnd UpperLineEnd);