using Luthetus.Common.RazorLib.KeyCase;
using Luthetus.Ide.RazorLib.TerminalCase.Models;

namespace Luthetus.Ide.RazorLib.DotNetSolutionCase.Viewables;

public class DotNetSolutionFormScene
{
    public Key<TerminalCommand> NewDotNetSolutionTerminalCommandKey { get; } = Key<TerminalCommand>.NewKey();

    public CancellationTokenSource NewDotNetSolutionCancellationTokenSource { get; set; } = new();
    public string SolutionName { get; set; } = string.Empty;
    public string ParentDirectoryName { get; set; } = string.Empty;
    

    public const string HackForWebsite_NEW_SOLUTION_TEMPLATE = @"
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version 17
VisualStudioVersion = 17.7.34018.315
MinimumVisualStudioVersion = 10.0.40219.1
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(ProjectConfigurationPlatforms) = postSolution
		{EC571C96-8996-402C-B44A-264F84598795}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{EC571C96-8996-402C-B44A-264F84598795}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{EC571C96-8996-402C-B44A-264F84598795}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{EC571C96-8996-402C-B44A-264F84598795}.Release|Any CPU.Build.0 = Release|Any CPU
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
	GlobalSection(ExtensibilityGlobals) = postSolution
		SolutionGuid = {CC0E8FC7-3D42-4480-BAF6-86D1E2F2289E}
	EndGlobalSection
EndGlobal
";
}
