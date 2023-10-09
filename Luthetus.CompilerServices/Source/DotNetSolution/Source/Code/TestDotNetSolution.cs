using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.Code;

public record TestDotNetSolution
{
    public TestDotNetSolution(
        DotNetSolutionHeader dotNetSolutionHeader,
        ImmutableArray<DotNetSolutionProjectEntry> dotNetSolutionProjectEntryBag,
        DotNetSolutionGlobal dotNetSolutionGlobal)
    {
        DotNetSolutionHeader = dotNetSolutionHeader;
        DotNetSolutionProjectEntryBag = dotNetSolutionProjectEntryBag;
        DotNetSolutionGlobal = dotNetSolutionGlobal;
    }

    public DotNetSolutionHeader DotNetSolutionHeader { get; init; }
    public ImmutableArray<DotNetSolutionProjectEntry> DotNetSolutionProjectEntryBag { get; init; }
    public DotNetSolutionGlobal DotNetSolutionGlobal { get; init; }
}
