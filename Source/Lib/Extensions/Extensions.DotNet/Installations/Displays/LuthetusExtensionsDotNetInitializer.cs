using Microsoft.AspNetCore.Components;
using Luthetus.Extensions.DotNet.BackgroundTasks.Models;

namespace Luthetus.Extensions.DotNet.Installations.Displays;

public partial class LuthetusExtensionsDotNetInitializer : ComponentBase
{
    [Inject]
	private DotNetBackgroundTaskApi DotNetBackgroundTaskApi { get; set; } = null!;

    protected override void OnInitialized()
	{
		DotNetBackgroundTaskApi.Enqueue(new DotNetBackgroundTaskApiWorkArgs
		{
			WorkKind = DotNetBackgroundTaskApiWorkKind.LuthetusExtensionsDotNetInitializerOnInit,
		});
		base.OnInitialized();
	}
	
	protected override void OnAfterRender(bool firstRender)
	{
		if (firstRender)
		{
            DotNetBackgroundTaskApi.Enqueue(new DotNetBackgroundTaskApiWorkArgs
            {
            	WorkKind = DotNetBackgroundTaskApiWorkKind.LuthetusExtensionsDotNetInitializerOnAfterRender
            });
		}
	}
}