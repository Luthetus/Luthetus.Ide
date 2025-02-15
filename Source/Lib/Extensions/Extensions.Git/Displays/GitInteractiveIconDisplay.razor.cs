using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Extensions.Git.Models;
using Luthetus.Extensions.Git.BackgroundTasks.Models;

namespace Luthetus.Extensions.Git.Displays;

public partial class GitInteractiveIconDisplay : ComponentBase, IDisposable
{
    [Inject]
    private GitBackgroundTaskApi GitBackgroundTaskApi { get; set; } = null!;
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;
    [Inject]
    private IPanelService PanelService { get; set; } = null!;
    [Inject]
    private IDialogService DialogService { get; set; } = null!;

    [Parameter]
    public string CssClassString { get; set; } = string.Empty;
    [Parameter]
    public string CssStyleString { get; set; } = string.Empty;
    
    protected override void OnInitialized()
    {
		GitBackgroundTaskApi.Git.GitStateChanged += OnGitStateChanged;
    	base.OnInitialized();
    }

    private void HandleOnClick(GitState localGitState)
    {
        if (localGitState.Repo is null)
        {
            var dialogViewModel = new DialogViewModel(
            Key<IDynamicViewModel>.NewKey(),
            $"Git Repo",
            typeof(GitAddRepoDisplay),
            null,
            null,
            true,
            null);

            DialogService.ReduceRegisterAction(dialogViewModel);
        }
        else
        {
            var panelState = PanelService.GetPanelState();
            var gitPanel = panelState.PanelList.FirstOrDefault(x => x.ContextRecordKey == ContextFacts.GitContext.ContextKey);

            if (gitPanel is null)
                return;

            var panelGroup = gitPanel.TabGroup as PanelGroup;

            if (panelGroup is not null)
            {
                PanelService.SetActivePanelTab(panelGroup.Key, gitPanel.Key);
            }
            else
            {
                PanelService.RegisterPanelTab(PanelFacts.LeftPanelGroupKey, gitPanel, true);
                PanelService.SetActivePanelTab(PanelFacts.LeftPanelGroupKey, gitPanel.Key);
            }
        }
    }
    
    private async void OnGitStateChanged()
    {
    	await InvokeAsync(StateHasChanged);
    }
    
    public void Dispose()
    {
		GitBackgroundTaskApi.Git.GitStateChanged -= OnGitStateChanged;
    }
}