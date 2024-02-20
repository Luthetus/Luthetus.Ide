using Luthetus.Common.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Ide.Tests.Basis.Shareds.Models;

public class BackgroundTaskDialogModelTests
{
    public BackgroundTaskDialogModel(List<IBackgroundTask> seenBackgroundTasks)
    {
        SeenBackgroundTasks = seenBackgroundTasks;
    }

    public List<IBackgroundTask> SeenBackgroundTasks { get; }
    public Func<Task>? ReRenderFuncAsync { get; set; }
}
