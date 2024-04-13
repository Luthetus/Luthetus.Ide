using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Storages.Models;

namespace Luthetus.Common.RazorLib.Storages.States;

public partial class StorageSync
{
    private readonly IStorageService _storageService;

    public StorageSync(
        IStorageService storageService,
        IBackgroundTaskService backgroundTaskService,
        IDispatcher dispatcher)
    {
        _storageService = storageService;

        BackgroundTaskService = backgroundTaskService;
        Dispatcher = dispatcher;
    }

    public IBackgroundTaskService BackgroundTaskService { get; }
    public IDispatcher Dispatcher { get; }
}