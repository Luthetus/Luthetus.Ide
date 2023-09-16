using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTaskCase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            inTask.Sync.BackgroundTaskService.Enqueue(BackgroundTaskKey.NewKey(), ContinuousBackgroundTaskWorker.Queue.Key,
                "RefreshGit",
                async () => await inTask.Sync.LocalStorageSetItem(inTask));

            return inState;
        }
    }
}
