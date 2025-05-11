using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Notifications.Displays;
using Luthetus.TextEditor.RazorLib.Edits.Displays;
using Luthetus.Ide.RazorLib.Shareds.Models;
using Luthetus.Extensions.Git.Displays;

namespace Luthetus.Extensions.Config.BackgroundTasks.Models;

public class ConfigBackgroundTaskApi : IBackgroundTaskGroup
{
    public ConfigBackgroundTaskApi(
		BackgroundTaskService backgroundTaskService,
		IIdeMainLayoutService ideMainLayoutService)
    {
        _backgroundTaskService = backgroundTaskService;
        _ideMainLayoutService = ideMainLayoutService;
    }

    public Key<IBackgroundTaskGroup> BackgroundTaskKey { get; } = Key<IBackgroundTaskGroup>.NewKey();
    public Key<BackgroundTaskQueue> QueueKey { get; } = BackgroundTaskFacts.ContinuousQueueKey;
    public string Name { get; } = nameof(ConfigBackgroundTaskApi);
    public bool EarlyBatchEnabled { get; } = false;

    public bool __TaskCompletionSourceWasCreated { get; set; }

    private readonly Queue<ConfigWorkKind> _workKindQueue = new();
    private readonly object _workLock = new();

    private readonly BackgroundTaskService _backgroundTaskService;
    private readonly IIdeMainLayoutService _ideMainLayoutService;

    public void Enqueue_InitializeFooterJustifyEndComponents()
	{
		lock (_workLock)
        {
            _workKindQueue.Enqueue(ConfigWorkKind.InitializeFooterJustifyEndComponents);
            _backgroundTaskService.EnqueueGroup(this);
        }
	}

    public ValueTask Do_InitializeFooterJustifyEndComponents()
    {
        /*_ideMainLayoutService.RegisterFooterJustifyEndComponent(
            new FooterJustifyEndComponent(
                Key<FooterJustifyEndComponent>.NewKey(),
                typeof(GitInteractiveIconDisplay),
                new Dictionary<string, object?>
                {
                    {
                        nameof(GitInteractiveIconDisplay.CssStyleString),
                        "margin-right: 15px;"
                    }
                }));*/

        _ideMainLayoutService.RegisterFooterJustifyEndComponent(
            new FooterJustifyEndComponent(
                Key<FooterJustifyEndComponent>.NewKey(),
                typeof(DirtyResourceUriInteractiveIconDisplay),
                new Dictionary<string, object?>
                {
                    {
                        nameof(GitInteractiveIconDisplay.CssStyleString),
                        "margin-right: 15px;"
                    }
                }));

        _ideMainLayoutService.RegisterFooterJustifyEndComponent(
            new FooterJustifyEndComponent(
                Key<FooterJustifyEndComponent>.NewKey(),
                typeof(NotificationsInteractiveIconDisplay),
                ComponentParameterMap: null));

        return ValueTask.CompletedTask;
    }

    public IBackgroundTaskGroup? EarlyBatchOrDefault(IBackgroundTaskGroup oldEvent)
	{
		return null;
	}
	
	public ValueTask HandleEvent(CancellationToken cancellationToken)
	{
		ConfigWorkKind workKind;
		
		lock (_workLock)
		{
			if (!_workKindQueue.TryDequeue(out workKind))
				return ValueTask.CompletedTask;
		}
			
		switch (workKind)
		{
			case ConfigWorkKind.InitializeFooterJustifyEndComponents:
			{
				return Do_InitializeFooterJustifyEndComponents();
			}
			default:
			{
				Console.WriteLine($"{nameof(ConfigBackgroundTaskApi)} {nameof(HandleEvent)} default case");
				return ValueTask.CompletedTask;
			}
		}
	}
}
