using Luthetus.TextEditor.RazorLib.CompilerServices;

namespace Luthetus.CompilerServices.JavaScript.Facts;

public static class JavaScriptFacts
{
    public const char STRING_STARTING_CHARACTER = '"';
    public const char STRING_ENDING_CHARACTER = '"';

    public const string COMMENT_SINGLE_LINE_START = "//";
    public const string COMMENT_MULTI_LINE_START = "/*";

    public static readonly IReadOnlyList<char> COMMENT_SINGLE_LINE_ENDINGS = new List<char>
    {
        WhitespaceFacts.CARRIAGE_RETURN,
        WhitespaceFacts.LINE_FEED,
    };

    public const string COMMENT_MULTI_LINE_END = "*/";
}