using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Ide.RazorLib.Shareds.Displays;

public partial class ActiveBackgroundTaskDisplay : IDisposable
{
    [Inject]
    public IBackgroundTaskService BackgroundTaskService { get; set; } = null!;
    [Inject]
    public IDialogService DialogService { get; set; } = null!;

    private BackgroundTaskQueue _continuousBackgroundTaskWorker = null!;

    //private readonly IThrottle _executingBackgroundTaskChangedThrottle = new Throttle(TimeSpan.FromMilliseconds(30));

    private readonly List<IBackgroundTask> _seenBackgroundTasks = new List<IBackgroundTask>();

    protected override void OnInitialized()
    {
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
                    nameof(BackgroundTaskDialogDisplay.SeenBackgroundTasks),
                    _seenBackgroundTasks.ToArray()
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
    }

    public void Dispose()
    {
        _continuousBackgroundTaskWorker.ExecutingBackgroundTaskChanged -= ContinuousBackgroundTaskWorker_ExecutingBackgroundTaskChanged;
    }
}