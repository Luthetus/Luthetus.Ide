namespace Luthetus.CompilerServices.Lang.DotNetSolution.Tests.TestData;

public class TestDataHeader
{
    public const string FORMAT_VERSION_NO_ENDING_NEWLINE =
        @"Microsoft Visual Studio Solution File, Format Version 12.00";

    public const string HASHTAG_VISUAL_STUDIO_VERSION =
        @"# Visual Studio Version 17";

    public const string EXACT_VISUAL_STUDIO_VERSION =
        @"VisualStudioVersion = 17.7.34018.315";

    public const string MINIMUM_VISUAL_STUDIO_VERSION =
        @"MinimumVisualStudioVersion = 10.0.40219.1";

    public const string FULL = @"
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version 17
VisualStudioVersion = 17.7.34018.315
MinimumVisualStudioVersion = 10.0.40219.1";
}
