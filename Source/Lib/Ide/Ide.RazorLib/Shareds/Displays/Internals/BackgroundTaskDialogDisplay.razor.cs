using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Ide.RazorLib.Shareds.Models;

namespace Luthetus.Ide.RazorLib.Shareds.Displays.Internals;

public partial class BackgroundTaskDialogDisplay : ComponentBase, IDisposable
{
    [Inject]
    public IBackgroundTaskService BackgroundTaskService { get; set; } = null!;

    private readonly ThrottleAsync _executingBackgroundTaskChangedThrottle = new ThrottleAsync(TimeSpan.FromMilliseconds(1000));
    private readonly List<IBackgroundTask> _seenBackgroundTasks = new List<IBackgroundTask>();
    private readonly object _seenBackgroundTasksLock = new();

    private BackgroundTaskQueue _continuousBackgroundTaskWorker = null!;
	private int _countTracked = 100;
	private int _clearTo = 15;

    protected override void OnInitialized()
    {
        _continuousBackgroundTaskWorker = (BackgroundTaskQueue)BackgroundTaskService.GetQueue(ContinuousBackgroundTaskWorker.GetQueueKey());
        _continuousBackgroundTaskWorker.ExecutingBackgroundTaskChanged += ContinuousBackgroundTaskWorker_ExecutingBackgroundTaskChanged;

        base.OnInitialized();
    }

    private async void ContinuousBackgroundTaskWorker_ExecutingBackgroundTaskChanged()
    {
        var executingBackgroundTask = _continuousBackgroundTaskWorker.ExecutingBackgroundTask;
        
        if (executingBackgroundTask is not null)
        {
			lock (_seenBackgroundTasksLock)
			{
	            if (_seenBackgroundTasks.Count > _countTracked)
	            {
	                var lastFifty = _seenBackgroundTasks.TakeLast(15).ToList();
	                _seenBackgroundTasks.Clear();
	                _seenBackgroundTasks.AddRange(lastFifty);
	            }
	
	            _seenBackgroundTasks.Add(executingBackgroundTask);
			}
        }

        await _executingBackgroundTaskChangedThrottle.PushEvent(async _ =>
        {
            await InvokeAsync(StateHasChanged);
        });
    }

	private List<IBackgroundTask> GetThreadSafeCopyOfSeenBackgroundTasks()
	{
		lock (_seenBackgroundTasksLock)
		{
			return new List<IBackgroundTask>(_seenBackgroundTasks);
		}
	}

    public void Dispose()
    {
        _continuousBackgroundTaskWorker.ExecutingBackgroundTaskChanged -= ContinuousBackgroundTaskWorker_ExecutingBackgroundTaskChanged;
    }
}