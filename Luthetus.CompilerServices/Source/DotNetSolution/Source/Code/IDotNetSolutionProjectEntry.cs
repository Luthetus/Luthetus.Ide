using Luthetus.Common.RazorLib.FileSystems.Models;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.Code;

public interface IDotNetSolutionProjectEntry
{
    public string DisplayName { get; init; }
    public Guid ProjectTypeGuid { get; init; }
    public string RelativePathFromSolutionFileString { get; init; }
    public Guid ProjectIdGuid { get; init; }
    public IAbsolutePath AbsolutePath { get; init; }
    public DotNetProjectKind DotNetProjectKind { get; init; }
}
