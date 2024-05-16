using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.Dropdowns.Models;
using Luthetus.Common.RazorLib.Dropdowns.States;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Ide.RazorLib.DotNetSolutions.States;
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
        var menuOptionsList = new List<MenuOptionRecord>();

        // Repo
        {
            string menuOptionDisplayName;
            if (GitStateWrap.Value.Repo is null)
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
}