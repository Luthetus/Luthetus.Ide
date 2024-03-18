using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals.UiEvent;

public class ThrottleEventOnScrollVertical : ITextEditorTask
{
    private readonly TextEditorViewModelDisplay.TextEditorEvents _events;

    public ThrottleEventOnScrollVertical(
		double scrollTop,
        TextEditorViewModelDisplay.TextEditorEvents events,
        Key<TextEditorViewModel> viewModelKey)
    {
        _events = events;

		ScrollTop = scrollTop;
        ViewModelKey = viewModelKey;
    }

    public Key<BackgroundTask> BackgroundTaskKey { get; } = Key<BackgroundTask>.NewKey();
    public Key<BackgroundTaskQueue> QueueKey { get; } = ContinuousBackgroundTaskWorker.GetQueueKey();
    public string Name { get; } = nameof(ThrottleEventOnScrollVertical);
    public Task? WorkProgress { get; }
	public double ScrollTop { get; }
    public Key<TextEditorViewModel> ViewModelKey { get; }

    public TimeSpan ThrottleTimeSpan => _events.ThrottleDelayDefault;

    public async Task InvokeWithEditContext(ITextEditorEditContext editContext)
	{
        var viewModelModifier = editContext.GetViewModelModifier(ViewModelKey);

        if (viewModelModifier is null)
            return;

		await viewModelModifier.ViewModel!.SetScrollPositionFactory(null, ScrollTop)
			.Invoke(editContext)
			.ConfigureAwait(false);
	}

    public IBackgroundTask? BatchOrDefault(IBackgroundTask oldEvent)
    {
		return this;
    }

    public Task HandleEvent(CancellationToken cancellationToken)
    {
        throw new NotImplementedException($"{nameof(ITextEditorTask)} should not implement {nameof(HandleEvent)}" +
			"because they instead are contained within an 'IBackgroundTask' that came from the 'TextEditorService'");
    }
}
