using Luthetus.Common.RazorLib.FileSystem.Classes.FilePath;
using Luthetus.Common.RazorLib.FileSystem.Classes.Local;
using Luthetus.Common.RazorLib.Namespaces;
using Luthetus.CompilerServices.Lang.DotNetSolution;
using Xunit;

namespace Luthetus.Ide.Tests.Basics.DotNet;

public class DotNetSolutionParserTests
{
    [Fact]
    public void AdhocTest()
    {
        var localEnvironmentProvider = new LocalEnvironmentProvider();

        var solutionAbsoluteFilePathString = @"C:\Users\hunte\Repos\BlazorApp1\BlazorApp1.sln";

        var solutionAbsoluteFilePath = new AbsoluteFilePath(
            solutionAbsoluteFilePathString,
            false,
            localEnvironmentProvider);

        var solution = DotNetSolutionLexer.Lex(
            SOLUTION_TEST_DATA,
            new NamespacePath("", solutionAbsoluteFilePath),
            localEnvironmentProvider);
    }

    private const string SOLUTION_TEST_DATA = @"
Microsoft Visual Studio Solution File, Format Version 12.00
Project(""{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"") = ""BlazorServerSideApp"", ""BlazorServerSideApp\BlazorServerSideApp.csproj"", ""{7C8BE884-912D-4EB7-B371-0034CAD0E966}""
EndProject
Project(""{2150E333-8FDC-42A3-9474-1A3956D46DE8}"") = ""Hosts"", ""Hosts"", ""{C4D61A9B-4E78-4185-ADF3-C9DB01F61FE1}""
EndProject
Project(""{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"") = ""BlazorWasmApp"", ""BlazorWasmApp\BlazorWasmApp.csproj"", ""{F1381E33-6EF5-413E-A6F6-D5623AFF78C0}""
EndProject
Project(""{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"") = ""TestsProject"", ""TestsProject\TestsProject.csproj"", ""{17FB7085-85DF-4762-A631-E2FD03DB3481}""
EndProject
Project(""{2150E333-8FDC-42A3-9474-1A3956D46DE8}"") = ""Tests"", ""Tests"", ""{D82F9124-4358-4ED2-981C-70706DC777E1}""
EndProject
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(ProjectConfigurationPlatforms) = postSolution
		{7C8BE884-912D-4EB7-B371-0034CAD0E966}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{7C8BE884-912D-4EB7-B371-0034CAD0E966}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{7C8BE884-912D-4EB7-B371-0034CAD0E966}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{7C8BE884-912D-4EB7-B371-0034CAD0E966}.Release|Any CPU.Build.0 = Release|Any CPU
		{F1381E33-6EF5-413E-A6F6-D5623AFF78C0}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{F1381E33-6EF5-413E-A6F6-D5623AFF78C0}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{F1381E33-6EF5-413E-A6F6-D5623AFF78C0}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{F1381E33-6EF5-413E-A6F6-D5623AFF78C0}.Release|Any CPU.Build.0 = Release|Any CPU
		{17FB7085-85DF-4762-A631-E2FD03DB3481}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{17FB7085-85DF-4762-A631-E2FD03DB3481}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{17FB7085-85DF-4762-A631-E2FD03DB3481}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{17FB7085-85DF-4762-A631-E2FD03DB3481}.Release|Any CPU.Build.0 = Release|Any CPU
	EndGlobalSection
	GlobalSection(NestedProjects) = preSolution
		{7C8BE884-912D-4EB7-B371-0034CAD0E966} = {C4D61A9B-4E78-4185-ADF3-C9DB01F61FE1}
		{F1381E33-6EF5-413E-A6F6-D5623AFF78C0} = {C4D61A9B-4E78-4185-ADF3-C9DB01F61FE1}
		{17FB7085-85DF-4762-A631-E2FD03DB3481} = {D82F9124-4358-4ED2-981C-70706DC777E1}
	EndGlobalSection
EndGlobal
";
}