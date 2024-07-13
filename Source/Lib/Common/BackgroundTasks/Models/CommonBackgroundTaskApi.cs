using Luthetus.Common.RazorLib.Storages.Models;

namespace Luthetus.Common.RazorLib.BackgroundTasks.Models;

public class CommonBackgroundTaskApi
{
    private readonly IBackgroundTaskService _backgroundTaskService;
    private readonly IStorageService _storageService;

    public CommonBackgroundTaskApi(
        IBackgroundTaskService backgroundTaskService,
        IStorageService storageService)
    {
        _backgroundTaskService = backgroundTaskService;
        _storageService = storageService;

        Storage = new StorageCommonApi(
            _backgroundTaskService,
            _storageService);
    }

    public StorageCommonApi Storage { get; }
}
