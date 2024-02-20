using Fluxor;

namespace Luthetus.Ide.Tests.Basis.Gits.States;

public class GitStateReducerTests
{
    private class Reducer
    {
        [ReducerMethod]
        public static GitState ReduceSetGitStateWithAction(
            GitState inGitState,
            SetGitStateWithAction setGitStateWithAction)
        {
            return setGitStateWithAction.GitStateWithFunc.Invoke(inGitState);
        }
    }
}