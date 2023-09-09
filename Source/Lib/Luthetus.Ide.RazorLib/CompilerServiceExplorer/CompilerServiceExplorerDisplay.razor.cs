using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Store.TabGroupCase;
using Luthetus.Common.RazorLib.TabGroups;
using Luthetus.Ide.ClassLib.Store.CompilerServiceExplorerCase;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.CompilerServiceExplorer;

public partial class CompilerServiceExplorerDisplay : FluxorComponent
{
    [Inject]
    private IState<CompilerServiceExplorerState> CompilerServiceExplorerStateWrap { get; set; } = null!;
    [Inject]
    private IStateSelection<TabGroupsCollection, TabGroup?> TabGroupsCollectionStateSelection { get; set; } = null!;

    protected override void OnInitialized()
    {
        TabGroupsCollectionStateSelection.Select(tabGroupsCollection =>
            tabGroupsCollection.TabGroups.SingleOrDefault(
                x => x.TabGroupKey == CompilerServiceExplorerState.TabGroupKey));

        base.OnInitialized();
    }
}

