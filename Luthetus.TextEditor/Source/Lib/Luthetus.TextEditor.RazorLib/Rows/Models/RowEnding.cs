namespace Luthetus.TextEditor.RazorLib.Rows.Models;

public record RowEnding
{
    public RowEnding(
        int startPositionIndexInclusive,
        int endPositionIndexExclusive,
        RowEndingKind rowEndingKind)
    {
        StartPositionIndexInclusive = startPositionIndexInclusive;
        EndPositionIndexExclusive = endPositionIndexExclusive;
        RowEndingKind = rowEndingKind;
    }

    /// <summary>
    /// Given: "Hello World!\nAbc123"<br/>
    /// Then: \n starts inclusively at position index 12
    /// </summary>
    public int StartPositionIndexInclusive { get; set; }
    /// <summary>
    /// Given: "Hello World!\nAbc123"<br/>
    /// Then: \n ends exclusively at position index 13
    /// </summary>
    public int EndPositionIndexExclusive { get; set; }
    /// <summary>
    /// Given: "Hello World!\nAbc123"<br/>
    /// Then: \n is <see cref="RowEndingKind.Linefeed"/>
    /// </summary>
    public RowEndingKind RowEndingKind { get; set; }
}
