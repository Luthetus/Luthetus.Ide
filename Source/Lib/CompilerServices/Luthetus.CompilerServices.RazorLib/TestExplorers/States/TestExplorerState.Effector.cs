using Fluxor;
using Luthetus.CompilerServices.RazorLib.DotNetSolutions.States;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;

namespace Luthetus.CompilerServices.RazorLib.TestExplorers.States;

public partial record TestExplorerState
{
	public class Effector
	{
		private readonly IState<DotNetSolutionState> _dotNetSolutionStateWrap;
		private readonly IdeBackgroundTaskApi _ideBackgroundTaskApi;
	
		public Effector(
			IState<DotNetSolutionState> dotNetSolutionStateWrap,
			IdeBackgroundTaskApi ideBackgroundTaskApi)
		{
            _ideBackgroundTaskApi = ideBackgroundTaskApi;
		}
		
		[EffectMethod(typeof(DotNetSolutionState.StateHasChanged))]
		public Task HandleDotNetSolutionStateStateHasChanged(IDispatcher dispatcher)
		{
			_ = dispatcher; // Suppress unused parameter

            _ideBackgroundTaskApi.TestExplorer.Enqueue_ConstructTreeView();
			return Task.CompletedTask;
		}
	}
}
