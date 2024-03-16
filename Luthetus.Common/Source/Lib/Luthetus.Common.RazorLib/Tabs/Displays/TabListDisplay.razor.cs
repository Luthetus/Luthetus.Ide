using Luthetus.Common.RazorLib.Tabs.Models;
using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.Tabs.Displays;

public partial class TabListDisplay : ComponentBase
{
	[Parameter, EditorRequired]
	public ImmutableArray<ITabViewModel> TabList { get; set; } = ImmutableArray<ITabViewModel>.Empty;
	
	[Parameter]
	public string CssClassString { get; set; } = string.Empty;

	public async Task NotifyStateChangedAsync()
	{
		await InvokeAsync(StateHasChanged);
	}
}