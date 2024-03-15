using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.Tabs.Displays;

public partial class PolymorphicTabListDisplay : ComponentBase
{
	[Parameter, EditorRequired]
	public ImmutableArray<IPolymorphicTab> TabList { get; set; } = ImmutableArray<IPolymorphicTab>.Empty;
	
	[Parameter]
	public string CssClassString { get; set; } = string.Empty;

	public async Task NotifyStateChangedAsync()
	{
		await InvokeAsync(StateHasChanged);
	}
}