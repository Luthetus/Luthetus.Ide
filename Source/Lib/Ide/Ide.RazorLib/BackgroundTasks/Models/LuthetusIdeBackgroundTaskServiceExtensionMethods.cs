using Luthetus.Common.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Ide.RazorLib.BackgroundTasks.Models;

public static class LuthetusIdeBackgroundTaskServiceExtensionMethods
{
    public static LuthetusIdeBackgroundTaskApi GetIdeApi(this IBackgroundTaskService backgroundTaskService)
    {
        return new LuthetusIdeBackgroundTaskServiceApi(backgroundTaskService);
    }
}