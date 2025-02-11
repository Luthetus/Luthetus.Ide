using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Extensions.Git.Displays;

namespace Luthetus.Extensions.Git.Installations.Displays;

public partial class LuthetusExtensionsGitInitializer : ComponentBase
{
	[Inject]
	private IPanelService PanelService { get; set; } = null!;
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
            BackgroundTaskFacts.ContinuousQueueKey,
            nameof(LuthetusExtensionsGitInitializer),
            () =>
            {
                InitializePanelTabs();
                return ValueTask.CompletedTask;
            });
            
        base.OnInitialized();
	}

    private void InitializePanelTabs()
    {
        InitializeLeftPanelTabs();
    }

    private void InitializeLeftPanelTabs()
    {
        var leftPanel = PanelFacts.GetTopLeftPanelGroup(PanelService.GetPanelState());
        leftPanel.PanelService = PanelService;

        // gitPanel
        var gitPanel = new Panel(
            "Git Changes",
            Key<Panel>.NewKey(),
            Key<IDynamicViewModel>.NewKey(),
            ContextFacts.GitContext.ContextKey,
            typeof(GitDisplay),
            null,
            PanelService,
            DialogService,
            JsRuntime);
        PanelService.ReduceRegisterPanelAction(gitPanel);
        PanelService.ReduceRegisterPanelTabAction(leftPanel.Key, gitPanel, false);
    }
}
