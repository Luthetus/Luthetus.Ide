using Fluxor;

namespace Luthetus.Ide.RazorLib.GitCase;

public partial record GitRegistry
{
    private class Reducer
    {
        [ReducerMethod]
        public static GitRegistry ReduceSetGitStateWithAction(
            GitRegistry inGitState,
            SetGitStateWithAction setGitStateWithAction)
        {
            return setGitStateWithAction.GitStateWithFunc
                .Invoke(inGitState);
        }
    }
}