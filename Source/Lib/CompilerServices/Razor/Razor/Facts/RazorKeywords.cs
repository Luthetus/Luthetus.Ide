using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Razor.Razor.Facts;

public static class RazorKeywords
{
    public const string PAGE_KEYWORD = "page";
    public const string NAMESPACE_KEYWORD = "namespace";
    public const string FUNCTIONS_KEYWORD = "functions";
    public const string CODE_KEYWORD = "code";
    public const string INHERITS_KEYWORD = "inherits";
    public const string MODEL_KEYWORD = "model";
    public const string SECTION_KEYWORD = "section";
    public const string HELPER_KEYWORD = "helper";

    public static readonly ImmutableArray<string> ALL = new[]
    {
        PAGE_KEYWORD,
        NAMESPACE_KEYWORD,
        FUNCTIONS_KEYWORD,
        CODE_KEYWORD,
        INHERITS_KEYWORD,
        MODEL_KEYWORD,
        SECTION_KEYWORD,
        HELPER_KEYWORD,
    }.ToImmutableArray();
}