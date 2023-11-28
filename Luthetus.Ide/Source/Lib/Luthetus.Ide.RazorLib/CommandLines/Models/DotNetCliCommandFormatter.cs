using Luthetus.Common.RazorLib.FileSystems.Models;

namespace Luthetus.Ide.RazorLib.CommandLines.Models;

/// <summary>
/// Any values given will NOT be wrapped in quotes internally at this step.
/// Later, if one uses CliWrap, then at that point they will be wrapped in quotes.
/// </summary>
public static class DotNetCliCommandFormatter
{
    public const string DOT_NET_CLI_TARGET_FILE_NAME = "dotnet";

    public static FormattedCommand FormatStartProjectWithoutDebugging(IAbsolutePath projectAbsolutePath)
    {
        return FormatStartProjectWithoutDebugging(projectAbsolutePath.Value);
    }

    public static FormattedCommand FormatStartProjectWithoutDebugging(string projectPath)
    {
        return new FormattedCommand(DOT_NET_CLI_TARGET_FILE_NAME, new[]
        {
            "run",
            "--project",
            projectPath
        });
    }

    public static FormattedCommand FormatDotnetNewSln(string solutionName)
    {
        return new FormattedCommand(DOT_NET_CLI_TARGET_FILE_NAME, new[]
        {
            "new",
            "sln",
            "-o",
            solutionName
        });
    }

    public static FormattedCommand FormatDotnetNewCSharpProject(
        string projectTemplateName,
        string cSharpProjectName,
        string optionalParameters)
    {
        var argumentsBag = new List<string>
        {
            "new",
            projectTemplateName,
            "-o",
            cSharpProjectName
        };

        if (!string.IsNullOrWhiteSpace(optionalParameters))
            argumentsBag.Add(optionalParameters);

        return new FormattedCommand(DOT_NET_CLI_TARGET_FILE_NAME, argumentsBag);
    }

    public static FormattedCommand FormatAddExistingProjectToSolution(
        string solutionAbsolutePathString,
        string cSharpProjectPath)
    {
        return new FormattedCommand(DOT_NET_CLI_TARGET_FILE_NAME, new[]
        {
            "sln",
            solutionAbsolutePathString,
            "add",
            cSharpProjectPath
        });
    }

    public static FormattedCommand FormatRemoveCSharpProjectReferenceFromSolutionAction(
        string solutionAbsolutePathString,
        string cSharpProjectAbsolutePathString)
    {
        return new FormattedCommand(DOT_NET_CLI_TARGET_FILE_NAME, new[]
        {
            "sln",
            solutionAbsolutePathString,
            "remove",
            cSharpProjectAbsolutePathString
        });
    }

    public static FormattedCommand FormatAddNugetPackageReferenceToProject(
        string cSharpProjectAbsolutePathString,
        string nugetPackageId,
        string nugetPackageVersion)
    {
        return new FormattedCommand(DOT_NET_CLI_TARGET_FILE_NAME, new[]
        {
            "add",
            cSharpProjectAbsolutePathString,
            "package",
            nugetPackageId,
            "--version",
            nugetPackageVersion
        });
    }

    public static FormattedCommand FormatRemoveNugetPackageReferenceFromProject(
        string cSharpProjectAbsolutePathString,
        string nugetPackageId)
    {
        return new FormattedCommand(DOT_NET_CLI_TARGET_FILE_NAME, new[]
        {
            "remove",
            cSharpProjectAbsolutePathString,
            "package",
            nugetPackageId
        });
    }

    public static FormattedCommand FormatAddProjectToProjectReference(
        string receivingProjectAbsolutePathString,
        string referenceProjectAbsolutePathString)
    {
        return new FormattedCommand(DOT_NET_CLI_TARGET_FILE_NAME, new[]
        {
            "add",
            receivingProjectAbsolutePathString,
            "reference",
            referenceProjectAbsolutePathString
        });
    }

    public static FormattedCommand FormatRemoveProjectToProjectReference(
        string modifyProjectAbsolutePathString,
        string referenceProjectAbsolutePathString)
    {
        return new FormattedCommand(DOT_NET_CLI_TARGET_FILE_NAME, new[]
        {
            "remove",
            modifyProjectAbsolutePathString,
            "reference",
            referenceProjectAbsolutePathString
        });
    }

    public static FormattedCommand FormatMoveProjectToSolutionFolder(
        string solutionAbsolutePathString,
        string projectToMoveAbsolutePathString,
        string solutionFolderPath)
    {
        return new FormattedCommand(DOT_NET_CLI_TARGET_FILE_NAME, new[]
        {
            "sln",
            solutionAbsolutePathString,
            "add",
            projectToMoveAbsolutePathString,
            "--solution-folder",
            solutionFolderPath,
        });
    }

    public static FormattedCommand FormatDotnetNewList() =>
        new FormattedCommand(DOT_NET_CLI_TARGET_FILE_NAME, new[]
        {
            "new",
            "list",
        });

    public static FormattedCommand FormatDotnetNewListDeprecated() =>
        new FormattedCommand(DOT_NET_CLI_TARGET_FILE_NAME, new[]
        {
            "new",
            "--list",
        });
	
	public static FormattedCommand FormatDotNetTestListTests() =>
        new FormattedCommand(DOT_NET_CLI_TARGET_FILE_NAME, new[]
        {
            "test",
            "-t",
        });
}