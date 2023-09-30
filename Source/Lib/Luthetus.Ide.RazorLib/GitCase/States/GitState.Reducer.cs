using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTaskCase.Models;
using Luthetus.Common.RazorLib.KeyCase.Models;

namespace Luthetus.Ide.RazorLib.GitCase.States;

public partial record GitState
{
    private class Reducer
    {
        [ReducerMethod]
        public static GitState ReduceSetGitStateWithAction(
            GitState inGitState,
            SetGitStateWithAction setGitStateWithAction)
        {
            return setGitStateWithAction.GitStateWithFunc.Invoke(
                inGitState);
        }
    }
}