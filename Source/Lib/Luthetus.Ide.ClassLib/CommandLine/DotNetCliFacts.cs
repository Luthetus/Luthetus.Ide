using Luthetus.Common.RazorLib.FileSystem.Interfaces;

namespace Luthetus.Ide.ClassLib.CommandLine;

/// <summary>
/// Any values given will NOT be wrapped in quotes internally at this step.
/// Later, if one uses CliWrap, then at that point they will be wrapped in quotes.
/// </summary>
public static class DotNetCliFacts
{
    public const string DOT_NET_CLI_TARGET_FILE_NAME = "dotnet";

    public static FormattedCommand FormatStartProjectWithoutDebugging(
        IAbsolutePath projectAbsoluteFilePath)
    {
        return FormatStartProjectWithoutDebugging(
            projectAbsoluteFilePath.FormattedInput);
    }

    public static FormattedCommand FormatStartProjectWithoutDebugging(
        string projectPath)
    {
        return new FormattedCommand(
            DOT_NET_CLI_TARGET_FILE_NAME,
            new[]
            {
                "run",
                "--project",
                projectPath
            });
    }

    public static FormattedCommand FormatDotnetNewSln(
        string solutionName)
    {
        return new FormattedCommand(
            DOT_NET_CLI_TARGET_FILE_NAME,
            new[]
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
        var arguments = new List<string>
        {
            "new",
            projectTemplateName,
            "-o",
            cSharpProjectName
        };

        if (!string.IsNullOrWhiteSpace(optionalParameters))
            arguments.Add(optionalParameters);

        return new FormattedCommand(
            DOT_NET_CLI_TARGET_FILE_NAME,
            arguments);
    }

    public static FormattedCommand FormatAddExistingProjectToSolution(
        string solutionAbsoluteFilePathString,
        string cSharpProjectPath)
    {
        return new FormattedCommand(
            DOT_NET_CLI_TARGET_FILE_NAME,
            new[]
            {
                "sln",
                solutionAbsoluteFilePathString,
                "add",
                cSharpProjectPath
            });
    }

    public static FormattedCommand FormatRemoveCSharpProjectReferenceFromSolutionAction(
        string solutionAbsoluteFilePathString,
        string cSharpProjectAbsoluteFilePathString)
    {
        return new FormattedCommand(
            DOT_NET_CLI_TARGET_FILE_NAME,
            new[]
            {
                "sln",
                solutionAbsoluteFilePathString,
                "remove",
                cSharpProjectAbsoluteFilePathString
            });
    }

    public static FormattedCommand FormatAddNugetPackageReferenceToProject(
        string cSharpProjectAbsoluteFilePathString,
        string nugetPackageId,
        string nugetPackageVersion)
    {
        return new FormattedCommand(
            DOT_NET_CLI_TARGET_FILE_NAME,
            new[]
            {
                "add",
                cSharpProjectAbsoluteFilePathString,
                "package",
                nugetPackageId,
                "--version",
                nugetPackageVersion
            });
    }

    public static FormattedCommand FormatRemoveNugetPackageReferenceFromProject(
        string cSharpProjectAbsoluteFilePathString,
        string nugetPackageId)
    {
        return new FormattedCommand(
            DOT_NET_CLI_TARGET_FILE_NAME,
            new[]
            {
                "remove",
                cSharpProjectAbsoluteFilePathString,
                "package",
                nugetPackageId
            });
    }

    public static FormattedCommand FormatAddProjectToProjectReference(
        string receivingProjectAbsoluteFilePathString,
        string referenceProjectAbsoluteFilePathString)
    {
        return new FormattedCommand(
            DOT_NET_CLI_TARGET_FILE_NAME,
            new[]
            {
                "add",
                receivingProjectAbsoluteFilePathString,
                "reference",
                referenceProjectAbsoluteFilePathString
            });
    }

    public static FormattedCommand FormatRemoveProjectToProjectReference(
        string modifyProjectAbsoluteFilePathString,
        string referenceProjectAbsoluteFilePathString)
    {
        return new FormattedCommand(
            DOT_NET_CLI_TARGET_FILE_NAME,
            new[]
            {
                "remove",
                modifyProjectAbsoluteFilePathString,
                "reference",
                referenceProjectAbsoluteFilePathString
            });
    }

    public static FormattedCommand FormatMoveProjectToSolutionFolder(
        string solutionAbsoluteFilePathString,
        string projectToMoveAbsoluteFilePathString,
        string solutionFolderPath)
    {
        return new FormattedCommand(
            DOT_NET_CLI_TARGET_FILE_NAME,
            new[]
            {
                "sln",
                solutionAbsoluteFilePathString,
                "add",
                projectToMoveAbsoluteFilePathString,
                "--solution-folder",
                solutionFolderPath,
            });
    }
    
    public static FormattedCommand FormatDotnetNewList()
    {
        return new FormattedCommand(
            DOT_NET_CLI_TARGET_FILE_NAME,
            new[]
            {
                "new",
                "list",
            });
    }
    
    public static FormattedCommand FormatDotnetNewListDeprecated()
    {
        return new FormattedCommand(
            DOT_NET_CLI_TARGET_FILE_NAME,
            new[]
            {
                "new",
                "--list",
            });
    }
}