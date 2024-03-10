using Luthetus.Common.RazorLib.PolymorphicUis.Models;
using Microsoft.AspNetCore.Components;
using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.PolymorphicUis.Displays;

public partial class PolymorphicTabListDisplay : ComponentBase
{
	[Parameter, EditorRequired]
	public ImmutableArray<IPolymorphicTab> TabList { get; set; } = ImmutableArray<IPolymorphicTab>.Empty;

	public async Task NotifyStateChangedAsync()
	{
		await InvokeAsync(StateHasChanged);
	}
}