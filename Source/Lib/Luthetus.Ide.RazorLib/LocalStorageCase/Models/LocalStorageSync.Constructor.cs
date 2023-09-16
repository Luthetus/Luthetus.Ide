using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTaskCase.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Luthetus.Ide.RazorLib.LocalStorageCase.Models;

public partial class LocalStorageSync
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