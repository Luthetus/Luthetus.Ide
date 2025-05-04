namespace Luthetus.TextEditor.RazorLib.Lines.Models;

/// <param name="LowerLineEnd">
/// The smaller positionIndex.
/// </param>
/// <param name="UpperLineEnd">
/// The larger positionIndex.
/// </param>
public record struct LineInformation(
    int Index,
    int Position_StartInclusiveIndex,
    int Position_EndExclusiveIndex,
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
    public int LastValidColumnIndex => UpperLineEnd.Position_StartInclusiveIndex - LowerLineEnd.Position_EndExclusiveIndex;
}