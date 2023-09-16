using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTaskCase.Models;
using Luthetus.Common.RazorLib.KeyCase;

namespace Luthetus.Ide.RazorLib.FileSystemCase.States;

public partial class FileSystemState
{
    [ReducerMethod]
    public static FileSystemState ReduceSaveFileAction(
        FileSystemState inFileSystemState,
        SaveFileAction saveFileAction)
    {
        saveFileAction.Sync.BackgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.Queue.Key,
            "Handle Save File Action",
            async () => await saveFileAction.Sync.SaveFile(saveFileAction));

        return inFileSystemState;
    }
}