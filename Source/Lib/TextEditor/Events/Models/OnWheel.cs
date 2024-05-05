using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.RazorLib.Events.Models;

public class OnWheel : ITextEditorTask
{
    private readonly TextEditorViewModelDisplay.TextEditorEvents _events;

    public OnWheel(
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
    public string Name { get; } = nameof(OnWheel);
    public Task? WorkProgress { get; }
    public WheelEventArgs WheelEventArgs { get; }
    public Key<TextEditorViewModel> ViewModelKey { get; }

    public TimeSpan ThrottleTimeSpan => TextEditorViewModelDisplay.TextEditorEvents.ThrottleDelayDefault;

    public async Task InvokeWithEditContext(IEditContext editContext)
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
        // If the two individuals, or a batch and an individual are both positive,
        // then batch them, etc... for negative and 0

        if (oldEvent is OnWheel oldEventOnWheel)
        {
            if (oldEventOnWheel.WheelEventArgs.DeltaY > 0 &&
                WheelEventArgs.DeltaY > 0)
            {
                return new OnWheelBatch(
                    new List<WheelEventArgs>()
                    {
                        oldEventOnWheel.WheelEventArgs,
                        WheelEventArgs
                    },
                    _events,
                    ViewModelKey);
            }
            else if (oldEventOnWheel.WheelEventArgs.DeltaY < 0 &&
                     WheelEventArgs.DeltaY < 0)
            {
                return new OnWheelBatch(
                    new List<WheelEventArgs>()
                    {
                        oldEventOnWheel.WheelEventArgs,
                        WheelEventArgs
                    },
                    _events,
                    ViewModelKey);
            }
            else if (oldEventOnWheel.WheelEventArgs.DeltaY == 0 &&
                     WheelEventArgs.DeltaY == 0)
            {
                return new OnWheelBatch(
                    new List<WheelEventArgs>()
                    {
                        oldEventOnWheel.WheelEventArgs,
                        WheelEventArgs
                    },
                    _events,
                    ViewModelKey);
            }
        }

        if (oldEvent is OnWheelBatch oldEventOnWheelBatch)
        {
            if (oldEventOnWheelBatch.WheelEventArgsList.Last().DeltaY > 0 &&
                WheelEventArgs.DeltaY > 0)
            {
                oldEventOnWheelBatch.WheelEventArgsList.Add(WheelEventArgs);
                return oldEventOnWheelBatch;
            }
            else if (oldEventOnWheelBatch.WheelEventArgsList.Last().DeltaY < 0 &&
                     WheelEventArgs.DeltaY < 0)
            {
                oldEventOnWheelBatch.WheelEventArgsList.Add(WheelEventArgs);
                return oldEventOnWheelBatch;
            }
            else if (oldEventOnWheelBatch.WheelEventArgsList.Last().DeltaY == 0 &&
                     WheelEventArgs.DeltaY == 0)
            {
                oldEventOnWheelBatch.WheelEventArgsList.Add(WheelEventArgs);
                return oldEventOnWheelBatch;
            }
        }

        return null;
    }

    public Task HandleEvent(CancellationToken cancellationToken)
    {
        throw new NotImplementedException($"{nameof(ITextEditorTask)} should not implement {nameof(HandleEvent)}" +
            "because they instead are contained within an 'IBackgroundTask' that came from the 'TextEditorService'");
    }
}
