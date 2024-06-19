using Fluxor;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.DotNetSolutions.States;

namespace Luthetus.Ide.RazorLib.TestExplorers.States;

public partial record TestExplorerState
{
	public class Effector
	{
		private readonly LuthetusIdeBackgroundTaskApi _ideBackgroundTaskApi;

		public Effector(LuthetusIdeBackgroundTaskApi ideBackgroundTaskApi)
		{
            _ideBackgroundTaskApi = ideBackgroundTaskApi;
		}

		[EffectMethod(typeof(DotNetSolutionState.StateHasChanged))]
		public Task HandleDotNetSolutionStateStateHasChanged(IDispatcher dispatcher)
		{
			_ = dispatcher; // Suppress unused parameter

            _ideBackgroundTaskApi.TestExplorer.DotNetSolutionStateWrap_StateChanged();
			return Task.CompletedTask;
		}
	}
}
