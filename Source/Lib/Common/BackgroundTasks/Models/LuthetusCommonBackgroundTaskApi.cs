using Luthetus.Common.RazorLib.Storages.Models;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

public class LuthetusCommonBackgroundTaskApi
{
    private readonly IBackgroundTaskService _backgroundTaskService;

    public LuthetusCommonBackgroundTaskApi(IBackgroundTaskService backgroundTaskService)
    {
        _backgroundTaskService = backgroundTaskService;

        Storage = new LuthetusCommonStorageBackgroundTaskApi(_backgroundTaskService);
    }

    public LuthetusCommonStorageBackgroundTaskApi Storage { get; }
}
