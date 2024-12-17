using Fluxor;
using Luthetus.Extensions.DotNet.BackgroundTasks.Models;
using Luthetus.Extensions.DotNet.DotNetSolutions.States;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;

namespace Luthetus.Extensions.DotNet.TestExplorers.States;

public partial record TestExplorerState
{
	public class Effector
	{
        private readonly DotNetBackgroundTaskApi _dotNetBackgroundTaskApi;
        private readonly IdeBackgroundTaskApi _ideBackgroundTaskApi;
		private readonly IState<TestExplorerState> _testExplorerStateWrap;
        private readonly IState<DotNetSolutionState> _dotNetSolutionStateWrap;

        public Effector(
			DotNetBackgroundTaskApi dotNetBackgroundTaskApi,
			IdeBackgroundTaskApi ideBackgroundTaskApi,
			IState<TestExplorerState> testExplorerStateWrap,
			IState<DotNetSolutionState> dotNetSolutionStateWrap)
		{
            _dotNetBackgroundTaskApi = dotNetBackgroundTaskApi;
            _ideBackgroundTaskApi = ideBackgroundTaskApi;
            _testExplorerStateWrap = testExplorerStateWrap;
            _dotNetSolutionStateWrap = dotNetSolutionStateWrap;
        }
        
        /// <summary>
        /// Each time the user opens the 'Test Explorer' panel,
        /// a check is done to see if the data being displayed
        /// is in sync with the user's selected .NET solution.
        ///
        /// If it is not in sync, then it starts discovering tests for each of the
        /// projects in the solution.
        ///
        /// But, if the user cancels this task, if they change panel tabs
        /// from the 'Test Explorer' to something else, when they return
        /// it will once again try to discover tests in all the projects for the solution.
        ///
        /// This is very annoying from a user perspective.
        /// So this field will track whether we've already started
        /// the task to discover tests in all the projects for the solution or not.
        ///
        /// This is fine because there is a button in the top left of the panel that
        /// has a 'refresh' icon and it will start this task if the
        /// user manually clicks it, (even if they cancelled the automatic invocation).
        /// </summary>
        private string _intentToDiscoverTestsInSolutionFilePath = string.Empty;
        
        [EffectMethod(typeof(DotNetSolutionState.StateHasChanged))]
		public Task HandleDotNetSolutionStateStateHasChanged(IDispatcher dispatcher)
		{
			_ = dispatcher; // Suppress unused parameter

            _dotNetBackgroundTaskApi.TestExplorer.Enqueue_ConstructTreeView();
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

			if (dotNetSolutionModel.AbsolutePath.Value != testExplorerState.SolutionFilePath &&
				_intentToDiscoverTestsInSolutionFilePath != dotNetSolutionModel.AbsolutePath.Value)
			{
				_intentToDiscoverTestsInSolutionFilePath = dotNetSolutionModel.AbsolutePath.Value;
				dispatcher.Dispatch(new TestExplorerState.ShouldInitializeEffect());
			}

			return Task.CompletedTask;
		}
		
		[EffectMethod(typeof(TestExplorerState.ShouldInitializeEffect))]
		public Task HandleShouldDiscoverTestsEffect(IDispatcher dispatcher)
		{
			_ = dispatcher; // Suppress unused parameter

            return _dotNetBackgroundTaskApi.TestExplorer.Task_DiscoverTests();
		}
	}
}
