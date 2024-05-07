using Fluxor;
using Luthetus.Ide.RazorLib.DotNetSolutions.States;

namespace Luthetus.Ide.RazorLib.TestExplorers.States;

public partial record TestExplorerState
{
	public class Effector
	{
		private readonly TestExplorerSync _testExplorerSync;

		public Effector(TestExplorerSync testExplorerSync)
		{
			_testExplorerSync = testExplorerSync;
		}

		[EffectMethod(typeof(DotNetSolutionState.StateHasChanged))]
		public Task HandleDotNetSolutionStateStateHasChanged(IDispatcher dispatcher)
		{
			_ = dispatcher; // Suppress unused parameter

            return _testExplorerSync.DotNetSolutionStateWrap_StateChanged();
		}
	}
}
