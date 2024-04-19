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
    LineEnd UpperLineEnd)
{
    /// <summary>
    /// Given: "Abc\r\n", the last valid column index is between "Abc" and "\r\n".
    ///         i.e. column index of 3.<br/>
    ///         
    /// Reason: The last valid column index is the index between the content and the line ending.
    ///         Think of a cursor, rendered in the text editor. Would it be allowed to go "there".
    /// </summary>
    public int LastValidColumnIndex => UpperLineEnd.StartPositionIndexInclusive;
}