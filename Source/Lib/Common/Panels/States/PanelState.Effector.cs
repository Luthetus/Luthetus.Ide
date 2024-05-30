using Fluxor;
using Luthetus.Common.RazorLib.Dimensions.States;

namespace Luthetus.Common.RazorLib.Panels.States;

public partial record PanelState
{
	public class Effector
	{
		[EffectMethod(typeof(SetActivePanelTabAction))]
        public Task HandleSetActivePanelTabAction(
            IDispatcher dispatcher)
		{
			OnActiveTabChanged(dispatcher);
			return Task.CompletedTask;
		}

		[EffectMethod(typeof(SetPanelTabAsActiveByContextRecordKeyAction))]
        public Task HandleSetPanelTabAsActiveByContextRecordKeyAction(
            IDispatcher dispatcher)
		{
			OnActiveTabChanged(dispatcher);
			return Task.CompletedTask;
		}

		private void OnActiveTabChanged(IDispatcher dispatcher)
		{
			dispatcher.Dispatch(new AppDimensionState.NotifyIntraAppResizeAction());
		}
	}
}
