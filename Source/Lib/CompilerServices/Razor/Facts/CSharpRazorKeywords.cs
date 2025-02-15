namespace Luthetus.CompilerServices.Razor.Facts;

public static class CSharpRazorKeywords
{
    public const string CASE_KEYWORD = "case";
    public const string DO_KEYWORD = "do";
    public const string DEFAULT_KEYWORD = "default";
    public const string FOR_KEYWORD = "for";
    public const string FOREACH_KEYWORD = "foreach";
    public const string IF_KEYWORD = "if";
    public const string ELSE_KEYWORD = "else";
    public const string LOCK_KEYWORD = "lock";
    public const string SWITCH_KEYWORD = "switch";
    public const string TRY_KEYWORD = "try";
    public const string CATCH_KEYWORD = "catch";
    public const string FINALLY_KEYWORD = "finally";
    public const string USING_KEYWORD = "using";
    public const string WHILE_KEYWORD = "while";

    public static readonly List<string> ALL = new()
    {
        CASE_KEYWORD,
        DO_KEYWORD,
        DEFAULT_KEYWORD,
        FOREACH_KEYWORD,
        FOR_KEYWORD,
        IF_KEYWORD,
        ELSE_KEYWORD,
        LOCK_KEYWORD,
        SWITCH_KEYWORD,
        TRY_KEYWORD,
        CATCH_KEYWORD,
        FINALLY_KEYWORD,
        USING_KEYWORD,
        WHILE_KEYWORD,
    };
}