using Fluxor;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Ide.RazorLib.TestExplorers.States;

public partial record TestExplorerState
{
	public class Effector
	{
		private readonly IdeBackgroundTaskApi _ideBackgroundTaskApi;
		private readonly IState<TestExplorerState> _testExplorerStateWrap;

		public Effector(
			IdeBackgroundTaskApi ideBackgroundTaskApi,
			IState<TestExplorerState> testExplorerStateWrap)
		{
            _ideBackgroundTaskApi = ideBackgroundTaskApi;
            _testExplorerStateWrap = testExplorerStateWrap;
		}

		[EffectMethod(typeof(TestExplorerState.UserInterfaceWasInitializedEffect))]
		public Task HandleUserInterfaceWasInitializedEffect(IDispatcher dispatcher)
		{
            //// Am moving .NET code out so the IDE is language agnostic. (2024-07-15)
            // =======================================================================
			//
			// var dotNetSolutionState = _dotNetSolutionStateWrap.Value;
	        // var dotNetSolutionModel = dotNetSolutionState.DotNetSolutionModel;
			// 
	        // if (dotNetSolutionModel is null)
	        //     return Task.CompletedTask;
	        //    
	        // var testExplorerState = _testExplorerStateWrap.Value;
			// 	            
	        // if (dotNetSolutionModel.AbsolutePath.Value != testExplorerState.SolutionFilePath)
		    //     dispatcher.Dispatch(new TestExplorerState.ShouldInitializeEffect());
			
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
