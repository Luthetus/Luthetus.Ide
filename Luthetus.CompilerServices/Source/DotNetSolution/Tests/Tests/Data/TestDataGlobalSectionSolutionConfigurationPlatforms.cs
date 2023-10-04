namespace Luthetus.CompilerServices.Lang.DotNetSolution.Tests.TestData;

public class TestDataGlobalSectionSolutionConfigurationPlatforms
{
    public const string START_TOKEN_ORDER = @"preSolution";

    public const string FIRST_PROPERTY_NAME = @"Debug|Any CPU";
    public const string FIRST_PROPERTY_VALUE = @"Debug|Any CPU";

    public const string SECOND_PROPERTY_NAME = @"Release|Any CPU";
    public const string SECOND_PROPERTY_VALUE = @"Release|Any CPU";

    public const string FULL = @"GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection";
}
