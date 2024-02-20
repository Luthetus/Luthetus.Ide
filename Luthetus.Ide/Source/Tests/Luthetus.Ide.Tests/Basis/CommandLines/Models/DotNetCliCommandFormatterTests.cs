using Luthetus.Common.RazorLib.FileSystems.Models;

namespace Luthetus.Ide.Tests.Basis.CommandLines.Models;

public class DotNetCliCommandFormatterTests
{
    [Fact]
    public void DOT_NET_CLI_TARGET_FILE_NAME()
    {
        //public const string  = "dotnet";
    }

    [Fact]
    public void FormatStartProjectWithoutDebugging()
    {
        //public static FormattedCommand (IAbsolutePath projectAbsolutePath)
    }

    [Fact]
    public void FormatStartProjectWithoutDebugging()
    {
        //public static FormattedCommand (string projectPath)
    }

    [Fact]
    public void FormatDotnetNewSln()
    {
        //public static FormattedCommand (string solutionName)
    }

    [Fact]
    public void FormatDotnetNewCSharpProject()
    {
        //public static FormattedCommand (
        //    string projectTemplateName,
        //    string cSharpProjectName,
        //    string optionalParameters)
    }

    [Fact]
    public void FormatAddExistingProjectToSolution()
    {
        //public static FormattedCommand (
        //    string solutionAbsolutePathString,
        //    string cSharpProjectPath)
    }

    [Fact]
    public void FormatRemoveCSharpProjectReferenceFromSolutionAction()
    {
        //public static FormattedCommand (
        //    string solutionAbsolutePathString,
        //    string cSharpProjectAbsolutePathString)
    }

    [Fact]
    public void FormatAddNugetPackageReferenceToProject()
    {
        //public static FormattedCommand (
        //    string cSharpProjectAbsolutePathString,
        //    string nugetPackageId,
        //    string nugetPackageVersion)
    }

    [Fact]
    public void FormatRemoveNugetPackageReferenceFromProject()
    {
        //public static FormattedCommand (
        //    string cSharpProjectAbsolutePathString,
        //    string nugetPackageId)
    }

    [Fact]
    public void FormatAddProjectToProjectReference()
    {
        //public static FormattedCommand (
        //    string receivingProjectAbsolutePathString,
        //    string referenceProjectAbsolutePathString)
    }

    [Fact]
    public void FormatRemoveProjectToProjectReference()
    {
        //public static FormattedCommand (
        //    string modifyProjectAbsolutePathString,
        //    string referenceProjectAbsolutePathString)
    }

    [Fact]
    public void FormatMoveProjectToSolutionFolder()
    {
        //public static FormattedCommand (
        //    string solutionAbsolutePathString,
        //    string projectToMoveAbsolutePathString,
        //    string solutionFolderPath)
    }

    [Fact]
    public void FormatDotnetNewList()
    {
        //public static FormattedCommand () =>
    }

    [Fact]
    public void FormatDotnetNewListDeprecated()
    {
        //public static FormattedCommand () =>
    }

    [Fact]
    public void FormatDotNetTestListTests()
    {
        //public static FormattedCommand () =>
    }

    [Fact]
    public void FormatDotNetTestByFullyQualifiedName()
    {
        //public static FormattedCommand (string fullyQualifiedName) =>
    }
}