using Fluxor;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Extensions.DotNet.BackgroundTasks.Models;

namespace Luthetus.Extensions.DotNet.Outputs.States;

public partial record OutputState
{
	public class Effector
	{
		private readonly DotNetBackgroundTaskApi _dotNetBackgroundTaskApi;
		
		private readonly Throttle _throttleCreateTreeView = new Throttle(TimeSpan.FromMilliseconds(333));

        public Effector(DotNetBackgroundTaskApi dotNetBackgroundTaskApi)
		{
			_dotNetBackgroundTaskApi = dotNetBackgroundTaskApi;
        }
        
        [EffectMethod(typeof(OutputState.ConstructTreeViewEffect))]
		public Task HandleConstructTreeViewEffect(IDispatcher dispatcher)
		{
			_ = dispatcher; // Suppress unused parameter

			_throttleCreateTreeView.Run(async _ => await _dotNetBackgroundTaskApi.Output.Task_ConstructTreeView());
            return Task.CompletedTask;
		}
	}
}
