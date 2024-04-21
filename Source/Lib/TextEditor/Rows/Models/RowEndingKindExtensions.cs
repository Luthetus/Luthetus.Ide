using Luthetus.TextEditor.RazorLib.Exceptions;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.Rows.Models;

public static class RowEndingKindExtensions
{
    /// <summary>
    /// In order to not override the ToString() method in a possibly unexpected way <see cref="AsCharacters" /> was made
    /// to convert a <see cref="LineEndKind" /> to its character(s) representation.
    /// <br /><br />
    /// Example: <see cref="LineEndKind.LineFeed" /> would return '\n'
    /// </summary>
    public static string AsCharacters(this LineEndKind rowEndingKind)
    {
        return rowEndingKind switch
        {
            LineEndKind.CarriageReturn => "\r",
            LineEndKind.LineFeed => "\n",
            LineEndKind.CarriageReturnLineFeed => "\r\n",
            LineEndKind.StartOfFile or LineEndKind.EndOfFile or LineEndKind.Unset => string.Empty,
            _ => throw new LuthetusTextEditorException($"Unexpected {nameof(LineEndKind)} of: {rowEndingKind}"),
        };
    }

    public static string AsCharactersHtmlEscaped(this LineEndKind rowEndingKind)
    {
        return rowEndingKind switch
        {
            LineEndKind.CarriageReturn => "\\r",
            LineEndKind.LineFeed => "\\n",
            LineEndKind.CarriageReturnLineFeed => "\\r\\n",
            LineEndKind.StartOfFile => "SOF",
            LineEndKind.EndOfFile => "EOF",
            _ => throw new LuthetusTextEditorException($"Unexpected {nameof(LineEndKind)} of: {rowEndingKind}"),
        };
    }

    public static string AsFriendlyName(this LineEndKind rowEndingKind)
    {
        return rowEndingKind switch
        {
            LineEndKind.CarriageReturn => "CR",
            LineEndKind.LineFeed => "LF",
            LineEndKind.CarriageReturnLineFeed => "CRLF",
            LineEndKind.Unset => "Unset",
            LineEndKind.StartOfFile => "SOF",
            LineEndKind.EndOfFile => "EOF",
            _ => throw new LuthetusTextEditorException($"Unexpected {nameof(LineEndKind)} of: {rowEndingKind}"),
        };
    }

    public static ImmutableArray<LineEndKind> GetRowEndingsUserAllowedToUse(this LineEndKind rowEndingKind)
    {
        return new[]
        {
            LineEndKind.CarriageReturn,
            LineEndKind.LineFeed,
            LineEndKind.CarriageReturnLineFeed,
        }.ToImmutableArray();
    }
}