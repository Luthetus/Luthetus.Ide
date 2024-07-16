using Luthetus.Ide.RazorLib.DotNetSolutions.States;

namespace Luthetus.CompilerServices.RazorLib.TestExplorers.States;

public partial record TestExplorerState
{
	public class Effector
	{
		private readonly IState<DotNetSolutionState> _dotNetSolutionStateWrap;
	
		public Effector(IState<DotNetSolutionState> dotNetSolutionStateWrap)
		{
            _dotNetSolutionStateWrap = dotNetSolutionStateWrap;
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
