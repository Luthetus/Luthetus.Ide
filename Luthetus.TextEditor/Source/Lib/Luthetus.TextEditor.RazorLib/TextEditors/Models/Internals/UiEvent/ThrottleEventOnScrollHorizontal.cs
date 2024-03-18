using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals.UiEvent;

public class ThrottleEventOnScrollHorizontal : ITextEditorTask
{
    private readonly TextEditorViewModelDisplay.TextEditorEvents _events;

    public ThrottleEventOnScrollHorizontal(
		double scrollLeft,
        TextEditorViewModelDisplay.TextEditorEvents events,
        Key<TextEditorViewModel> viewModelKey)
    {
        _events = events;

		ScrollLeft = scrollLeft;
        ViewModelKey = viewModelKey;
    }

    public Key<BackgroundTask> BackgroundTaskKey { get; } = Key<BackgroundTask>.NewKey();
    public Key<BackgroundTaskQueue> QueueKey { get; } = ContinuousBackgroundTaskWorker.GetQueueKey();
    public string Name { get; } = nameof(ThrottleEventOnScrollHorizontal);
    public Task? WorkProgress { get; }
	public double ScrollLeft { get; }
    public Key<TextEditorViewModel> ViewModelKey { get; }

    public TimeSpan ThrottleTimeSpan => _events.ThrottleDelayDefault;

    public async Task InvokeWithEditContext(ITextEditorEditContext editContext)
	{
		var viewModelModifier = editContext.GetViewModelModifier(ViewModelKey);

        if (viewModelModifier is null)
            return;

		await viewModelModifier.ViewModel!.SetScrollPositionFactory(ScrollLeft, null)
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
