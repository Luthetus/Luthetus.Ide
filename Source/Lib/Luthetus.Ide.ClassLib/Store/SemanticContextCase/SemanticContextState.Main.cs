using Fluxor;
using Luthetus.CompilerServices.Lang.CSharp.SemanticContextCase.Implementations;

namespace Luthetus.Ide.ClassLib.Store.SemanticContextCase;

[FeatureState]
public partial class SemanticContextState
{
    private SemanticContextState()
    {
    }

    public SemanticContextState(DotNetSolutionSemanticContext dotNetSolutionSemanticContext)
    {
        DotNetSolutionSemanticContext = dotNetSolutionSemanticContext;
    }

    public DotNetSolutionSemanticContext? DotNetSolutionSemanticContext { get; }
}