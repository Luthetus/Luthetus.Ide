using System.Collections.Immutable;

namespace Luthetus.CompilerServices.TypeScript.Facts;

/// <summary>
/// Found the list of keywords for TypeScript used in Monaco:
/// https://github.com/microsoft/monaco-editor/blob/main/src/basic-languages/typescript/typescript.ts
/// <br/><br/>
/// Monaco is making their list of keywords by looking at the TypeScript source code
/// see the:
/// https://github.com/microsoft/TypeScript/blob/master/src/compiler/scanner.ts
/// </summary>
public static class TypeScriptKeywords
{
    public const string ABSTRACT_KEYWORD = "abstract";
    public const string ANY_KEYWORD = "any";
    public const string AS_KEYWORD = "as";
    public const string ASSERTS_KEYWORD = "asserts";
    public const string BIGINT_KEYWORD = "bigint";
    public const string BOOLEAN_KEYWORD = "boolean";
    public const string BREAK_KEYWORD = "break";
    public const string CASE_KEYWORD = "case";
    public const string CATCH_KEYWORD = "catch";
    public const string CLASS_KEYWORD = "class";
    public const string CONTINUE_KEYWORD = "continue";
    public const string CONST_KEYWORD = "const";
    public const string CONSTRUCTOR_KEYWORD = "constructor";
    public const string DEBUGGER_KEYWORD = "debugger";
    public const string DECLARE_KEYWORD = "declare";
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
    public const string FROM_KEYWORD = "from";
    public const string FUNCTION_KEYWORD = "function";
    public const string GET_KEYWORD = "get";
    public const string IF_KEYWORD = "if";
    public const string IMPLEMENTS_KEYWORD = "implements";
    public const string IMPORT_KEYWORD = "import";
    public const string IN_KEYWORD = "in";
    public const string INFER_KEYWORD = "infer";
    public const string INSTANCEOF_KEYWORD = "instanceof";
    public const string INTERFACE_KEYWORD = "interface";
    public const string IS_KEYWORD = "is";
    public const string KEYOF_KEYWORD = "keyof";
    public const string LET_KEYWORD = "let";
    public const string MODULE_KEYWORD = "module";
    public const string NAMESPACE_KEYWORD = "namespace";
    public const string NEVER_KEYWORD = "never";
    public const string NEW_KEYWORD = "new";
    public const string NULL_KEYWORD = "null";
    public const string NUMBER_KEYWORD = "number";
    public const string OBJECT_KEYWORD = "object";
    public const string OUT_KEYWORD = "out";
    public const string PACKAGE_KEYWORD = "package";
    public const string PRIVATE_KEYWORD = "private";
    public const string PROTECTED_KEYWORD = "protected";
    public const string PUBLIC_KEYWORD = "public";
    public const string OVERRIDE_KEYWORD = "override";
    public const string READONLY_KEYWORD = "readonly";
    public const string REQUIRE_KEYWORD = "require";
    public const string GLOBAL_KEYWORD = "global";
    public const string RETURN_KEYWORD = "return";
    public const string SET_KEYWORD = "set";
    public const string STATIC_KEYWORD = "static";
    public const string STRING_KEYWORD = "string";
    public const string SUPER_KEYWORD = "super";
    public const string SWITCH_KEYWORD = "switch";
    public const string SYMBOL_KEYWORD = "symbol";
    public const string THIS_KEYWORD = "this";
    public const string THROW_KEYWORD = "throw";
    public const string TRUE_KEYWORD = "true";
    public const string TRY_KEYWORD = "try";
    public const string TYPE_KEYWORD = "type";
    public const string TYPEOF_KEYWORD = "typeof";
    public const string UNDEFINED_KEYWORD = "undefined";
    public const string UNIQUE_KEYWORD = "unique";
    public const string UNKNOWN_KEYWORD = "unknown";
    public const string VAR_KEYWORD = "var";
    public const string VOID_KEYWORD = "void";
    public const string WHILE_KEYWORD = "while";
    public const string WITH_KEYWORD = "with";
    public const string YIELD_KEYWORD = "yield";
    public const string ASYNC_KEYWORD = "async";
    public const string AWAIT_KEYWORD = "await";
    public const string OF_KEYWORD = "of";

    public static readonly ImmutableArray<string> ALL = new[]
    {
        ABSTRACT_KEYWORD,
        ANY_KEYWORD,
        AS_KEYWORD,
        ASSERTS_KEYWORD,
        BIGINT_KEYWORD,
        BOOLEAN_KEYWORD,
        BREAK_KEYWORD,
        CASE_KEYWORD,
        CATCH_KEYWORD,
        CLASS_KEYWORD,
        CONTINUE_KEYWORD,
        CONST_KEYWORD,
        CONSTRUCTOR_KEYWORD,
        DEBUGGER_KEYWORD,
        DECLARE_KEYWORD,
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
        FROM_KEYWORD,
        FUNCTION_KEYWORD,
        GET_KEYWORD,
        IF_KEYWORD,
        IMPLEMENTS_KEYWORD,
        IMPORT_KEYWORD,
        IN_KEYWORD,
        INFER_KEYWORD,
        INSTANCEOF_KEYWORD,
        INTERFACE_KEYWORD,
        IS_KEYWORD,
        KEYOF_KEYWORD,
        LET_KEYWORD,
        MODULE_KEYWORD,
        NAMESPACE_KEYWORD,
        NEVER_KEYWORD,
        NEW_KEYWORD,
        NULL_KEYWORD,
        NUMBER_KEYWORD,
        OBJECT_KEYWORD,
        OUT_KEYWORD,
        PACKAGE_KEYWORD,
        PRIVATE_KEYWORD,
        PROTECTED_KEYWORD,
        PUBLIC_KEYWORD,
        OVERRIDE_KEYWORD,
        READONLY_KEYWORD,
        REQUIRE_KEYWORD,
        GLOBAL_KEYWORD,
        RETURN_KEYWORD,
        SET_KEYWORD,
        STATIC_KEYWORD,
        STRING_KEYWORD,
        SUPER_KEYWORD,
        SWITCH_KEYWORD,
        SYMBOL_KEYWORD,
        THIS_KEYWORD,
        THROW_KEYWORD,
        TRUE_KEYWORD,
        TRY_KEYWORD,
        TYPE_KEYWORD,
        TYPEOF_KEYWORD,
        UNDEFINED_KEYWORD,
        UNIQUE_KEYWORD,
        UNKNOWN_KEYWORD,
        VAR_KEYWORD,
        VOID_KEYWORD,
        WHILE_KEYWORD,
        WITH_KEYWORD,
        YIELD_KEYWORD,
        ASYNC_KEYWORD,
        AWAIT_KEYWORD,
        OF_KEYWORD,
    }.ToImmutableArray();
}