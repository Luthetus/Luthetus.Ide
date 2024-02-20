using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Microsoft.JSInterop;

namespace Luthetus.Ide.Tests.Basis.LocalStorages.Models;

public partial class LocalStorageSyncConstructorTests
{
    private readonly IJSRuntime _jsRuntime;

    public LocalStorageSync(
        IJSRuntime jsRuntime,
        IBackgroundTaskService backgroundTaskService,
        IDispatcher dispatcher)
    {
        _jsRuntime = jsRuntime;
        
        BackgroundTaskService = backgroundTaskService;
        Dispatcher = dispatcher;
    }

    public IBackgroundTaskService BackgroundTaskService { get; }
    public IDispatcher Dispatcher { get; }
}