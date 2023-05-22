using Luthetus.Ide.ClassLib.FileSystem.Interfaces;

namespace Luthetus.Ide.ClassLib.DotNet.CSharp;

public class CSharpProject : IDotNetProject
{
    public CSharpProject(
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
    public DotNetProjectKind DotNetProjectKind => DotNetProjectKind.CSharpProject;

    /// <summary>
    /// TODO: This should just done from within the constructor? That is to say in the constructor for CSharpProjects calculate the absolute path from its relative path from the .sln
    /// </summary>
    public void SetAbsoluteFilePath(IAbsoluteFilePath absoluteFilePath)
    {
        AbsoluteFilePath = absoluteFilePath;
    }
}