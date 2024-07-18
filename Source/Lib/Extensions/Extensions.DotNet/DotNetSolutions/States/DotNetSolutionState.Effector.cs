using Fluxor;
using static Luthetus.Extensions.DotNet.DotNetSolutions.Models.DotNetSolutionIdeApi;

namespace Luthetus.Extensions.DotNet.DotNetSolutions.States;

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
