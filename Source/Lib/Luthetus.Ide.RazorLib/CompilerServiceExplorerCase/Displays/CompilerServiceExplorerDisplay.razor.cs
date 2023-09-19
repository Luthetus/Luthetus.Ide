using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.TabCase.Models;
using Luthetus.Common.RazorLib.TabCase.States;
using Luthetus.Ide.RazorLib.CompilerServiceExplorerCase.States;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.CompilerServiceExplorerCase.Displays;

public partial class CompilerServiceExplorerDisplay : FluxorComponent
{
    [Inject]
    private IState<CompilerServiceExplorerState> CompilerServiceExplorerRegistryWrap { get; set; } = null!;
    [Inject]
    private IStateSelection<TabState, TabGroup?> TabRegistrySelection { get; set; } = null!;

    protected override void OnInitialized()
    {
        TabRegistrySelection.Select(tabGroupsCollection =>
            tabGroupsCollection.TabGroupBag.SingleOrDefault(
                x => x.Key == CompilerServiceExplorerState.TabGroupKey));

        base.OnInitialized();
    }
}

