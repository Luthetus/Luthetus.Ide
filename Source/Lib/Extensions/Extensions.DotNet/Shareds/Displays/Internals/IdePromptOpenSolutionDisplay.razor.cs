using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Extensions.DotNet.Shareds.Displays.Internals;

public partial class IdePromptOpenSolutionDisplay : ComponentBase
{
	[Inject]
	private IdeBackgroundTaskApi IdeBackgroundTaskApi { get; set; } = null!;

	[Parameter, EditorRequired]
	public IAbsolutePath AbsolutePath { get; set; } = null!;

	private void OpenSolutionOnClick()
	{
		//// Am moving .NET code out so the IDE is language agnostic. (2024-07-15)
		// =========================================================================
		// IdeBackgroundTaskApi.DotNetSolution.SetDotNetSolution(AbsolutePath);
	}
}