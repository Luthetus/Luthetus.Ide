using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Extensions.DotNet.BackgroundTasks.Models;

namespace Luthetus.Extensions.DotNet.Shareds.Displays.Internals;

public partial class IdePromptOpenSolutionDisplay : ComponentBase
{
	[Inject]
	private DotNetBackgroundTaskApi DotNetBackgroundTaskApi { get; set; } = null!;

	[Parameter, EditorRequired]
	public IAbsolutePath AbsolutePath { get; set; } = null!;

	private void OpenSolutionOnClick()
	{
        DotNetBackgroundTaskApi.DotNetSolution.SetDotNetSolution(AbsolutePath);
	}
}