using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;

namespace Luthetus.TextEditor.RazorLib.Events.Models;

public struct OnScrollHorizontal : ITextEditorWork
{
    public OnScrollHorizontal(
        double scrollLeft,
		TextEditorComponentData componentData,
        Key<TextEditorViewModel> viewModelKey)
    {
		ComponentData = componentData;

        ScrollLeft = scrollLeft;
        ViewModelKey = viewModelKey;
    }

    public Key<IBackgroundTask> BackgroundTaskKey { get; } = Key<IBackgroundTask>.NewKey();
    public Key<IBackgroundTaskQueue> QueueKey { get; } = ContinuousBackgroundTaskWorker.GetQueueKey();
    public string Name { get; } = nameof(OnScrollHorizontal);
    public double ScrollLeft { get; }
    public Key<TextEditorViewModel> ViewModelKey { get; }
    public TextEditorComponentData ComponentData { get; }

	public ITextEditorEditContext EditContext { get; set; }

    public IBackgroundTask? BatchOrDefault(IBackgroundTask oldEvent)
    {
        if (oldEvent is OnScrollHorizontal)
		{
			// Replace the upstream event with this one,
			// because unhandled-consecutive events of this type are redundant.
			return this;
		}
        
		// Keep both events, because they are not able to be batched.
		return null;
    }

    public async Task HandleEvent(CancellationToken cancellationToken)
    {
		try
		{
            var viewModelModifier = EditContext.GetViewModelModifier(ViewModelKey);
            if (viewModelModifier is null)
                return;

            EditContext.TextEditorService.ViewModelApi.SetScrollPosition(
            	EditContext,
        		viewModelModifier,
            	ScrollLeft,
            	null);
            	
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
