using Luthetus.Ide.ClassLib.FileSystem.Interfaces;

namespace Luthetus.Ide.ClassLib.CommandLine;

/// <summary>
/// Any values given will NOT be wrapped in quotes internally
/// </summary>
public static class DotNetCliFacts
{
    public const string DOT_NET_CLI_TARGET_FILE_NAME = "dotnet";

    public static (string targetFileName, IEnumerable<string> arguments) FormatStartProjectWithoutDebugging(
        IAbsoluteFilePath projectAbsoluteFilePath)
    {
        return FormatStartProjectWithoutDebugging(
            projectAbsoluteFilePath.GetAbsoluteFilePathString());
    }

    public static (string targetFileName, IEnumerable<string> arguments) FormatStartProjectWithoutDebugging(
        string projectPath)
    {
        return (DOT_NET_CLI_TARGET_FILE_NAME, new[]
        {
            "run",
            "--project",
            projectPath
        });
    }

    public static (string targetFileName, IEnumerable<string> arguments) FormatDotnetNewSln(
        string solutionName)
    {
        return (DOT_NET_CLI_TARGET_FILE_NAME, new[]
        {
            "new",
            "sln",
            "-o",
            solutionName
        });
    }

    public static (string targetFileName, IEnumerable<string> arguments) FormatDotnetNewCSharpProject(
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

        return (DOT_NET_CLI_TARGET_FILE_NAME, arguments);
    }

    public static (string targetFileName, IEnumerable<string> arguments) FormatAddExistingProjectToSolution(
        string solutionAbsoluteFilePathString,
        string cSharpProjectPath)
    {
        return (DOT_NET_CLI_TARGET_FILE_NAME, new[]
        {
            "sln",
            solutionAbsoluteFilePathString,
            "add",
            cSharpProjectPath
        });
    }

    public static (string targetFileName, IEnumerable<string> arguments) FormatRemoveCSharpProjectReferenceFromSolutionAction(
        string solutionAbsoluteFilePathString,
        string cSharpProjectAbsoluteFilePathString)
    {
        return (DOT_NET_CLI_TARGET_FILE_NAME, new[]
        {
            "sln",
            solutionAbsoluteFilePathString,
            "remove",
            cSharpProjectAbsoluteFilePathString
        });
    }

    public static (string targetFileName, IEnumerable<string> arguments) FormatAddNugetPackageReferenceToProject(
        string cSharpProjectAbsoluteFilePathString,
        string nugetPackageId,
        string nugetPackageVersion)
    {
        return (DOT_NET_CLI_TARGET_FILE_NAME, new[]
        {
            "add",
            cSharpProjectAbsoluteFilePathString,
            "package",
            nugetPackageId,
            "--version",
            nugetPackageVersion
        });
    }

    public static (string targetFileName, IEnumerable<string> arguments) FormatRemoveNugetPackageReferenceFromProject(
        string cSharpProjectAbsoluteFilePathString,
        string nugetPackageId)
    {
        return (DOT_NET_CLI_TARGET_FILE_NAME, new[]
        {
            "remove",
            cSharpProjectAbsoluteFilePathString,
            "package",
            nugetPackageId
        });
    }

    public static (string targetFileName, IEnumerable<string> arguments) FormatAddProjectToProjectReference(
        string receivingProjectAbsoluteFilePathString,
        string referenceProjectAbsoluteFilePathString)
    {
        return (DOT_NET_CLI_TARGET_FILE_NAME, new[]
        {
            "add",
            receivingProjectAbsoluteFilePathString,
            "reference",
            referenceProjectAbsoluteFilePathString
        });
    }

    public static (string targetFileName, IEnumerable<string> arguments) FormatRemoveProjectToProjectReference(
        string modifyProjectAbsoluteFilePathString,
        string referenceProjectAbsoluteFilePathString)
    {
        return (DOT_NET_CLI_TARGET_FILE_NAME, new[]
        {
            "remove",
            modifyProjectAbsoluteFilePathString,
            "reference",
            referenceProjectAbsoluteFilePathString
        });
    }

    public static (string targetFileName, IEnumerable<string> arguments) FormatMoveProjectToSolutionFolder(
        string solutionAbsoluteFilePathString,
        string projectToMoveAbsoluteFilePathString,
        string solutionFolderPath)
    {
        return (DOT_NET_CLI_TARGET_FILE_NAME, new[]
        {
            "sln",
            solutionAbsoluteFilePathString,
            "add",
            projectToMoveAbsoluteFilePathString,
            "--solution-folder",
            solutionFolderPath,
        });
    }
}