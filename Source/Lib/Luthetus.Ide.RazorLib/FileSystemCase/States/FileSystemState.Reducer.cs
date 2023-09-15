using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTaskCase.Models;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;

namespace Luthetus.Ide.RazorLib.FileSystemCase.States;

public partial class FileSystemState
{
    [ReducerMethod]
    public static FileSystemState ReduceSaveFileAction(
        FileSystemState inFileSystemState,
        SaveFileAction saveFileAction)
    {
        saveFileAction.Sync.BackgroundTaskService.Enqueue(BackgroundTaskKey.NewKey(), ContinuousBackgroundTaskWorker.Queue.Key,
            "Handle Save File Action",
            async () => await saveFileAction.Sync.SaveFile(saveFileAction));

        return inFileSystemState;
    }
}