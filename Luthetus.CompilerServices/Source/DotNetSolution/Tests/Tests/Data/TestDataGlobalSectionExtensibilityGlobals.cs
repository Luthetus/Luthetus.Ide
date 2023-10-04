namespace Luthetus.CompilerServices.Lang.DotNetSolution.Tests.TestData;

public class TestDataGlobalSectionExtensibilityGlobals
{
    public const string START_TOKEN_ORDER = @"postSolution";

    public const string FIRST_PROPERTY_NAME = @"SolutionGuid";
    public const string FIRST_PROPERTY_VALUE = @"{04F0131F-99FC-457C-BC14-8AAFA8F9952F}";

    public const string FULL =
        @"GlobalSection(ExtensibilityGlobals) = postSolution
		SolutionGuid = {04F0131F-99FC-457C-BC14-8AAFA8F9952F}
	EndGlobalSection";
}
