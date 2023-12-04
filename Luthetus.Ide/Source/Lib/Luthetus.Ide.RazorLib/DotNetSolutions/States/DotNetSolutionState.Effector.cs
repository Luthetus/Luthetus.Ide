using Fluxor;
using static Luthetus.Ide.RazorLib.DotNetSolutions.States.DotNetSolutionState;
using static Luthetus.Ide.RazorLib.DotNetSolutions.States.DotNetSolutionSync;

namespace Luthetus.Ide.RazorLib.DotNetSolutions.States;

public partial record DotNetSolutionState
{
	public class Effector
	{
		[EffectMethod(typeof(IWithAction))]
		public async Task NotifyDotNetSolutionStateStateHasChanged(
			IDispatcher dispatcher)
		{
			dispatcher.Dispatch(new DotNetSolutionState.StateHasChanged());
		}
	}
}
