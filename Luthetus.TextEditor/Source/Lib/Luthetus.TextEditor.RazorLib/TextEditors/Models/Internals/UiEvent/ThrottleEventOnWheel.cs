using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals.UiEvent;

public class ThrottleEventOnWheel : ITextEditorTask
{
    private readonly TextEditorViewModelDisplay.TextEditorEvents _events;

    public ThrottleEventOnWheel(
        WheelEventArgs wheelEventArgs,
        TextEditorViewModelDisplay.TextEditorEvents events,
        Key<TextEditorViewModel> viewModelKey)
    {
        _events = events;

        WheelEventArgs = wheelEventArgs;
        ViewModelKey = viewModelKey;
    }

    public Key<BackgroundTask> BackgroundTaskKey { get; } = Key<BackgroundTask>.NewKey();
    public Key<BackgroundTaskQueue> QueueKey { get; } = ContinuousBackgroundTaskWorker.GetQueueKey();
    public string Name { get; } = nameof(ThrottleEventOnWheel);
    public Task? WorkProgress { get; }
    public WheelEventArgs WheelEventArgs { get; }
    public Key<TextEditorViewModel> ViewModelKey { get; }

    public TimeSpan ThrottleTimeSpan => _events.ThrottleDelayDefault;

    public async Task InvokeWithEditContext(ITextEditorEditContext editContext)
	{
		var viewModelModifier = editContext.GetViewModelModifier(ViewModelKey);

        if (viewModelModifier is null)
            return;

        if (WheelEventArgs.ShiftKey)
		{
            await viewModelModifier.ViewModel.MutateScrollHorizontalPositionByPixelsFactory(WheelEventArgs.DeltaY)
				.Invoke(editContext)
				.ConfigureAwait(false);
		}
        else
		{
            await viewModelModifier.ViewModel.MutateScrollVerticalPositionByPixelsFactory(WheelEventArgs.DeltaY)
				.Invoke(editContext)
				.ConfigureAwait(false);
		}
	}

    public IBackgroundTask? BatchOrDefault(IBackgroundTask oldEvent)
    {
        if (oldEvent is ThrottleEventOnWheel oldEventOnWheel)
        {
            return new ThrottleEventOnWheelBatch(
                new List<WheelEventArgs>()
                {
                    oldEventOnWheel.WheelEventArgs,
                    WheelEventArgs
                },
                _events,
                ViewModelKey);
        }

        if (oldEvent is ThrottleEventOnWheelBatch oldEventOnWheelBatch)
        {
            oldEventOnWheelBatch.WheelEventArgsList.Add(WheelEventArgs);
            return oldEventOnWheelBatch;
        }

        return null;
    }

    public Task HandleEvent(CancellationToken cancellationToken)
    {
        throw new NotImplementedException($"{nameof(ITextEditorTask)} should not implement {nameof(HandleEvent)}" +
			"because they instead are contained within an 'IBackgroundTask' that came from the 'TextEditorService'");
    }
}
