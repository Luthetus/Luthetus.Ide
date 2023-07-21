namespace Luthetus.Ide.ClassLib.Store.SemanticContextCase;

public partial class SemanticContextState
{
    private class Reducer
    {
        [ReducerMethod]
        public static SemanticContextState ReduceSetDotNetSolutionSemanticContext(
            SemanticContextState inSemanticContextState,
            SetDotNetSolutionSemanticContextAction setDotNetSolutionSemanticContextAction)
        {
            return new SemanticContextState(
                setDotNetSolutionSemanticContextAction.DotNetSolutionSemanticContext);
        }
    }
}