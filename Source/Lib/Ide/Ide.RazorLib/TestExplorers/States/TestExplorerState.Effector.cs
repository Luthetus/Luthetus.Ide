using Fluxor;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.DotNetSolutions.States;

namespace Luthetus.Ide.RazorLib.TestExplorers.States;

public partial record TestExplorerState
{
	public class Effector
	{
		private readonly IdeBackgroundTaskApi _ideBackgroundTaskApi;

		public Effector(IdeBackgroundTaskApi ideBackgroundTaskApi)
		{
            _ideBackgroundTaskApi = ideBackgroundTaskApi;
		}

		[EffectMethod(typeof(DotNetSolutionState.StateHasChanged))]
		public Task HandleDotNetSolutionStateStateHasChanged(IDispatcher dispatcher)
		{
			_ = dispatcher; // Suppress unused parameter

            _ideBackgroundTaskApi.TestExplorer.ConstructTreeView();
			return Task.CompletedTask;
		}
		
		[EffectMethod(typeof(TestExplorerState.ShouldInitializeEffect))]
		public Task HandleShouldDiscoverTestsEffect(IDispatcher dispatcher)
		{
			_ = dispatcher; // Suppress unused parameter

            _ideBackgroundTaskApi.TestExplorer.DiscoverTests();
			return Task.CompletedTask;
		}
	}
}
