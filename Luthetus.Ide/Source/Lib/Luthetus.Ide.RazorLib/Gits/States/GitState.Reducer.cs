using Fluxor;

namespace Luthetus.Ide.RazorLib.Gits.States;

public partial record GitState
{
    public class Reducer
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