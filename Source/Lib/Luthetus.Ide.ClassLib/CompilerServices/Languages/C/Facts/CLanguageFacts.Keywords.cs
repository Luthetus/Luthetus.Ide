using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.Parsing.C.Facts;

public partial class CLanguageFacts
{
    public class Keywords
    {
        public const string AUTO_KEYWORD = "auto";
        public const string BREAK_KEYWORD = "break";
        public const string CASE_KEYWORD = "case";
        public const string CHAR_KEYWORD = "char";
        public const string CONST_KEYWORD = "const";
        public const string CONTINUE_KEYWORD = "continue";
        public const string DEFAULT_KEYWORD = "default";
        public const string DO_KEYWORD = "do";
        public const string DOUBLE_KEYWORD = "double";
        public const string ELSE_KEYWORD = "else";
        public const string ENUM_KEYWORD = "enum";
        public const string EXTERN_KEYWORD = "extern";
        public const string FLOAT_KEYWORD = "float";
        public const string FOR_KEYWORD = "for";
        public const string GOTO_KEYWORD = "goto";
        public const string IF_KEYWORD = "if";
        public const string INT_KEYWORD = "int";
        public const string LONG_KEYWORD = "long";
        public const string REGISTER_KEYWORD = "register";
        public const string RETURN_KEYWORD = "return";
        public const string SHORT_KEYWORD = "short";
        public const string SIGNED_KEYWORD = "signed";
        public const string SIZEOF_KEYWORD = "sizeof";
        public const string STATIC_KEYWORD = "static";
        public const string STRUCT_KEYWORD = "struct";
        public const string SWITCH_KEYWORD = "switch";
        public const string TYPEDEF_KEYWORD = "typedef";
        public const string UNION_KEYWORD = "union";
        public const string UNSIGNED_KEYWORD = "unsigned";
        public const string VOID_KEYWORD = "void";
        public const string VOLATILE_KEYWORD = "volatile";
        public const string WHILE_KEYWORD = "while";

        public static readonly ImmutableArray<string> ALL = new[]
        {
            AUTO_KEYWORD,
            BREAK_KEYWORD,
            CASE_KEYWORD,
            CHAR_KEYWORD,
            CONST_KEYWORD,
            CONTINUE_KEYWORD,
            DEFAULT_KEYWORD,
            DO_KEYWORD,
            DOUBLE_KEYWORD,
            ELSE_KEYWORD,
            ENUM_KEYWORD,
            EXTERN_KEYWORD,
            FLOAT_KEYWORD,
            FOR_KEYWORD,
            GOTO_KEYWORD,
            IF_KEYWORD,
            INT_KEYWORD,
            LONG_KEYWORD,
            REGISTER_KEYWORD,
            RETURN_KEYWORD,
            SHORT_KEYWORD,
            SIGNED_KEYWORD,
            SIZEOF_KEYWORD,
            STATIC_KEYWORD,
            STRUCT_KEYWORD,
            SWITCH_KEYWORD,
            TYPEDEF_KEYWORD,
            UNION_KEYWORD,
            UNSIGNED_KEYWORD,
            VOID_KEYWORD,
            VOLATILE_KEYWORD,
            WHILE_KEYWORD,
        }.ToImmutableArray();
        
        public static readonly ImmutableArray<string> CONTROL_KEYWORDS = new[]
        {
            BREAK_KEYWORD,
            CASE_KEYWORD,
            CONTINUE_KEYWORD,
            DO_KEYWORD,
            ELSE_KEYWORD,
            FOR_KEYWORD,
            GOTO_KEYWORD,
            IF_KEYWORD,
            RETURN_KEYWORD,
            SWITCH_KEYWORD,
            WHILE_KEYWORD,
        }.ToImmutableArray();
    }
}