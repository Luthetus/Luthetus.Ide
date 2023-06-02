namespace Luthetus.Ide.Tests.Basics.CompilerServices.Languages.CSharp.SemanticContextCase;

public partial class SemanticContextTests
{
	/// <summary>Perferably all tests would be self contained. But this .NET Solution source text is a lot and until I see issues arise I'm going to share this text among the <see cref="IContext"/> tests.</summary>
    public const string SLN_HEADER = @"
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version 17
VisualStudioVersion = 17.5.33627.172
MinimumVisualStudioVersion = 10.0.40219.1
";
	/// <summary>Perferably all tests would be self contained. But this .NET Solution source text is a lot and until I see issues arise I'm going to share this text among the <see cref="IContext"/> tests.</summary>
    public const string SLN_GLOBAL = @"Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(ProjectConfigurationPlatforms) = postSolution
		{A41C752D-A976-4337-8865-7EA232E0AE7E}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{A41C752D-A976-4337-8865-7EA232E0AE7E}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{A41C752D-A976-4337-8865-7EA232E0AE7E}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{A41C752D-A976-4337-8865-7EA232E0AE7E}.Release|Any CPU.Build.0 = Release|Any CPU
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
	GlobalSection(ExtensibilityGlobals) = postSolution
		SolutionGuid = {F37C2ECB-ABC6-40C3-B9B7-96D353EA0C26}
	EndGlobalSection
EndGlobal
";
}
