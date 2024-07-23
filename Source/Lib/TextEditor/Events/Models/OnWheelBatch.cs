using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;

namespace Luthetus.TextEditor.RazorLib.Events.Models;

public class OnWheelBatch : ITextEditorWork
{
    public OnWheelBatch(
        List<WheelEventArgs> wheelEventArgsList,
		TextEditorComponentData componentData,
        Key<TextEditorViewModel> viewModelKey)
    {
		ComponentData = componentData;

        WheelEventArgsList = wheelEventArgsList;
        ViewModelKey = viewModelKey;
    }

    public Key<IBackgroundTask> BackgroundTaskKey { get; } = Key<IBackgroundTask>.NewKey();
    public Key<IBackgroundTaskQueue> QueueKey { get; } = ContinuousBackgroundTaskWorker.GetQueueKey();
    public string Name { get; private set; } = nameof(OnWheelBatch);
    public Task? WorkProgress { get; }
    public List<WheelEventArgs> WheelEventArgsList { get; }
    public Key<TextEditorViewModel> ViewModelKey { get; }
    public TextEditorComponentData ComponentData { get; }

	public IEditContext EditContext { get; set; }

    public TimeSpan ThrottleTimeSpan => TextEditorComponentData.ThrottleDelayDefault;

    public IBackgroundTask? BatchOrDefault(IBackgroundTask oldEvent)
    {
        return null;
    }

    public async Task HandleEvent(CancellationToken cancellationToken)
    {
		try
		{
			Name += $"_{WheelEventArgsList.Count}";

            var viewModelModifier = EditContext.GetViewModelModifier(ViewModelKey);

            if (viewModelModifier is null)
                return;

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
                EditContext.TextEditorService.ViewModelApi.MutateScrollHorizontalPosition(
                    EditContext,
			        viewModelModifier,
			        horizontalMutateScrollPositionByPixels.Value);
            }

            if (verticalMutateScrollPositionByPixels is not null)
            {
                EditContext.TextEditorService.ViewModelApi.MutateScrollVerticalPosition(
                    EditContext,
			        viewModelModifier,
			        verticalMutateScrollPositionByPixels.Value);
            }
		}
		finally
		{
			await EditContext.TextEditorService.FinalizePost(EditContext);
		}
    }
}
