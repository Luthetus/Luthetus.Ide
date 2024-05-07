namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

public static class LuthetusCommonBackgroundTaskServiceExtensionMethods
{
    public static LuthetusCommonBackgroundTaskServiceApi GetCommonApi(this IBackgroundTaskService backgroundTaskService)
    {
        return new LuthetusCommonBackgroundTaskServiceApi(backgroundTaskService);
    }
}