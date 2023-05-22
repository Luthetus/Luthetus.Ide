using Fluxor;

namespace Luthetus.Ide.ClassLib.Store.GitCase;

public partial record GitState
{
    private class Reducer
    {
        [ReducerMethod]
        public static GitState ReduceSetGitStateWithAction(
            GitState inGitState,
            SetGitStateWithAction setGitStateWithAction)
        {
            return setGitStateWithAction.GitStateWithFunc
                .Invoke(inGitState);
        }
    }
}

