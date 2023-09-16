using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTaskCase.Models;
using Luthetus.Common.RazorLib.KeyCase;

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
        
        [ReducerMethod]
        public static GitState ReduceRefreshGitTask(
            GitState inState,
            RefreshGitTask inTask)
        {
            inTask.Sync.BackgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.Queue.Key,
                "RefreshGit",
                async () => await inTask.Sync.RefreshGit(inTask));

            return inState;
        }
        
        [ReducerMethod]
        public static GitState ReduceGitInitTask(
            GitState inState,
            GitInitTask inTask)
        {
            inTask.Sync.BackgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.Queue.Key,
                "GitInit",
                async () => await inTask.Sync.GitInit(inTask));

            return inState;
        }
        
        [ReducerMethod]
        public static GitState ReduceTryFindGitFolderInDirectoryTask(
            GitState inState,
            TryFindGitFolderInDirectoryTask inTask)
        {
            inTask.Sync.BackgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.Queue.Key,
            "TryFindGitFolderInDirectory",
            async () => await inTask.Sync.TryFindGitFolderInDirectory(inTask));

            return inState;
        }
    }
}