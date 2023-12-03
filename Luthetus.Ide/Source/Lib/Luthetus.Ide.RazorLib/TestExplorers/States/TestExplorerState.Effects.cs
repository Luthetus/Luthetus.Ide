using Luthetus.Ide.RazorLib.DotNetSolutions.States;

namespace Luthetus.Ide.RazorLib.TestExplorers.States;

public class TestExplorerState
{
	public class Effector
	{
		private TestExplorerSync _testExplorerSync;

		public Effector(TestExplorerSync testExplorerSync)
		{
			_testExplorerSync = testExplorerSync;
		}

		[EffectMethod]
		public async Task HandleDotNetSolutionStateStateHasChanged(
			DotNetSolutionState.StateHasChanged dotNetSolutionStateStateHasChanged,
			IDispatcher dispatcher)
		{
			_testExplorerSync.DotNetSolutionStateWrap_StateChanged();
		}
	}
}
