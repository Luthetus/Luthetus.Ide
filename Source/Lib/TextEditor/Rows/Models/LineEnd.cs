namespace Luthetus.TextEditor.RazorLib.Rows.Models;

public record struct LineEnd
{
    public static readonly LineEnd StartOfFile = new(0, 0, LineEndKind.StartOfFile);

    public LineEnd(
        int startPositionIndexInclusive,
        int endPositionIndexExclusive,
        LineEndKind rowEndingKind)
    {
        StartPositionIndexInclusive = startPositionIndexInclusive;
        EndPositionIndexExclusive = endPositionIndexExclusive;
        LineEndKind = rowEndingKind;
    }

    /// <summary>
    /// Given: "Hello World!\nAbc123"<br/>
    /// Then: \n starts inclusively at position index 12
    /// </summary>
    public int StartPositionIndexInclusive { get; init; }
    /// <summary>
    /// Given: "Hello World!\nAbc123"<br/>
    /// Then: \n ends exclusively at position index 13
    /// </summary>
    public int EndPositionIndexExclusive { get; init; }
    /// <summary>
    /// Given: "Hello World!\nAbc123"<br/>
    /// Then: \n is <see cref="LineEndKind.LineFeed"/>
    /// </summary>
    public LineEndKind LineEndKind { get; init; }
}
