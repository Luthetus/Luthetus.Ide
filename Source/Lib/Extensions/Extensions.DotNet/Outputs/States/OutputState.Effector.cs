using Fluxor;

namespace Luthetus.Extensions.DotNet.Outputs.States;

public partial record OutputState
{
	public class Effector
	{
        public Effector()
		{
        }
        
        [EffectMethod(typeof(OutputState.ConstructTreeViewEffect))]
		public Task HandleConstructTreeViewEffect(IDispatcher dispatcher)
		{
			_ = dispatcher; // Suppress unused parameter

			_throttleCreateTreeView.Run(_ => _dotNetBackgroundTaskApi.Output.Task_ConstructTreeView());
            return Task.CompletedTask;
		}
	}
}
