using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;

namespace Luthetus.TextEditor.RazorLib.Events.Models;

public struct OnWheelBatch : ITextEditorWork
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

    public Key<IBackgroundTask> BackgroundTaskKey => Key<IBackgroundTask>.Empty;
    public Key<IBackgroundTaskQueue> QueueKey { get; } = ContinuousBackgroundTaskWorker.GetQueueKey();
    public string Name { get; private set; } = nameof(OnWheelBatch);
    public List<WheelEventArgs> WheelEventArgsList { get; }
    public Key<TextEditorViewModel> ViewModelKey { get; }
    public TextEditorComponentData ComponentData { get; }

	public ITextEditorEditContext EditContext { get; set; }

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

			// See this quoted comment from OnWheel.cs
			// =======================================
			// 	"TODO: Why was this made as 'if' 'else' whereas the OnWheelBatch...
			// 	      ...is doing 'if' 'if'.
			// 	      |
			// 	      The OnWheelBatch doesn't currently batch horizontal with vertical
			// 	      the OnWheel events have to be the same axis to batch."
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
            
            await EditContext.TextEditorService
            	.FinalizePost(EditContext)
            	.ConfigureAwait(false);
            	
            await Task.Delay(ThrottleFacts.TwentyFour_Frames_Per_Second).ConfigureAwait(false);
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
		}
    }
}
