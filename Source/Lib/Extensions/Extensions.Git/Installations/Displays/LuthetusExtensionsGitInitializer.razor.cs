using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.Panels.States;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Extensions.Git.Displays;

namespace Luthetus.Extensions.Git.Installations.Displays;

public partial class LuthetusExtensionsGitInitializer : ComponentBase
{
	[Inject]
	private IState<PanelState> PanelStateWrap { get; set; } = null!;
	[Inject]
	private IBackgroundTaskService BackgroundTaskService { get; set; } = null!;
	[Inject]
	private IDialogService DialogService { get; set; } = null!;
	[Inject]
	private IDispatcher Dispatcher { get; set; } = null!;
	[Inject]
	private IJSRuntime JsRuntime { get; set; } = null!;

	protected override void OnInitialized()
	{
		BackgroundTaskService.Enqueue(
            Key<IBackgroundTask>.NewKey(),
            ContinuousBackgroundTaskWorker.GetQueueKey(),
            nameof(LuthetusExtensionsGitInitializer),
            () =>
            {
                InitializePanelTabs();
                return Task.CompletedTask;
            });
            
        base.OnInitialized();
	}

    private void InitializePanelTabs()
    {
        InitializeLeftPanelTabs();
    }

    private void InitializeLeftPanelTabs()
    {
        var leftPanel = PanelFacts.GetTopLeftPanelGroup(PanelStateWrap.Value);
        leftPanel.Dispatcher = Dispatcher;

        // gitPanel
        var gitPanel = new Panel(
            "Git Changes",
            Key<Panel>.NewKey(),
            Key<IDynamicViewModel>.NewKey(),
            ContextFacts.GitContext.ContextKey,
            typeof(GitDisplay),
            null,
            Dispatcher,
            DialogService,
            JsRuntime);
        Dispatcher.Dispatch(new PanelState.RegisterPanelAction(gitPanel));
        Dispatcher.Dispatch(new PanelState.RegisterPanelTabAction(leftPanel.Key, gitPanel, false));
    }
}
