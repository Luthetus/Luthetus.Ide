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

            _dotNetBackgroundTaskApi.TestExplorer.Enqueue_DiscoverTests();
			return Task.CompletedTask;
		}
	}
}
