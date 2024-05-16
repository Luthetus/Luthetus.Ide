using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Dropdowns.States;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.Gits.States;
using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.Gits.Displays;

/// <summary>
/// Long term goal is to support any arbitrary version control system.
/// For now, implement a Git UI, this lets us get a feel for what the interface should be.
/// </summary>
public partial class GitDisplay : FluxorComponent
{
    [Inject]
    private IState<GitState> GitStateWrap { get; set; } = null!;
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;
    [Inject]
    private ILuthetusIdeComponentRenderers IdeComponentRenderers { get; set; } = null!;
    [Inject]
    private ILuthetusCommonComponentRenderers CommonComponentRenderers { get; set; } = null!;
    [Inject]
    private LuthetusIdeBackgroundTaskApi IdeBackgroundTaskApi { get; set; } = null!;

    private Key<DropdownRecord> _menuDropdownKey = Key<DropdownRecord>.NewKey();
    private MenuRecord _menu = new(ImmutableArray<MenuOptionRecord>.Empty);
    private ElementReference? _menuButtonElementReference;

    protected override void OnInitialized()
    {
        ConstructMenu();
        base.OnInitialized();
    }

    private void ConstructMenu()
    {
        var localGitState = GitStateWrap.Value;

        var menuOptionsList = new List<MenuOptionRecord>();

        // Repo
        {
            string menuOptionDisplayName;
            if (localGitState.Repo is null)
                menuOptionDisplayName = "Pick Repo";
            else
                menuOptionDisplayName = "Change Repo";

            var menuOption = new MenuOptionRecord(
                menuOptionDisplayName,
                MenuOptionKind.Other,
                () => { ShowAddRepoDialog(); return Task.CompletedTask; });

            var menuOptionNew = new MenuOptionRecord(
                "Repo",
                MenuOptionKind.Other,
                SubMenu: new MenuRecord(new[] { menuOption }.ToImmutableArray()));

            menuOptionsList.Add(menuOptionNew);
        }
        
        // Branch
        if (localGitState.Repo is not null)
        {
            var branchNewMenuOption = GetBranchNewMenuOptionRecord(localGitState);
            var branchCheckoutMenuOption = GetBranchCheckoutOptionRecord(localGitState);

            var menuOptionNew = new MenuOptionRecord(
                "Branch",
                MenuOptionKind.Other,
                SubMenu: new MenuRecord(new[]
                { 
                    branchNewMenuOption,
                    branchCheckoutMenuOption,
                }.ToImmutableArray()));

            menuOptionsList.Add(menuOptionNew);
        }

        // Actions
        if (localGitState.Repo is not null)
        {
            var actionPushMenuOption = GetActionPushToOriginWithTrackingMenuOption(localGitState);

            var menuOptionNew = new MenuOptionRecord(
                "Actions",
                MenuOptionKind.Other,
                SubMenu: new MenuRecord(new[]
                {
                    actionPushMenuOption,
                }.ToImmutableArray()));

            menuOptionsList.Add(menuOptionNew);
        }

        _menu = new MenuRecord(menuOptionsList.ToImmutableArray());
    }

    private void ShowMenuDropdown(Key<DropdownRecord> dropdownKey)
    {
        ConstructMenu();
        Dispatcher.Dispatch(new DropdownState.AddActiveAction(dropdownKey));
    }

    private async Task RestoreFocusToMenuButton()
    {
        try
        {
            if (_menuButtonElementReference is not null)
            {
                await _menuButtonElementReference.Value
                    .FocusAsync()
                    .ConfigureAwait(false);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private void ShowAddRepoDialog()
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

    private MenuOptionRecord GetBranchNewMenuOptionRecord(GitState localGitState)
    {
        return new MenuOptionRecord(
            "New",
            MenuOptionKind.Create,
            WidgetRendererType: IdeComponentRenderers.FileFormRendererType,
            WidgetParameterMap: new Dictionary<string, object?>
            {
                { nameof(IFileFormRendererType.FileName), string.Empty },
                { nameof(IFileFormRendererType.CheckForTemplates), false },
                {
                    nameof(IFileFormRendererType.OnAfterSubmitFunc),
                    new Func<string, IFileTemplate?, ImmutableArray<IFileTemplate>, Task>(
                        async (fileName, exactMatchFileTemplate, relatedMatchFileTemplates) =>
                            await PerformNewFile(localGitState, fileName))
                },
            });

        async Task PerformNewFile(GitState localGitState, string fileName)
        {
            if (localGitState.Repo is not null)
                await IdeBackgroundTaskApi.Git.GitBranchNewExecute(localGitState.Repo, fileName);
        }
    }
    
    private MenuOptionRecord GetBranchCheckoutOptionRecord(GitState localGitState)
    {
        return new MenuOptionRecord(
            "Checkout",
            MenuOptionKind.Other,
            WidgetRendererType: typeof(GitBranchCheckoutDisplay),
            WidgetParameterMap: new Dictionary<string, object?>
            {
                { nameof(GitBranchCheckoutDisplay.GitState), localGitState },
            });
    }

    private async Task GetBranchesOnClick(GitState localGitState)
    {
        if (localGitState.Repo is null)
            return;

        await IdeBackgroundTaskApi.Git.GitBranchGetAllExecute(localGitState.Repo)
            .ConfigureAwait(false);
    }

    private MenuOptionRecord GetActionPushToOriginWithTrackingMenuOption(GitState localGitState)
    {
        return new MenuOptionRecord(
            $"push -u origin {localGitState.Branch}",
            MenuOptionKind.Other,
            DoAction);

        async Task DoAction()
        {
            if (localGitState.Repo is null)
                return;
            await IdeBackgroundTaskApi.Git.GitPushToOriginWithTrackingExecute(localGitState.Repo)
                .ConfigureAwait(false);
        }

    }
}