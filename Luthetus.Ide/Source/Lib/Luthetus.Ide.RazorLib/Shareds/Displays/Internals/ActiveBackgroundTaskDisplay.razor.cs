using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Ide.RazorLib.Shareds.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;

namespace Luthetus.Ide.RazorLib.Shareds.Displays.Internals;

public partial class ActiveBackgroundTaskDisplay : IDisposable
{
    [Inject]
    public IBackgroundTaskService BackgroundTaskService { get; set; } = null!;
    [Inject]
    public IDialogService DialogService { get; set; } = null!;

	private static readonly Key<IDynamicViewModel> _backgroundTaskDialogKey = Key<IDynamicViewModel>.NewKey();

    private readonly IThrottle _executingBackgroundTaskChangedThrottle = new Throttle(TimeSpan.FromMilliseconds(1000));
    private readonly List<IBackgroundTask> _seenBackgroundTasks = new List<IBackgroundTask>();

    private BackgroundTaskQueue _continuousBackgroundTaskWorker = null!;
    private BackgroundTaskDialogModel _backgroundTaskDialogModel = null!;

    protected override void OnInitialized()
    {
        _backgroundTaskDialogModel = new(_seenBackgroundTasks);

        _continuousBackgroundTaskWorker = BackgroundTaskService.GetQueue(ContinuousBackgroundTaskWorker.GetQueueKey());
        _continuousBackgroundTaskWorker.ExecutingBackgroundTaskChanged += ContinuousBackgroundTaskWorker_ExecutingBackgroundTaskChanged;

        base.OnInitialized();
    }

    public void ShowBackgroundTaskDialogOnClick()
    {
        DialogService.RegisterDialogRecord(new DialogViewModel(
            _backgroundTaskDialogKey,
            "Background Tasks",
            typeof(BackgroundTaskDialogDisplay),
            new Dictionary<string, object?>
            {
                {
                    nameof(BackgroundTaskDialogDisplay.BackgroundTaskDialogModel),
                    _backgroundTaskDialogModel
                }
            },
            null,
			true));
    }

    private async void ContinuousBackgroundTaskWorker_ExecutingBackgroundTaskChanged()
    {
        var executingBackgroundTask = _continuousBackgroundTaskWorker.ExecutingBackgroundTask;
        
        if (executingBackgroundTask is not null)
        {
            if (_seenBackgroundTasks.Count > 1000)
            {
                var lastFifty = _seenBackgroundTasks.TakeLast(50).ToList();
                _seenBackgroundTasks.Clear();
                _seenBackgroundTasks.AddRange(lastFifty);
            }

            _seenBackgroundTasks.Add(executingBackgroundTask);
        }

        _executingBackgroundTaskChangedThrottle.PushEvent(async _ =>
        {
            await InvokeAsync(StateHasChanged);
        });

        if (_backgroundTaskDialogModel.ReRenderFuncAsync is not null)
            await _backgroundTaskDialogModel.ReRenderFuncAsync.Invoke();
    }

    public void Dispose()
    {
        _continuousBackgroundTaskWorker.ExecutingBackgroundTaskChanged -= ContinuousBackgroundTaskWorker_ExecutingBackgroundTaskChanged;
    }
}