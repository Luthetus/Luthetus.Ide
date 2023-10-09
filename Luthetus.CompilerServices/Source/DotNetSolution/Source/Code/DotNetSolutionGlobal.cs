using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.Code;

public record DotNetSolutionGlobal
{
    public DotNetSolutionGlobal(ImmutableArray<DotNetSolutionGlobalSection> dotNetSolutionGlobalSectionBag)
    {
        DotNetSolutionGlobalSectionBag = dotNetSolutionGlobalSectionBag;
    }

    public ImmutableArray<DotNetSolutionGlobalSection> DotNetSolutionGlobalSectionBag { get; init; }
    /// <summary>
    /// If one does not find the <see cref="Facts.LexSolutionFacts.Global.START_TOKEN"/> 
    /// then <see cref="WasFound"/> is to be false. Otherwise, upon finding the 
    /// <see cref="Facts.LexSolutionFacts.Global.START_TOKEN"/>, then <see cref="WasFound"/>
    /// should be set to true.
    /// </summary>
    public bool WasFound { get; init; }
}
