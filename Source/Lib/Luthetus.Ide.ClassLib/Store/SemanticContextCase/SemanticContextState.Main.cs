using Fluxor;
using Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.SemanticContextCase.Implementations;

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
