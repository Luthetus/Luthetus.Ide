using System.Collections.Concurrent;
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

    public bool __TaskCompletionSourceWasCreated { get; set; }

    private readonly ConcurrentQueue<ConfigWorkKind> _workKindQueue = new();

    private readonly BackgroundTaskService _backgroundTaskService;
    private readonly IIdeMainLayoutService _ideMainLayoutService;

    public void Enqueue(ConfigWorkKind configWorkKind)
	{
        _workKindQueue.Enqueue(configWorkKind);
        _backgroundTaskService.Continuous_EnqueueGroup(this);
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

	public ValueTask HandleEvent()
	{
		if (!_workKindQueue.TryDequeue(out ConfigWorkKind workKind))
			return ValueTask.CompletedTask;
			
		switch (workKind)
		{
			case ConfigWorkKind.InitializeFooterJustifyEndComponents:
				return Do_InitializeFooterJustifyEndComponents();
			default:
				Console.WriteLine($"{nameof(ConfigBackgroundTaskApi)} {nameof(HandleEvent)} default case");
				return ValueTask.CompletedTask;
		}
	}
}
