using System.Collections.Immutable;

namespace Luthetus.CompilerServices.DotNetSolution.Facts;

public partial class LexSolutionFacts
{
    public class Header
    {
        public const string FORMAT_VERSION_START_TOKEN =
            @"Microsoft Visual Studio Solution File, Format Version";

        public const string HASHTAG_VISUAL_STUDIO_VERSION_START_TOKEN =
            @"# Visual Studio Version";

        public const string EXACT_VISUAL_STUDIO_VERSION_START_TOKEN =
            @"VisualStudioVersion =";

        public const string MINIMUM_VISUAL_STUDIO_VERSION_START_TOKEN =
            @"MinimumVisualStudioVersion =";

        public static readonly ImmutableArray<string> WellKnownAssociatedNamesList = new[]
        {
            FORMAT_VERSION_START_TOKEN,
            HASHTAG_VISUAL_STUDIO_VERSION_START_TOKEN,
            EXACT_VISUAL_STUDIO_VERSION_START_TOKEN,
            MINIMUM_VISUAL_STUDIO_VERSION_START_TOKEN,
        }.ToImmutableArray();
    }
}
