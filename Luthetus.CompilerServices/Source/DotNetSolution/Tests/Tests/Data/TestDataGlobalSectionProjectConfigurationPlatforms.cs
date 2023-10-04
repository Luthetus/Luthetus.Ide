namespace Luthetus.CompilerServices.Lang.DotNetSolution.Tests.TestData;

public class TestDataGlobalSectionProjectConfigurationPlatforms
{
    public const string START_TOKEN_ORDER = @"postSolution";

    public const string FIRST_PROPERTY_NAME = @"{623099D9-D9DE-47E8-8CCD-BC301C82F70F}.Debug|Any CPU.ActiveCfg";
    public const string FIRST_PROPERTY_VALUE = @"Debug|Any CPU";

    public const string SECOND_PROPERTY_NAME = @"{623099D9-D9DE-47E8-8CCD-BC301C82F70F}.Debug|Any CPU.Build.0";
    public const string SECOND_PROPERTY_VALUE = @"Debug|Any CPU";

    public const string THIRD_PROPERTY_NAME = @"{623099D9-D9DE-47E8-8CCD-BC301C82F70F}.Release|Any CPU.ActiveCfg";
    public const string THIRD_PROPERTY_VALUE = @"Release|Any CPU";

    public const string FOURTH_PROPERTY_NAME = @"{623099D9-D9DE-47E8-8CCD-BC301C82F70F}.Release|Any CPU.Build.0";
    public const string FOURTH_PROPERTY_VALUE = @"Release|Any CPU";

    public const string FULL =
        @"GlobalSection(ProjectConfigurationPlatforms) = postSolution
		{623099D9-D9DE-47E8-8CCD-BC301C82F70F}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{623099D9-D9DE-47E8-8CCD-BC301C82F70F}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{623099D9-D9DE-47E8-8CCD-BC301C82F70F}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{623099D9-D9DE-47E8-8CCD-BC301C82F70F}.Release|Any CPU.Build.0 = Release|Any CPU
	EndGlobalSection";
}
