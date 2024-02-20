using Fluxor;
using static Luthetus.Ide.RazorLib.DotNetSolutions.States.DotNetSolutionSync;

namespace Luthetus.Ide.Tests.Basis.DotNetSolutions.States;

public class DotNetSolutionStateEffectorTests
{
	public class Effector
	{
		[EffectMethod(typeof(IWithAction))]
		public Task NotifyDotNetSolutionStateStateHasChanged(IDispatcher dispatcher)
		{
			dispatcher.Dispatch(new StateHasChanged());
			return Task.CompletedTask;
		}
	}
}
