using Luthetus.Common.RazorLib.Storages.Models;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

public class LuthetusCommonBackgroundTaskApi
{
    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly IStorageService _storageService;

    public LuthetusCommonBackgroundTaskApi(
        IBackgroundTaskService backgroundTaskService,
        IStorageService storageService)
    {
        _backgroundTaskService = backgroundTaskService;
        _storageService = storageService;

        Storage = new LuthetusCommonStorageBackgroundTaskApi(
            _backgroundTaskService,
            _storageService);
    }

    public LuthetusCommonStorageBackgroundTaskApi Storage { get; }
}
