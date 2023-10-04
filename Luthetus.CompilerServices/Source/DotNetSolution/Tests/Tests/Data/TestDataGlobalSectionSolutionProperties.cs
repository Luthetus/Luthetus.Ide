namespace Luthetus.CompilerServices.Lang.DotNetSolution.Tests.TestData;

public class TestDataGlobalSectionSolutionProperties
{
    public const string START_TOKEN_ORDER = @"preSolution";
    public const string HIDE_SOLUTION_NODE_VALUE = @"FALSE";

    public const string FULL =
        @"GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection";
}
