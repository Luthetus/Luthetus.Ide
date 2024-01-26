using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.Shareds.Models;

namespace Luthetus.Ide.RazorLib.Shareds.Displays.Internals;

public partial class ActiveBackgroundTaskDisplay : IDisposable
{
    [Inject]
    public IBackgroundTaskService BackgroundTaskService { get; set; } = null!;
    [Inject]
    public IDialogService DialogService { get; set; } = null!;

    private BackgroundTaskQueue _continuousBackgroundTaskWorker = null!;

    //private readonly IThrottle _executingBackgroundTaskChangedThrottle = new Throttle(TimeSpan.FromMilliseconds(30));

    private readonly List<IBackgroundTask> _seenBackgroundTasks = new List<IBackgroundTask>();
    private BackgroundTaskDialogModel _backgroundTaskDialogModel = null!;

    protected override void OnInitialized()
    {
        _backgroundTaskDialogModel = new(_seenBackgroundTasks);

        _continuousBackgroundTaskWorker = BackgroundTaskService.GetQueue(ContinuousBackgroundTaskWorker.GetQueueKey());
        _continuousBackgroundTaskWorker.ExecutingBackgroundTaskChanged += ContinuousBackgroundTaskWorker_ExecutingBackgroundTaskChanged;

        base.OnInitialized();
    }

    private void ShowBackgroundTaskDialogOnClick()
    {
        DialogService.RegisterDialogRecord(new DialogRecord(
            Key<DialogRecord>.NewKey(),
            "Background Tasks",
            typeof(BackgroundTaskDialogDisplay),
            new Dictionary<string, object?>
            {
                {
                    nameof(BackgroundTaskDialogDisplay.BackgroundTaskDialogModel),
                    _backgroundTaskDialogModel
                }
            },
            null)
        {
            IsResizable = true
        });
    }

    private async void ContinuousBackgroundTaskWorker_ExecutingBackgroundTaskChanged()
    {
        var executingBackgroundTask = _continuousBackgroundTaskWorker.ExecutingBackgroundTask;

        if (executingBackgroundTask is not null)
            _seenBackgroundTasks.Add(executingBackgroundTask);

        await InvokeAsync(StateHasChanged);

        if (_backgroundTaskDialogModel.ReRenderFuncAsync is not null)
            await _backgroundTaskDialogModel.ReRenderFuncAsync.Invoke();
    }

    public void Dispose()
    {
        _continuousBackgroundTaskWorker.ExecutingBackgroundTaskChanged -= ContinuousBackgroundTaskWorker_ExecutingBackgroundTaskChanged;
    }
}