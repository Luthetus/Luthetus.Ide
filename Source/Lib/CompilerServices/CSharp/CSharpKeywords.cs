using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices;

namespace Luthetus.CompilerServices.CSharp;

public class CSharpKeywords
{
    // Non-Contextual Keywords
    public const string ABSTRACT_KEYWORD = "abstract";
    public const string AS_KEYWORD = "as";
    public const string BASE_KEYWORD = "base";
    public const string BOOL_KEYWORD = "bool";
    public const string BREAK_KEYWORD = "break";
    public const string BYTE_KEYWORD = "byte";
    public const string CASE_KEYWORD = "case";
    public const string CATCH_KEYWORD = "catch";
    public const string CHAR_KEYWORD = "char";
    public const string CHECKED_KEYWORD = "checked";
    public const string CLASS_KEYWORD = "class";
    public const string CONST_KEYWORD = "const";
    public const string CONTINUE_KEYWORD = "continue";
    public const string DECIMAL_KEYWORD = "decimal";
    public const string DEFAULT_KEYWORD = "default";
    public const string DELEGATE_KEYWORD = "delegate";
    public const string DO_KEYWORD = "do";
    public const string DOUBLE_KEYWORD = "double";
    public const string ELSE_KEYWORD = "else";
    public const string ENUM_KEYWORD = "enum";
    public const string EVENT_KEYWORD = "event";
    public const string EXPLICIT_KEYWORD = "explicit";
    public const string EXTERN_KEYWORD = "extern";
    public const string FALSE_KEYWORD = "false";
    public const string FINALLY_KEYWORD = "finally";
    public const string FIXED_KEYWORD = "fixed";
    public const string FLOAT_KEYWORD = "float";
    public const string FOR_KEYWORD = "for";
    public const string FOREACH_KEYWORD = "foreach";
    public const string GOTO_KEYWORD = "goto";
    public const string IF_KEYWORD = "if";
    public const string IMPLICIT_KEYWORD = "implicit";
    public const string IN_KEYWORD = "in";
    public const string INT_KEYWORD = "int";
    public const string INTERFACE_KEYWORD = "interface";
    public const string INTERNAL_KEYWORD = "internal";
    public const string IS_KEYWORD = "is";
    public const string LOCK_KEYWORD = "lock";
    public const string LONG_KEYWORD = "long";
    public const string NAMESPACE_KEYWORD = "namespace";
    public const string NEW_KEYWORD = "new";
    public const string NULL_KEYWORD = "null";
    public const string OBJECT_KEYWORD = "object";
    public const string OPERATOR_KEYWORD = "operator";
    public const string OUT_KEYWORD = "out";
    public const string OVERRIDE_KEYWORD = "override";
    public const string PARAMS_KEYWORD = "params";
    public const string PRIVATE_KEYWORD = "private";
    public const string PROTECTED_KEYWORD = "protected";
    public const string PUBLIC_KEYWORD = "public";
    public const string READONLY_KEYWORD = "readonly";
    public const string REF_KEYWORD = "ref";
    public const string RETURN_KEYWORD = "return";
    public const string SBYTE_KEYWORD = "sbyte";
    public const string SEALED_KEYWORD = "sealed";
    public const string SHORT_KEYWORD = "short";
    public const string SIZEOF_KEYWORD = "sizeof";
    public const string STACKALLOC_KEYWORD = "stackalloc";
    public const string STATIC_KEYWORD = "static";
    public const string STRING_KEYWORD = "string";
    public const string STRUCT_KEYWORD = "struct";
    public const string SWITCH_KEYWORD = "switch";
    public const string THIS_KEYWORD = "this";
    public const string THROW_KEYWORD = "throw";
    public const string TRUE_KEYWORD = "true";
    public const string TRY_KEYWORD = "try";
    public const string TYPEOF_KEYWORD = "typeof";
    public const string UINT_KEYWORD = "uint";
    public const string ULONG_KEYWORD = "ulong";
    public const string UNCHECKED_KEYWORD = "unchecked";
    public const string UNSAFE_KEYWORD = "unsafe";
    public const string USHORT_KEYWORD = "ushort";
    public const string USING_KEYWORD = "using";
    public const string VIRTUAL_KEYWORD = "virtual";
    public const string VOID_KEYWORD = "void";
    public const string VOLATILE_KEYWORD = "volatile";
    public const string WHILE_KEYWORD = "while";

    // Contextual Keywords
    public const string ADD_KEYWORD = "add";
    public const string AND_KEYWORD = "and";
    public const string ALIAS_KEYWORD = "alias";
    public const string ASCENDING_KEYWORD = "ascending";
    public const string ARGS_KEYWORD = "args";
    public const string ASYNC_KEYWORD = "async";
    public const string AWAIT_KEYWORD = "await";
    public const string BY_KEYWORD = "by";
    public const string DESCENDING_KEYWORD = "descending";
    public const string DYNAMIC_KEYWORD = "dynamic";
    public const string EQUALS_KEYWORD = "equals";
    public const string FILE_KEYWORD = "file";
    public const string FROM_KEYWORD = "from";
    public const string GET_KEYWORD = "get";
    public const string GLOBAL_KEYWORD = "global";
    public const string GROUP_KEYWORD = "group";
    public const string INIT_KEYWORD = "init";
    public const string INTO_KEYWORD = "into";
    public const string JOIN_KEYWORD = "join";
    public const string LET_KEYWORD = "let";
    public const string MANAGED_KEYWORD = "managed"; // (function pointer calling convention)
    public const string NAMEOF_KEYWORD = "nameof";
    public const string NINT_KEYWORD = "nint";
    public const string NOT_KEYWORD = "not";
    public const string NOTNULL_KEYWORD = "notnull";
    public const string NUINT_KEYWORD = "nuint";
    public const string ON_KEYWORD = "on";
    public const string OR_KEYWORD = "or";
    public const string ORDERBY_KEYWORD = "orderby";
    public const string PARTIAL_KEYWORD = "partial"; // (type) // public const string partial_KEYWORD = "partial"; // (method)
    public const string RECORD_KEYWORD = "record";
    public const string REMOVE_KEYWORD = "remove";
    public const string REQUIRED_KEYWORD = "required";
    public const string SCOPED_KEYWORD = "scoped";
    public const string SELECT_KEYWORD = "select";
    public const string SET_KEYWORD = "set";
    public const string UNMANAGED_KEYWORD = "unmanaged"; // (function pointer calling convention) // public const string unmanaged_KEYWORD = "unmanaged"; // (generic type constraint)
    public const string VALUE_KEYWORD = "value";
    public const string VAR_KEYWORD = "var";
    public const string WHEN_KEYWORD = "when"; // (filter condition)
    public const string WHERE_KEYWORD = "where"; // (generic type constraint) // public const string WHERE_KEYWORD = "where"; // (query clause)
    public const string WITH_KEYWORD = "with";
    public const string YIELD_KEYWORD = "yield";

    public static readonly string[] NON_CONTEXTUAL_KEYWORDS = new[]
    {
        ABSTRACT_KEYWORD,
        AS_KEYWORD,
        BASE_KEYWORD,
        BOOL_KEYWORD,
        BREAK_KEYWORD,
        BYTE_KEYWORD,
        CASE_KEYWORD,
        CATCH_KEYWORD,
        CHAR_KEYWORD,
        CHECKED_KEYWORD,
        CLASS_KEYWORD,
        CONST_KEYWORD,
        CONTINUE_KEYWORD,
        DECIMAL_KEYWORD,
        DEFAULT_KEYWORD,
        DELEGATE_KEYWORD,
        DO_KEYWORD,
        DOUBLE_KEYWORD,
        ELSE_KEYWORD,
        ENUM_KEYWORD,
        EVENT_KEYWORD,
        EXPLICIT_KEYWORD,
        EXTERN_KEYWORD,
        FALSE_KEYWORD,
        FINALLY_KEYWORD,
        FIXED_KEYWORD,
        FLOAT_KEYWORD,
        FOR_KEYWORD,
        FOREACH_KEYWORD,
        GOTO_KEYWORD,
        IF_KEYWORD,
        IMPLICIT_KEYWORD,
        IN_KEYWORD,
        INT_KEYWORD,
        INTERFACE_KEYWORD,
        INTERNAL_KEYWORD,
        IS_KEYWORD,
        LOCK_KEYWORD,
        LONG_KEYWORD,
        NAMESPACE_KEYWORD,
        NEW_KEYWORD,
        NULL_KEYWORD,
        OBJECT_KEYWORD,
        OPERATOR_KEYWORD,
        OUT_KEYWORD,
        OVERRIDE_KEYWORD,
        PARAMS_KEYWORD,
        PRIVATE_KEYWORD,
        PROTECTED_KEYWORD,
        PUBLIC_KEYWORD,
        READONLY_KEYWORD,
        REF_KEYWORD,
        RETURN_KEYWORD,
        SBYTE_KEYWORD,
        SEALED_KEYWORD,
        SHORT_KEYWORD,
        SIZEOF_KEYWORD,
        STACKALLOC_KEYWORD,
        STATIC_KEYWORD,
        STRING_KEYWORD,
        STRUCT_KEYWORD,
        SWITCH_KEYWORD,
        THIS_KEYWORD,
        THROW_KEYWORD,
        TRUE_KEYWORD,
        TRY_KEYWORD,
        TYPEOF_KEYWORD,
        UINT_KEYWORD,
        ULONG_KEYWORD,
        UNCHECKED_KEYWORD,
        UNSAFE_KEYWORD,
        USHORT_KEYWORD,
        USING_KEYWORD,
        VIRTUAL_KEYWORD,
        VOID_KEYWORD,
        VOLATILE_KEYWORD,
        WHILE_KEYWORD,
    };

    public static readonly string[] CONTROL_KEYWORDS = new[]
    {
        BREAK_KEYWORD,
        CASE_KEYWORD,
        CONTINUE_KEYWORD,
        DO_KEYWORD,
        ELSE_KEYWORD,
        FOR_KEYWORD,
        FOREACH_KEYWORD,
        GOTO_KEYWORD,
        IF_KEYWORD,
        RETURN_KEYWORD,
        SWITCH_KEYWORD,
        THROW_KEYWORD,
        WHILE_KEYWORD,
        YIELD_KEYWORD
    };

    public static readonly string[] CONTEXTUAL_KEYWORDS = new[]
    {
        ADD_KEYWORD,
        AND_KEYWORD,
        ALIAS_KEYWORD,
        ASCENDING_KEYWORD,
        ARGS_KEYWORD,
        ASYNC_KEYWORD,
        AWAIT_KEYWORD,
        BY_KEYWORD,
        DESCENDING_KEYWORD,
        DYNAMIC_KEYWORD,
        EQUALS_KEYWORD,
        FILE_KEYWORD,
        FROM_KEYWORD,
        GET_KEYWORD,
        GLOBAL_KEYWORD,
        GROUP_KEYWORD,
        INIT_KEYWORD,
        INTO_KEYWORD,
        JOIN_KEYWORD,
        LET_KEYWORD,
        MANAGED_KEYWORD,
        NAMEOF_KEYWORD,
        NINT_KEYWORD,
        NOT_KEYWORD,
        NOTNULL_KEYWORD,
        NUINT_KEYWORD,
        ON_KEYWORD,
        OR_KEYWORD,
        ORDERBY_KEYWORD,
        PARTIAL_KEYWORD,
        RECORD_KEYWORD,
        REMOVE_KEYWORD,
        REQUIRED_KEYWORD,
        SCOPED_KEYWORD,
        SELECT_KEYWORD,
        SET_KEYWORD,
        UNMANAGED_KEYWORD,
        VALUE_KEYWORD,
        VAR_KEYWORD,
        WHEN_KEYWORD,
        WHERE_KEYWORD,
        WITH_KEYWORD,
        YIELD_KEYWORD,
    };
    
    public static readonly string[] ALL_KEYWORDS = NON_CONTEXTUAL_KEYWORDS
        .Union(CONTEXTUAL_KEYWORDS)
        .ToArray();
        
    public static readonly HashSet<string> ALL_KEYWORDS_HASH_SET = new(ALL_KEYWORDS);
        
    public static readonly LexerKeywords LexerKeywords = new LexerKeywords(
    	NON_CONTEXTUAL_KEYWORDS,
    	CONTROL_KEYWORDS, 
    	CONTEXTUAL_KEYWORDS);
}