using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Panels.Models;
using Luthetus.Common.RazorLib.Panels.States;
using Luthetus.Ide.RazorLib.Gits.States;
using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.Gits.Displays;

public partial class GitInteractiveIconDisplay : FluxorComponent
{
    [Inject]
    private IState<GitState> GitStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private IState<PanelState> PanelStateWrap { get; set; } = null!;
    [Inject]
    private IState<DialogState> DialogStateWrap { get; set; } = null!;

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
            true);

            Dispatcher.Dispatch(new DialogState.RegisterAction(dialogViewModel));
        }
        else
        {
            var panelState = PanelStateWrap.Value;
            var gitPanel = panelState.PanelList.FirstOrDefault(x => x.ContextRecordKey == ContextFacts.GitContext.ContextKey);

            if (gitPanel is null)
                return;

            var panelGroup = gitPanel.TabGroup as PanelGroup;

            if (panelGroup is not null)
            {
                Dispatcher.Dispatch(new PanelState.SetActivePanelTabAction(panelGroup.Key, gitPanel.Key));
            }
            else
            {
                Dispatcher.Dispatch(new PanelState.RegisterPanelTabAction(PanelFacts.LeftPanelGroupKey, gitPanel, true));
                Dispatcher.Dispatch(new PanelState.SetActivePanelTabAction(PanelFacts.LeftPanelGroupKey, gitPanel.Key));
            }
        }
    }
}