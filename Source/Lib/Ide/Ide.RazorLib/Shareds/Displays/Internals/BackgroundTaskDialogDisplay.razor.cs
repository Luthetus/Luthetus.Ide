/*using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Reactives.Models;

namespace Luthetus.Ide.RazorLib.Shareds.Displays.Internals;

public partial class BackgroundTaskDialogDisplay : ComponentBase, IDisposable
{
    [Inject]
    public BackgroundTaskService BackgroundTaskService { get; set; } = null!;

    private readonly Throttle _executingBackgroundTaskChangedThrottle = new(TimeSpan.FromMilliseconds(1000));
    private readonly List<IBackgroundTask> _seenBackgroundTasks = new List<IBackgroundTask>();
    private readonly object _seenBackgroundTasksLock = new();

	private int _countTracked = 100;
	private int _clearTo = 15;

    protected override void OnInitialized()
    {
        BackgroundTaskService.ContinuousTaskWorker.ExecutingBackgroundTaskChanged += ContinuousBackgroundTaskWorker_ExecutingBackgroundTaskChanged;

        base.OnInitialized();
    }

    private void ContinuousBackgroundTaskWorker_ExecutingBackgroundTaskChanged()
    {
        var executingBackgroundTask = BackgroundTaskService.ContinuousTaskWorker.ExecutingBackgroundTask;
        
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

        _executingBackgroundTaskChangedThrottle.Run(async _ =>
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
        BackgroundTaskService.ContinuousTaskWorker.ExecutingBackgroundTaskChanged -= ContinuousBackgroundTaskWorker_ExecutingBackgroundTaskChanged;
    }
}*/