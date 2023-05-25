using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Languages.C.Facts;

public partial class CLanguageFacts
{
    public class Preprocessor
    {
        public class Directives
        {
            public const string INCLUDE = "#include";
            public const string DEFINE = "#define";
            public const string UNDEF = "#undef";
            public const string IF = "#if";
            public const string IFDEF = "#ifdef";
            public const string IFNDEF = "#ifndef";
            public const string ERROR = "#error";
            public const string FILE = "__FILE__";
            public const string LINE = "__LINE__";
            public const string DATE = "__DATE__";
            public const string TIME = "__TIME__";
            public const string TIMESTAMP = "__TIMESTAMP__";
            public const string PRAGMA = "#pragma";
            
            // TODO: What are these preprocessor directives? I think the idea was to type '#' then a defined macro? Its not "actually" a named preprocessor command.
            //
            // public const string MACRO_OPERATOR_SINGLE_HASHTAG = " macro operator"; // TODO: This used to be the string literal value of "# macro operator". Which is correct?
            // public const string MACRO_OPERATOR_DOUBLE_HASHTAG = " macro operator"; // TODO: This used to be the string literal value of "## macro operator". Which is correct?

            public static readonly ImmutableArray<string> All = new[]
            {
                INCLUDE,
                DEFINE,
                UNDEF,
                IF,
                IFDEF,
                IFNDEF,
                ERROR,
                FILE,
                LINE,
                DATE,
                TIME,
                TIMESTAMP,
                PRAGMA,
            }.ToImmutableArray();
        }
        
        public class Variables
        {
            public const string FILE = "__FILE__";
            public const string LINE = "__LINE__";
            public const string DATE = "__DATE__";
            public const string TIME = "__TIME__";
            public const string TIMESTAMP = "__TIMESTAMP__";

            public static readonly ImmutableArray<string> All = new[]
            {
                FILE,
                LINE,
                DATE,
                TIME,
                TIMESTAMP,
            }.ToImmutableArray();
        }
    }
}