using Microsoft.AspNetCore.Components;
using Luthetus.Extensions.DotNet.BackgroundTasks.Models;

namespace Luthetus.Extensions.DotNet.DotNetSolutions.Displays.Internals;

public partial class SolutionPropertiesDisplay : ComponentBase, IDisposable
{
	[Inject]
	private DotNetBackgroundTaskApi DotNetBackgroundTaskApi { get; set; } = null!;
	
	protected override void OnInitialized()
	{
		DotNetBackgroundTaskApi.DotNetSolutionService.DotNetSolutionStateChanged += OnDotNetSolutionStateChanged;
		base.OnInitialized();
	}
	
	private async void OnDotNetSolutionStateChanged()
	{
		await InvokeAsync(StateHasChanged);
	}
	
	public void Dispose()
	{
		DotNetBackgroundTaskApi.DotNetSolutionService.DotNetSolutionStateChanged -= OnDotNetSolutionStateChanged;
	}
}