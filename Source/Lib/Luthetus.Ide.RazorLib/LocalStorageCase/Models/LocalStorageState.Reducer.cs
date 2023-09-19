using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTaskCase.Models;
using Luthetus.Common.RazorLib.KeyCase.Models;

namespace Luthetus.Ide.RazorLib.LocalStorageCase.Models;

public partial class LocalStorageState
{
    private class Reducer
    {
        [ReducerMethod]
        public static LocalStorageState ReduceLocalStorageSetItemTask(
            LocalStorageState inState,
            LocalStorageSetItemTask inTask)
        {
            inTask.Sync.BackgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.Queue.Key,
                "RefreshGit",
                async () => await inTask.Sync.LocalStorageSetItem(inTask));

            return inState;
        }
    }
}
