using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
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
    private LuthetusCommonApi CommonApi { get; set; } = null!;
	[Inject]
	private IJSRuntime JsRuntime { get; set; } = null!;

	protected override void OnInitialized()
	{
        CommonApi.BackgroundTaskApi.Enqueue(
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
        var leftPanel = PanelFacts.GetTopLeftPanelGroup(CommonApi.PanelApi.GetPanelState());
        leftPanel.PanelService = CommonApi.PanelApi;

        // gitPanel
        var gitPanel = new Panel(
            "Git Changes",
            Key<Panel>.NewKey(),
            Key<IDynamicViewModel>.NewKey(),
            ContextFacts.GitContext.ContextKey,
            typeof(GitDisplay),
            null,
            CommonApi,
            JsRuntime);
        CommonApi.PanelApi.ReduceRegisterPanelAction(gitPanel);
        CommonApi.PanelApi.ReduceRegisterPanelTabAction(leftPanel.Key, gitPanel, false);
    }
}
