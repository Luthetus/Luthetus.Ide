using Luthetus.TextEditor.RazorLib.Exceptions;

namespace Luthetus.TextEditor.RazorLib.Lines.Models;

public static class LineEndKindExtensions
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
    
    /// <summary>
    /// TODO: Check if using an expression bound property that returns 'nameof(LineEndKind.CarriageReturn)'...
    /// ...will return "CarriageReturn", and only be stored statically?
    /// </summary>
    public static string AsEnumName(this LineEndKind rowEndingKind)
    {
        return rowEndingKind switch
        {
            LineEndKind.CarriageReturn => nameof(LineEndKind.CarriageReturn),
            LineEndKind.LineFeed => nameof(LineEndKind.LineFeed),
            LineEndKind.CarriageReturnLineFeed => nameof(LineEndKind.CarriageReturnLineFeed),
            LineEndKind.Unset => nameof(LineEndKind.Unset),
            LineEndKind.StartOfFile => nameof(LineEndKind.StartOfFile),
            LineEndKind.EndOfFile => nameof(LineEndKind.EndOfFile),
            _ => throw new LuthetusTextEditorException($"Unexpected {nameof(LineEndKind)} of: {rowEndingKind}"),
        };
    }

    public static List<LineEndKind> GetRowEndingsUserAllowedToUse(this LineEndKind rowEndingKind)
    {
        return _rowEndingsUserAllowedToUse;
	}

    private static readonly List<LineEndKind> _rowEndingsUserAllowedToUse = new()
    {
		LineEndKind.CarriageReturn,
		LineEndKind.LineFeed,
		LineEndKind.CarriageReturnLineFeed,
	};
}