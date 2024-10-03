using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Python.Facts;

public partial class PythonLanguageFacts
{
    public class Preprocessor
    {
        public class Directives
        {
            public static readonly ImmutableArray<string> All = ImmutableArray<string>.Empty;
        }

        public class Variables
        {
            public static readonly ImmutableArray<string> All = ImmutableArray<string>.Empty;
        }
    }
}