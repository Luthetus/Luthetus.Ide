using Luthetus.CompilerServices.Lang.CSharp.SemanticContextCase.Implementations;

namespace Luthetus.Ide.ClassLib.Store.SemanticContextCase;

public partial class SemanticContextState
{
    public record SetDotNetSolutionSemanticContextAction(DotNetSolutionSemanticContext DotNetSolutionSemanticContext);
}