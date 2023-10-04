using Microsoft.AspNetCore.Components;
using Fluxor.Blazor.Web.Components;
using Fluxor;
using Luthetus.Common.RazorLib.Tabs.States;
using Luthetus.Common.RazorLib.Tabs.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Tabs.Displays;

public partial class TabGroupDisplay : FluxorComponent
{
    [Inject]
    private IStateSelection<TabState, TabGroup?> TabStateSelection { get; set; } = null!;

    [Parameter, EditorRequired]
    public Key<TabGroup> TabGroupKey { get; set; } = Key<TabGroup>.Empty;

    protected override void OnInitialized()
    {
        TabStateSelection
            .Select(tabState => tabState.TabGroupBag
                .SingleOrDefault(x => x.Key == TabGroupKey));

        base.OnInitialized();
    }
}