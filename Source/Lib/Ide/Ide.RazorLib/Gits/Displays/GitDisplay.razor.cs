using Fluxor;
using System.Collections.Immutable;
using Fluxor.Blazor.Web.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Dropdowns.States;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Menus.Displays;
using Luthetus.Common.RazorLib.JsRuntimes.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.Gits.States;

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
	[Inject]
    private IJSRuntime JsRuntime { get; set; } = null!;

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

        Dispatcher.Dispatch(new DropdownState.RegisterAction(dropdownRecord));
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
            var pushMenuOption = GetActionPushToOriginWithTrackingMenuOption(localGitState);
            var pullMenuOption = GetActionPullMenuOption(localGitState);
            var fetchMenuOption = GetActionFetchMenuOption(localGitState);

            var menuOptionNew = new MenuOptionRecord(
                "Actions",
                MenuOptionKind.Other,
                SubMenu: new MenuRecord(new[]
                {
                    pushMenuOption,
                    pullMenuOption,
                    fetchMenuOption,
                }.ToImmutableArray()));

            menuOptionsList.Add(menuOptionNew);
        }

        return new MenuRecord(menuOptionsList.ToImmutableArray());
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
                            await PerformBranchNewEnqueue(localGitState, fileName))
                },
            });

        Task PerformBranchNewEnqueue(GitState localGitState, string fileName)
        {
            if (localGitState.Repo is not null)
                IdeBackgroundTaskApi.Git.BranchNewEnqueue(localGitState.Repo, fileName);

			return Task.CompletedTask;
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

    private MenuOptionRecord GetActionPushToOriginWithTrackingMenuOption(GitState localGitState)
    {
        return new MenuOptionRecord(
            $"push -u origin {localGitState.Branch}",
            MenuOptionKind.Other,
            DoAction);

        Task DoAction()
        {
            if (localGitState.Repo is not null)
            	IdeBackgroundTaskApi.Git.PushToOriginWithTrackingEnqueue(localGitState.Repo);

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
                IdeBackgroundTaskApi.Git.PullEnqueue(localGitState.Repo);

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
                IdeBackgroundTaskApi.Git.FetchEnqueue(localGitState.Repo);

            return Task.CompletedTask;
        }
    }
}