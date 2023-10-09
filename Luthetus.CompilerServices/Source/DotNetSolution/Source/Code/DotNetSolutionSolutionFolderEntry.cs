using Luthetus.Common.RazorLib.FileSystems.Models;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.Code;

public record DotNetSolutionSolutionFolderEntry : IDotNetSolutionProjectEntry
{
    public DotNetSolutionSolutionFolderEntry(
        string displayName,
        Guid projectTypeGuid,
        string relativePathFromSolutionFileString,
        Guid projectIdGuid,
        IAbsolutePath absolutePath)
    {
        DisplayName = displayName;
        ProjectTypeGuid = projectTypeGuid;
        RelativePathFromSolutionFileString = relativePathFromSolutionFileString;
        ProjectIdGuid = projectIdGuid;
        AbsolutePath = absolutePath;
    }

    public string DisplayName { get; init; }
    public Guid ProjectTypeGuid { get; init; }
    public string RelativePathFromSolutionFileString { get; init; }
    public Guid ProjectIdGuid { get; init; }
    public IAbsolutePath AbsolutePath { get; init; }

    public DotNetProjectKind DotNetProjectKind => DotNetProjectKind.SolutionFolder;
}
