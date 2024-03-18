using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals.UiEvent;

public class ThrottleEventOnWheelBatch : ITextEditorTask
{
    private readonly TextEditorViewModelDisplay.TextEditorEvents _events;

    public ThrottleEventOnWheelBatch(
        List<WheelEventArgs> wheelEventArgsList,
        TextEditorViewModelDisplay.TextEditorEvents events,
        Key<TextEditorViewModel> viewModelKey)
    {
        _events = events;

        WheelEventArgsList = wheelEventArgsList;
        ViewModelKey = viewModelKey;
    }

    public Key<BackgroundTask> BackgroundTaskKey { get; } = Key<BackgroundTask>.NewKey();
    public Key<BackgroundTaskQueue> QueueKey { get; } = ContinuousBackgroundTaskWorker.GetQueueKey();
    public string Name => nameof(ThrottleEventOnWheelBatch) + $"_{WheelEventArgsList.Count}";
    public Task? WorkProgress { get; }
    public List<WheelEventArgs> WheelEventArgsList { get; }
    public Key<TextEditorViewModel> ViewModelKey { get; }

    public TimeSpan ThrottleTimeSpan => _events.ThrottleDelayDefault;

    public Task InvokeWithEditContext(ITextEditorEditContext editContext)
	{
		var viewModelModifier = editContext.GetViewModelModifier(ViewModelKey);

        if (viewModelModifier is null)
            return Task.CompletedTask;

        double? horizontalMutateScrollPositionByPixels = null;
        double? verticalMutateScrollPositionByPixels = null;

        foreach (var wheelEventArgs in WheelEventArgsList)
        {
            if (wheelEventArgs.ShiftKey)
            {
                horizontalMutateScrollPositionByPixels ??= 0;
                horizontalMutateScrollPositionByPixels += wheelEventArgs.DeltaY;
            }
            else
            {
                verticalMutateScrollPositionByPixels ??= 0;
                verticalMutateScrollPositionByPixels += wheelEventArgs.DeltaY;
            }
        }

        if (horizontalMutateScrollPositionByPixels is not null)
        {
            _events.TextEditorService.ViewModelApi.MutateScrollHorizontalPositionFactory(
                viewModelModifier.ViewModel.BodyElementId,
                viewModelModifier.ViewModel.GutterElementId,
                horizontalMutateScrollPositionByPixels.Value);
        }
        
        if (verticalMutateScrollPositionByPixels is not null)
        {
            _events.TextEditorService.ViewModelApi.MutateScrollVerticalPositionFactory(
                viewModelModifier.ViewModel.BodyElementId,
                viewModelModifier.ViewModel.GutterElementId,
                verticalMutateScrollPositionByPixels.Value);
        }
        
        return Task.CompletedTask;
	}

    public IBackgroundTask? BatchOrDefault(IBackgroundTask oldEvent)
    {
        return null;
    }

    public Task HandleEvent(CancellationToken cancellationToken)
    {
        throw new NotImplementedException($"{nameof(ITextEditorTask)} should not implement {nameof(HandleEvent)}" +
			"because they instead are contained within an 'IBackgroundTask' that came from the 'TextEditorService'");
    }
}
