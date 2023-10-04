using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.FSharp.FSharp.Facts;

public class FSharpKeywords
{
    public const string ABSTRACT_KEYWORD = "abstract";
    public const string AND_KEYWORD = "and";
    public const string AS_KEYWORD = "as";
    public const string ASSERT_KEYWORD = "assert";
    public const string BASE_KEYWORD = "base";
    public const string BEGIN_KEYWORD = "begin";
    public const string CLASS_KEYWORD = "class";
    public const string DEFAULT_KEYWORD = "default";
    public const string DELEGATE_KEYWORD = "delegate";
    public const string DO_KEYWORD = "do";
    public const string DONE_KEYWORD = "done";
    public const string DOWNCAST_KEYWORD = "downcast";
    public const string DOWNTO_KEYWORD = "downto";
    public const string ELIF_KEYWORD = "elif";
    public const string ELSE_KEYWORD = "else";
    public const string END_KEYWORD = "end";
    public const string EXCEPTION_KEYWORD = "exception";
    public const string EXTERN_KEYWORD = "extern";
    public const string FALSE_KEYWORD = "false";
    public const string FINALLY_KEYWORD = "finally";
    public const string FIXED_KEYWORD = "fixed";
    public const string FOR_KEYWORD = "for";
    public const string FUN_KEYWORD = "fun";
    public const string FUNCTION_KEYWORD = "function";
    public const string GLOBAL_KEYWORD = "global";
    public const string IF_KEYWORD = "if";
    public const string IN_KEYWORD = "in";
    public const string INHERIT_KEYWORD = "inherit";
    public const string INLINE_KEYWORD = "inline";
    public const string INTERFACE_KEYWORD = "interface";
    public const string INTERNAL_KEYWORD = "internal";
    public const string LAZY_KEYWORD = "lazy";
    public const string LET_KEYWORD = "let";
    public const string MATCH_KEYWORD = "match";
    public const string MEMBER_KEYWORD = "member";
    public const string MODULE_KEYWORD = "module";
    public const string MUTABLE_KEYWORD = "mutable";
    public const string NAMESPACE_KEYWORD = "namespace";
    public const string NEW_KEYWORD = "new";
    public const string NOT_KEYWORD = "not";
    public const string NULL_KEYWORD = "null";
    public const string OF_KEYWORD = "of";
    public const string OPEN_KEYWORD = "open";
    public const string OR_KEYWORD = "or";
    public const string OVERRIDE_KEYWORD = "override";
    public const string PRIVATE_KEYWORD = "private";
    public const string PUBLIC_KEYWORD = "public";
    public const string REC_KEYWORD = "rec";
    public const string RETURN_KEYWORD = "return";
    public const string SELECT_KEYWORD = "select";
    public const string STATIC_KEYWORD = "static";
    public const string STRUCT_KEYWORD = "struct";
    public const string THEN_KEYWORD = "then";
    public const string TO_KEYWORD = "to";
    public const string TRUE_KEYWORD = "true";
    public const string TRY_KEYWORD = "try";
    public const string TYPE_KEYWORD = "type";
    public const string UPCAST_KEYWORD = "upcast";
    public const string USE_KEYWORD = "use";
    public const string VAL_KEYWORD = "val";
    public const string VOID_KEYWORD = "void";
    public const string WHEN_KEYWORD = "when";
    public const string WHILE_KEYWORD = "while";
    public const string WITH_KEYWORD = "with";
    public const string YIELD_KEYWORD = "yield";
    public const string CONST_KEYWORD = "const";

    public static readonly ImmutableArray<string> ALL = new[]
    {
    ABSTRACT_KEYWORD,
    AND_KEYWORD,
    AS_KEYWORD,
    ASSERT_KEYWORD,
    BASE_KEYWORD,
    BEGIN_KEYWORD,
    CLASS_KEYWORD,
    DEFAULT_KEYWORD,
    DELEGATE_KEYWORD,
    DO_KEYWORD,
    DONE_KEYWORD,
    DOWNCAST_KEYWORD,
    DOWNTO_KEYWORD,
    ELIF_KEYWORD,
    ELSE_KEYWORD,
    END_KEYWORD,
    EXCEPTION_KEYWORD,
    EXTERN_KEYWORD,
    FALSE_KEYWORD,
    FINALLY_KEYWORD,
    FIXED_KEYWORD,
    FOR_KEYWORD,
    FUN_KEYWORD,
    FUNCTION_KEYWORD,
    GLOBAL_KEYWORD,
    IF_KEYWORD,
    IN_KEYWORD,
    INHERIT_KEYWORD,
    INLINE_KEYWORD,
    INTERFACE_KEYWORD,
    INTERNAL_KEYWORD,
    LAZY_KEYWORD,
    LET_KEYWORD,
    MATCH_KEYWORD,
    MEMBER_KEYWORD,
    MODULE_KEYWORD,
    MUTABLE_KEYWORD,
    NAMESPACE_KEYWORD,
    NEW_KEYWORD,
    NOT_KEYWORD,
    NULL_KEYWORD,
    OF_KEYWORD,
    OPEN_KEYWORD,
    OR_KEYWORD,
    OVERRIDE_KEYWORD,
    PRIVATE_KEYWORD,
    PUBLIC_KEYWORD,
    REC_KEYWORD,
    RETURN_KEYWORD,
    SELECT_KEYWORD,
    STATIC_KEYWORD,
    STRUCT_KEYWORD,
    THEN_KEYWORD,
    TO_KEYWORD,
    TRUE_KEYWORD,
    TRY_KEYWORD,
    TYPE_KEYWORD,
    UPCAST_KEYWORD,
    USE_KEYWORD,
    VAL_KEYWORD,
    VOID_KEYWORD,
    WHEN_KEYWORD,
    WHILE_KEYWORD,
    WITH_KEYWORD,
    YIELD_KEYWORD,
    CONST_KEYWORD,
}.ToImmutableArray();
}