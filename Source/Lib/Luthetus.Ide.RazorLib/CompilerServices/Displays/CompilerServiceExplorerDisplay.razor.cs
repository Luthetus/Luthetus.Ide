using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Tabs.States;
using Luthetus.Common.RazorLib.Tabs.Models;
using Luthetus.Ide.RazorLib.CompilerServiceExplorerCase.States;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.CompilerServices.Displays;

public partial class CompilerServiceExplorerDisplay : FluxorComponent
{
    [Inject]
    private IState<CompilerServiceExplorerState> CompilerServiceExplorerStateWrap { get; set; } = null!;
    [Inject]
    private IStateSelection<TabState, TabGroup?> TabStateSelection { get; set; } = null!;

    protected override void OnInitialized()
    {
        TabStateSelection.Select(tabGroupsCollection =>
            tabGroupsCollection.TabGroupBag.SingleOrDefault(
                x => x.Key == CompilerServiceExplorerState.TabGroupKey));

        base.OnInitialized();
    }
}

