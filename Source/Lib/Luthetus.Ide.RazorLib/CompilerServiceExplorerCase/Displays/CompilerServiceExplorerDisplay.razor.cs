using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Store.TabCase;
using Luthetus.Common.RazorLib.TabCase.Models;
using Luthetus.Ide.RazorLib.CompilerServiceExplorerCase.States;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.CompilerServiceExplorerCase.Displays;

public partial class CompilerServiceExplorerDisplay : FluxorComponent
{
    [Inject]
    private IState<CompilerServiceExplorerState> CompilerServiceExplorerRegistryWrap { get; set; } = null!;
    [Inject]
    private IStateSelection<TabRegistry, TabGroup?> TabRegistrySelection { get; set; } = null!;

    protected override void OnInitialized()
    {
        TabRegistrySelection.Select(tabGroupsCollection =>
            tabGroupsCollection.GroupBag.SingleOrDefault(
                x => x.Key == CompilerServiceExplorerState.TabGroupKey));

        base.OnInitialized();
    }
}

