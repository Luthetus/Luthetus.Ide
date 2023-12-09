using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.Rows.Models;

public static class RowEndingKindExtensions
{
    /// <summary>
    /// In order to not override the ToString() method in a possibly unexpected way <see cref="AsCharacters" /> was made
    /// to convert a <see cref="RowEndingKind" /> to its character(s) representation.
    /// <br /><br />
    /// Example: <see cref="RowEndingKind.Linefeed" /> would return '\n'
    /// </summary>
    public static string AsCharacters(this RowEndingKind rowEndingKind)
    {
        return rowEndingKind switch
        {
            RowEndingKind.CarriageReturn => "\r",
            RowEndingKind.Linefeed => "\n",
            RowEndingKind.CarriageReturnLinefeed => "\r\n",
            RowEndingKind.StartOfFile or RowEndingKind.EndOfFile or RowEndingKind.Unset => string.Empty,
            _ => throw new ApplicationException($"Unexpected {nameof(RowEndingKind)} of: {rowEndingKind}"),
        };
    }

    public static string AsCharactersHtmlEscaped(this RowEndingKind rowEndingKind)
    {
        return rowEndingKind switch
        {
            RowEndingKind.CarriageReturn => "\\r",
            RowEndingKind.Linefeed => "\\n",
            RowEndingKind.CarriageReturnLinefeed => "\\r\\n",
            RowEndingKind.StartOfFile => "SOF",
            RowEndingKind.EndOfFile => "EOF",
            _ => throw new ApplicationException($"Unexpected {nameof(RowEndingKind)} of: {rowEndingKind}"),
        };
    }

    public static string AsFriendlyName(this RowEndingKind rowEndingKind)
    {
        return rowEndingKind switch
        {
            RowEndingKind.CarriageReturn => "CR",
            RowEndingKind.Linefeed => "LF",
            RowEndingKind.CarriageReturnLinefeed => "CRLF",
            RowEndingKind.Unset => "Unset",
            RowEndingKind.StartOfFile => "SOF",
            RowEndingKind.EndOfFile => "EOF",
            _ => throw new ApplicationException($"Unexpected {nameof(RowEndingKind)} of: {rowEndingKind}"),
        };
    }

    public static ImmutableArray<RowEndingKind> GetRowEndingsUserAllowedToUse(this RowEndingKind rowEndingKind)
    {
        return new[]
        {
            RowEndingKind.CarriageReturn,
            RowEndingKind.Linefeed,
            RowEndingKind.CarriageReturnLinefeed,
        }.ToImmutableArray();
    }
}