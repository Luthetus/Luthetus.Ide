using Luthetus.Common.RazorLib.FileSystems.Models;

namespace Luthetus.CompilerServices.Lang.DotNetSolution;

public interface IDotNetProject
{
    public string DisplayName { get; }
    public Guid ProjectTypeGuid { get; }
    public string RelativePathFromSolutionFileString { get; }
    public Guid ProjectIdGuid { get; }
    public IAbsolutePath AbsolutePath { get; }
    public DotNetProjectKind DotNetProjectKind { get; }

    public void SetAbsolutePath(IAbsolutePath absolutePath);
}