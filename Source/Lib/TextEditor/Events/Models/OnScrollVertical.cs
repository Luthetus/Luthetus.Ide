using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;

namespace Luthetus.TextEditor.RazorLib.Events.Models;

public class OnScrollVertical : ITextEditorTask
{
    public OnScrollVertical(
        double scrollTop,
		TextEditorComponentData componentData,
        Key<TextEditorViewModel> viewModelKey)
    {
		ComponentData = componentData;

        ScrollTop = scrollTop;
        ViewModelKey = viewModelKey;
    }

    public Key<IBackgroundTask> BackgroundTaskKey { get; } = Key<IBackgroundTask>.NewKey();
    public Key<IBackgroundTaskQueue> QueueKey { get; } = ContinuousBackgroundTaskWorker.GetQueueKey();
    public string Name { get; } = nameof(OnScrollVertical);
    public Task? WorkProgress { get; }
    public double ScrollTop { get; }
    public Key<TextEditorViewModel> ViewModelKey { get; }
    public TextEditorComponentData ComponentData { get; }

	public IEditContext EditContext { get; set; }

    public TimeSpan ThrottleTimeSpan => TextEditorComponentData.ThrottleDelayDefault;

    public IBackgroundTask? BatchOrDefault(IBackgroundTask oldEvent)
    {
        if (oldEvent is OnScrollVertical)
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

            await EditContext.TextEditorService.ViewModelApi
                .SetScrollPositionFactory(ViewModelKey, null, ScrollTop)
                .Invoke(EditContext);
		}
		finally
		{
			await EditContext.TextEditorService.FinalizePost(EditContext);
		}
    }
}
