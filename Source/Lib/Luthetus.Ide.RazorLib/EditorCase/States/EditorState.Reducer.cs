using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTaskCase.Models;

namespace Luthetus.Ide.RazorLib.EditorCase.States;

public partial class EditorState
{
    [ReducerMethod]
    public static EditorState ReduceShowInputFileAction(
        EditorState inState,
        ShowInputFileAction inTask)
    {
        inTask.Sync.BackgroundTaskService.Enqueue(BackgroundTaskKey.NewKey(), ContinuousBackgroundTaskWorker.Queue.Key,
            "ShowInputFile",
            async () => await inTask.Sync.ShowInputFile(inTask));

        return inState;
    }
    
    [ReducerMethod]
    public static EditorState ReduceOpenInEditorAction(
        EditorState inState,
        OpenInEditorAction inTask)
    {
        inTask.Sync.BackgroundTaskService.Enqueue(BackgroundTaskKey.NewKey(), ContinuousBackgroundTaskWorker.Queue.Key,
            "OpenInEditor",
            async () => await inTask.Sync.OpenInEditor(inTask));

        return inState;
    }
}