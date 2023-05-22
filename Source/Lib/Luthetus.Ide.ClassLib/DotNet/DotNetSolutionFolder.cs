using Luthetus.Ide.ClassLib.FileSystem.Interfaces;

namespace Luthetus.Ide.ClassLib.DotNet;

public class DotNetSolutionFolder : IDotNetProject
{
    public static readonly Guid SolutionFolderProjectTypeGuid = Guid.Parse("2150E333-8FDC-42A3-9474-1A3956D46DE8");

    public DotNetSolutionFolder(
        string displayName,
        Guid projectTypeGuid,
        string relativePathFromSolutionFileString,
        Guid projectIdGuid)
    {
        DisplayName = displayName;
        ProjectTypeGuid = projectTypeGuid;
        RelativePathFromSolutionFileString = relativePathFromSolutionFileString;
        ProjectIdGuid = projectIdGuid;
    }

    public string DisplayName { get; }
    public Guid ProjectTypeGuid { get; }
    public string RelativePathFromSolutionFileString { get; }
    public Guid ProjectIdGuid { get; }
    public IAbsoluteFilePath AbsoluteFilePath { get; private set; }
    public DotNetProjectKind DotNetProjectKind => DotNetProjectKind.SolutionFolder;

    /// <summary>
    /// TODO: This should just done from within the constructor? That is to say in the constructor for CSharpProjects calculate the absolute path from its relative path from the .sln
    /// </summary>
    public void SetAbsoluteFilePath(IAbsoluteFilePath absoluteFilePath)
    {
        AbsoluteFilePath = absoluteFilePath;
    }
}