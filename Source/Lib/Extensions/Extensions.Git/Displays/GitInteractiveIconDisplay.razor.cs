using Microsoft.AspNetCore.Components;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Extensions.Git.States;

namespace Luthetus.Extensions.Git.Displays;

public partial class GitInteractiveIconDisplay : FluxorComponent
{
    [Inject]
    private IState<GitState> GitStateWrap { get; set; } = null!;
    [Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;
    [Inject]
    private IPanelService PanelService { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IDialogService DialogService { get; set; } = null!;

    [Parameter]
    public string CssClassString { get; set; } = string.Empty;
    [Parameter]
    public string CssStyleString { get; set; } = string.Empty;

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
                PanelService.ReduceSetActivePanelTabAction(panelGroup.Key, gitPanel.Key);
            }
            else
            {
                PanelService.ReduceRegisterPanelTabAction(PanelFacts.LeftPanelGroupKey, gitPanel, true);
                PanelService.ReduceSetActivePanelTabAction(PanelFacts.LeftPanelGroupKey, gitPanel.Key);
            }
        }
    }
}