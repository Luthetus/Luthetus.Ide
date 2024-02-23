using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.JavaScript.JavaScript.Facts;

public static class JavaScriptKeywords
{
    public const string AWAIT_KEYWORD = "await";
    public const string BREAK_KEYWORD = "break";
    public const string CASE_KEYWORD = "case";
    public const string CATCH_KEYWORD = "catch";
    public const string CLASS_KEYWORD = "class";
    public const string CONST_KEYWORD = "const";
    public const string CONTINUE_KEYWORD = "continue";
    public const string DEBUGGER_KEYWORD = "debugger";
    public const string DEFAULT_KEYWORD = "default";
    public const string DELETE_KEYWORD = "delete";
    public const string DO_KEYWORD = "do";
    public const string ELSE_KEYWORD = "else";
    public const string ENUM_KEYWORD = "enum";
    public const string EXPORT_KEYWORD = "export";
    public const string EXTENDS_KEYWORD = "extends";
    public const string FALSE_KEYWORD = "false";
    public const string FINALLY_KEYWORD = "finally";
    public const string FOR_KEYWORD = "for";
    public const string FUNCTION_KEYWORD = "function";
    public const string IF_KEYWORD = "if";
    public const string IMPLEMENTS_KEYWORD = "implements";
    public const string IMPORT_KEYWORD = "import";
    public const string IN_KEYWORD = "in";
    public const string INSTANCEOF_KEYWORD = "instanceof";
    public const string INTERFACE_KEYWORD = "interface";
    public const string LET_KEYWORD = "let";
    public const string NEW_KEYWORD = "new";
    public const string NULL_KEYWORD = "null";
    public const string PACKAGE_KEYWORD = "package";
    public const string PRIVATE_KEYWORD = "private";
    public const string PROTECTED_KEYWORD = "protected";
    public const string PUBLIC_KEYWORD = "public";
    public const string RETURN_KEYWORD = "return";
    public const string SUPER_KEYWORD = "super";
    public const string SWITCH_KEYWORD = "switch";
    public const string STATIC_KEYWORD = "static";
    public const string THIS_KEYWORD = "this";
    public const string THROW_KEYWORD = "throw";
    public const string TRY_KEYWORD = "try";
    public const string TRUE_KEYWORD = "True";
    public const string TYPEOF_KEYWORD = "typeof";
    public const string VAR_KEYWORD = "var";
    public const string VOID_KEYWORD = "void";
    public const string WHILE_KEYWORD = "while";
    public const string WITH_KEYWORD = "with";
    public const string YIELD_KEYWORD = "yield";

    public static readonly ImmutableArray<string> ALL = new[]
    {
        AWAIT_KEYWORD,
        BREAK_KEYWORD,
        CASE_KEYWORD,
        CATCH_KEYWORD,
        CLASS_KEYWORD,
        CONST_KEYWORD,
        CONTINUE_KEYWORD,
        DEBUGGER_KEYWORD,
        DEFAULT_KEYWORD,
        DELETE_KEYWORD,
        DO_KEYWORD,
        ELSE_KEYWORD,
        ENUM_KEYWORD,
        EXPORT_KEYWORD,
        EXTENDS_KEYWORD,
        FALSE_KEYWORD,
        FINALLY_KEYWORD,
        FOR_KEYWORD,
        FUNCTION_KEYWORD,
        IF_KEYWORD,
        IMPLEMENTS_KEYWORD,
        IMPORT_KEYWORD,
        IN_KEYWORD,
        INSTANCEOF_KEYWORD,
        INTERFACE_KEYWORD,
        LET_KEYWORD,
        NEW_KEYWORD,
        NULL_KEYWORD,
        PACKAGE_KEYWORD,
        PRIVATE_KEYWORD,
        PROTECTED_KEYWORD,
        PUBLIC_KEYWORD,
        RETURN_KEYWORD,
        SUPER_KEYWORD,
        SWITCH_KEYWORD,
        STATIC_KEYWORD,
        THIS_KEYWORD,
        THROW_KEYWORD,
        TRY_KEYWORD,
        TRUE_KEYWORD,
        TYPEOF_KEYWORD,
        VAR_KEYWORD,
        VOID_KEYWORD,
        WHILE_KEYWORD,
        WITH_KEYWORD,
        YIELD_KEYWORD,
    }.ToImmutableArray();
}