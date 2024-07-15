namespace Luthetus.CompilerServices.Razor.Razor.Facts;

public static class RazorFacts
{
    public const string TRANSITION_SUBSTRING = "@";
    public const string TRANSITION_SUBSTRING_ESCAPED = "@@";

    /// <summary>
    /// Only valid if follows immediately after <see cref="TRANSITION_SUBSTRING"/>
    /// </summary>
    public const char COMMENT_START = '*';
    /// <summary>
    /// Only valid if is immediately before <see cref="TRANSITION_SUBSTRING"/>
    /// </summary>
    public const char COMMENT_END = '*';

    public const char CODE_BLOCK_START = '{';
    public const char CODE_BLOCK_END = '}';

    public const char EXPLICIT_EXPRESSION_START = '(';
    public const char EXPLICIT_EXPRESSION_END = ')';

    public const char SINGLE_LINE_TEXT_OUTPUT_WITHOUT_ADDING_HTML_ELEMENT = ':';
}