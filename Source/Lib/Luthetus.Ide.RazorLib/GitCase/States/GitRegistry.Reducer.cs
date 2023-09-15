using Fluxor;

namespace Luthetus.Ide.RazorLib.GitCase.States;

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