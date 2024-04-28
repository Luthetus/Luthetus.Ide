using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.JavaScript.JavaScript.Facts;

public static class JavaScriptFacts
{
    public const char STRING_STARTING_CHARACTER = '"';
    public const char STRING_ENDING_CHARACTER = '"';

    public const string COMMENT_SINGLE_LINE_START = "//";
    public const string COMMENT_MULTI_LINE_START = "/*";

    public static readonly ImmutableArray<char> COMMENT_SINGLE_LINE_ENDINGS = new[]
    {
        WhitespaceFacts.CARRIAGE_RETURN,
        WhitespaceFacts.LINE_FEED,
    }.ToImmutableArray();

    public const string COMMENT_MULTI_LINE_END = "*/";
}