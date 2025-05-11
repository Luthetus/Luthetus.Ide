using Microsoft.AspNetCore.Components;
using Luthetus.Extensions.Git.BackgroundTasks.Models;

namespace Luthetus.Extensions.Git.Installations.Displays;

public partial class LuthetusExtensionsGitInitializer : ComponentBase
{
    [Inject]
	private GitBackgroundTaskApi GitBackgroundTaskApi { get; set; } = null!;

    protected override void OnInitialized()
	{
        // GitBackgroundTaskApi.Enqueue_LuthetusExtensionsGitInitializerOnInit();       
        base.OnInitialized();
	}
}
