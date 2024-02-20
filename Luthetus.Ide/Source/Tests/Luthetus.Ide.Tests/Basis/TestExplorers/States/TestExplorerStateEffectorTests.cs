using Fluxor;
using Luthetus.Ide.RazorLib.DotNetSolutions.States;

namespace Luthetus.Ide.Tests.Basis.TestExplorers.States;

public partial record TestExplorerStateEffectorTests
{
	public class Effector
	{
        [Fact]
        public void Constructor()
        {
	  //      public Effector(TestExplorerSync testExplorerSync)
			//{
			//	_testExplorerSync = testExplorerSync;
			//}
        }

        [Fact]
        public void HandleDotNetSolutionStateStateHasChanged()
        {
			//[EffectMethod(typeof(DotNetSolutionState.StateHasChanged))]
			//public Task (IDispatcher dispatcher)
			//{
			//	_ = dispatcher; // Suppress unused parameter

			//	_testExplorerSync.DotNetSolutionStateWrap_StateChanged();
			//	return Task.CompletedTask;
			//}
        }
	}
}
