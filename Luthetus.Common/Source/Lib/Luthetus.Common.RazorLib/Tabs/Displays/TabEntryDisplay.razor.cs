using Fluxor;
using Luthetus.Common.RazorLib.Tabs.States;
using Luthetus.Common.RazorLib.Tabs.Models;
using Microsoft.AspNetCore.Components;

namespace Luthetus.Common.RazorLib.Tabs.Displays;

public partial class TabEntryDisplay : ComponentBase
{
    [Inject]
    private IDispatcher Dispatcher { get; set; } = null!;

    [Parameter, EditorRequired]
    public TabGroup TabGroup { get; set; } = null!;
    [Parameter, EditorRequired]
    public TabEntryNoType TabEntryNoType { get; set; } = null!;

    private bool IsActive => TabGroup.ActiveEntryKey == TabEntryNoType.TabEntryKey;

    private string IsActiveCssClassString => IsActive ? "luth_active" : string.Empty;

    private void DispatchSetActivePanelTabActionOnClick()
    {
        Dispatcher.Dispatch(new TabState.SetActiveTabEntryKeyAction(
            TabGroup.Key,
            TabEntryNoType.TabEntryKey));
    }
}