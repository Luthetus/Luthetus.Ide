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
		private readonly IState<DotNetSolutionState> _dotNetSolutionStateWrap;
		private readonly IState<TestExplorerState> _testExplorerStateWrap;

		public Effector(
			IdeBackgroundTaskApi ideBackgroundTaskApi,
			IState<DotNetSolutionState> dotNetSolutionStateWrap,
			IState<TestExplorerState> testExplorerStateWrap)
		{
            _ideBackgroundTaskApi = ideBackgroundTaskApi;
            _dotNetSolutionStateWrap = dotNetSolutionStateWrap;
            _testExplorerStateWrap = testExplorerStateWrap;
		}

		[EffectMethod(typeof(DotNetSolutionState.StateHasChanged))]
		public Task HandleDotNetSolutionStateStateHasChanged(IDispatcher dispatcher)
		{
			_ = dispatcher; // Suppress unused parameter

            _ideBackgroundTaskApi.TestExplorer.Enqueue_ConstructTreeView();
			return Task.CompletedTask;
		}
		
		[EffectMethod(typeof(TestExplorerState.UserInterfaceWasInitializedEffect))]
		public Task HandleUserInterfaceWasInitializedEffect(IDispatcher dispatcher)
		{
			var dotNetSolutionState = _dotNetSolutionStateWrap.Value;
	        var dotNetSolutionModel = dotNetSolutionState.DotNetSolutionModel;
	
	        if (dotNetSolutionModel is null)
	            return Task.CompletedTask;
	            
	        var testExplorerState = _testExplorerStateWrap.Value;
	            
	        if (dotNetSolutionModel.AbsolutePath.Value != testExplorerState.SolutionFilePath)
		        dispatcher.Dispatch(new TestExplorerState.ShouldInitializeEffect());
		
			return Task.CompletedTask;
		}
		
		[EffectMethod(typeof(TestExplorerState.ShouldInitializeEffect))]
		public Task HandleShouldDiscoverTestsEffect(IDispatcher dispatcher)
		{
			_ = dispatcher; // Suppress unused parameter

            _ideBackgroundTaskApi.TestExplorer.Enqueue_DiscoverTests();
			return Task.CompletedTask;
		}
	}
}
