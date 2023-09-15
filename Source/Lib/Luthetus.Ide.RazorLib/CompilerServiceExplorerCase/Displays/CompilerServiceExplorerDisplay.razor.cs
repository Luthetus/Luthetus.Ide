using Fluxor;
using Fluxor.Blazor.Web.Components;
using Luthetus.Common.RazorLib.Store.TabCase;
using Luthetus.Common.RazorLib.TabCase;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.CompilerServiceExplorerCase.Displays;

public partial class CompilerServiceExplorerDisplay : FluxorComponent
{
    [Inject]
    private IState<CompilerServiceExplorerRegistry> CompilerServiceExplorerRegistryWrap { get; set; } = null!;
    [Inject]
    private IStateSelection<TabRegistry, TabGroup?> TabRegistrySelection { get; set; } = null!;

    protected override void OnInitialized()
    {
        TabRegistrySelection.Select(tabGroupsCollection =>
            tabGroupsCollection.GroupBag.SingleOrDefault(
                x => x.Key == CompilerServiceExplorerRegistry.TabGroupKey));

        base.OnInitialized();
    }
}

