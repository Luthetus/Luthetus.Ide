using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.Code;

public record DotNetSolutionGlobal
{
    public DotNetSolutionGlobal(ImmutableArray<DotNetSolutionGlobalSection> dotNetSolutionGlobalSectionBag)
    {
        DotNetSolutionGlobalSectionBag = dotNetSolutionGlobalSectionBag;
    }

    public ImmutableArray<DotNetSolutionGlobalSection> DotNetSolutionGlobalSectionBag { get; init; }
}
