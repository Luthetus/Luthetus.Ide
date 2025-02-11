using System.Collections.Immutable;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Menus.Displays;
using Luthetus.Common.RazorLib.JsRuntimes.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.FileSystems.Models;
using Luthetus.Extensions.Git.States;
using Luthetus.Extensions.Git.BackgroundTasks.Models;

namespace Luthetus.Extensions.Git.Displays;

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
    private IIdeComponentRenderers IdeComponentRenderers { get; set; } = null!;
    [Inject]
    private ICommonComponentRenderers CommonComponentRenderers { get; set; } = null!;
    [Inject]
    private GitBackgroundTaskApi GitBackgroundTaskApi { get; set; } = null!;
	[Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;
	[Inject]
    private IAppOptionsService AppOptionsService { get; set; } = null!;
	[Inject]
    private IDialogService DialogService { get; set; } = null!;
    [Inject]
    private IDropdownService DropdownService { get; set; } = null!;

	private const string _dropdownMenuHtmlElementId = "luth_ide_git-display-dropdown-menu";

    private readonly Key<DropdownRecord> _menuDropdownKey = Key<DropdownRecord>.NewKey();

    private ElementReference? _menuButtonElementReference;

	private async Task ShowMenuDropdown(Key<DropdownRecord> dropdownKey)
    {
        var menu = ConstructMenu();
		
		var jsRuntimeCommonApi = JsRuntime.GetLuthetusCommonApi();

		var buttonDimensions = await jsRuntimeCommonApi
			.MeasureElementById(_dropdownMenuHtmlElementId)
			.ConfigureAwait(false);

		var dropdownRecord = new DropdownRecord(
			_menuDropdownKey,
			buttonDimensions.LeftInPixels,
			buttonDimensions.TopInPixels + buttonDimensions.HeightInPixels,
			typeof(MenuDisplay),
			new Dictionary<string, object?>
			{
				{
					nameof(MenuDisplay.MenuRecord),
					menu
				}
			},
			restoreFocusOnClose: null);

        DropdownService.ReduceRegisterAction(dropdownRecord);
    }

    private MenuRecord ConstructMenu()
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
                subMenu: new MenuRecord(new() { menuOption }));

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
                subMenu: new MenuRecord(new()
                { 
                    branchNewMenuOption,
                    branchCheckoutMenuOption,
                }));

            menuOptionsList.Add(menuOptionNew);
        }

        // Actions
        if (localGitState.Repo is not null)
        {
            var pushMenuOption = GetActionPushToOriginWithTrackingMenuOption(localGitState);
            var pullMenuOption = GetActionPullMenuOption(localGitState);
            var fetchMenuOption = GetActionFetchMenuOption(localGitState);

            var menuOptionNew = new MenuOptionRecord(
                "Actions",
                MenuOptionKind.Other,
                subMenu: new MenuRecord(new()
                {
                    pushMenuOption,
                    pullMenuOption,
                    fetchMenuOption,
                }));

            menuOptionsList.Add(menuOptionNew);
        }

        return new MenuRecord(menuOptionsList);
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
            true,
            null);

        DialogService.ReduceRegisterAction(dialogViewModel);
    }

    private MenuOptionRecord GetBranchNewMenuOptionRecord(GitState localGitState)
    {
        return new MenuOptionRecord(
            "New",
            MenuOptionKind.Create,
            widgetRendererType: IdeComponentRenderers.FileFormRendererType,
            widgetParameterMap: new Dictionary<string, object?>
            {
                { nameof(IFileFormRendererType.FileName), string.Empty },
                { nameof(IFileFormRendererType.CheckForTemplates), false },
                {
                    nameof(IFileFormRendererType.OnAfterSubmitFunc),
                    new Func<string, IFileTemplate?, ImmutableArray<IFileTemplate>, Task>(
                        async (fileName, exactMatchFileTemplate, relatedMatchFileTemplates) =>
                            await PerformBranchNewEnqueue(localGitState, fileName))
                },
            });

        Task PerformBranchNewEnqueue(GitState localGitState, string fileName)
        {
            if (localGitState.Repo is not null)
                GitBackgroundTaskApi.Git.BranchNewEnqueue(localGitState.Repo, fileName);

			return Task.CompletedTask;
        }
    }
    
    private MenuOptionRecord GetBranchCheckoutOptionRecord(GitState localGitState)
    {
        return new MenuOptionRecord(
            "Checkout",
            MenuOptionKind.Other,
            widgetRendererType: typeof(GitBranchCheckoutDisplay),
            widgetParameterMap: new Dictionary<string, object?>
            {
                { nameof(GitBranchCheckoutDisplay.GitState), localGitState },
            });
    }

    private MenuOptionRecord GetActionPushToOriginWithTrackingMenuOption(GitState localGitState)
    {
        return new MenuOptionRecord(
            $"push -u origin {localGitState.Branch}",
            MenuOptionKind.Other,
            DoAction);

        Task DoAction()
        {
            if (localGitState.Repo is not null)
            	GitBackgroundTaskApi.Git.PushToOriginWithTrackingEnqueue(localGitState.Repo);

			return Task.CompletedTask;
        }
    }
    
    private MenuOptionRecord GetActionPullMenuOption(GitState localGitState)
    {
        return new MenuOptionRecord(
            "pull",
            MenuOptionKind.Other,
            DoAction);

        Task DoAction()
        {
            if (localGitState.Repo is not null)
                GitBackgroundTaskApi.Git.PullEnqueue(localGitState.Repo);

            return Task.CompletedTask;
        }
    }
    
    private MenuOptionRecord GetActionFetchMenuOption(GitState localGitState)
    {
        return new MenuOptionRecord(
            "fetch",
            MenuOptionKind.Other,
            DoAction);

        Task DoAction()
        {
            if (localGitState.Repo is not null)
                GitBackgroundTaskApi.Git.FetchEnqueue(localGitState.Repo);

            return Task.CompletedTask;
        }
    }
}