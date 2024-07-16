using Fluxor;
using static Luthetus.Ide.RazorLib.DotNetSolutions.Models.DotNetSolutionIdeApi;

namespace Luthetus.CompilerServices.RazorLib.DotNetSolutions.States;

public partial record DotNetSolutionState
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
