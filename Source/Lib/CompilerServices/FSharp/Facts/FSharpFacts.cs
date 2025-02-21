using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;

namespace Luthetus.CompilerServices.FSharp.Facts;

public static class FSharpFacts
{
    public const char STRING_STARTING_CHARACTER = '"';
    public const char STRING_ENDING_CHARACTER = '"';

    public const string COMMENT_SINGLE_LINE_START = "//";
    public const string COMMENT_MULTI_LINE_START = "(*";

    public static readonly IReadOnlyList<char> COMMENT_SINGLE_LINE_ENDINGS = new List<char>
    {
        WhitespaceFacts.CARRIAGE_RETURN,
        WhitespaceFacts.LINE_FEED,
    };

    public const string COMMENT_MULTI_LINE_END = "*)";
}