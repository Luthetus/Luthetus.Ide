namespace Luthetus.CompilerServices.Lang.DotNetSolution.Tests.TestData;

public class TestDataGlobalSection
{
    public const string START_TOKEN_PARAMETER = @"SolutionConfigurationPlatforms";
    public const string START_TOKEN_ORDER = @"preSolution";

    public const string FULL = @"GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection";
}
