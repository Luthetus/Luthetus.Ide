using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTaskCase.Models;
using Luthetus.Common.RazorLib.KeyCase;

namespace Luthetus.Ide.RazorLib.CompilerServiceExplorerCase.States;

public partial class CompilerServiceExplorerState
{
    private class Reducer
    {
        [ReducerMethod]
        public CompilerServiceExplorerState ReduceSetCompilerServiceExplorerTreeViewTask(
            CompilerServiceExplorerState inState,
            SetCompilerServiceExplorerTreeViewTask inTask)
        {
            inTask.Sync.BackgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.Queue.Key,
                "SetDotNetSolutionAsync",
                async () => await inTask.Sync.SetCompilerServiceExplorerTreeView());

            return inState;
        }

        [ReducerMethod]
        public CompilerServiceExplorerState ReduceNewAction(
            CompilerServiceExplorerState inCompilerServiceExplorerState,
            NewAction newAction)
        {
            return newAction.NewFunc.Invoke(inCompilerServiceExplorerState);
        }
    }
}