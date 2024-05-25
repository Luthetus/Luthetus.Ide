using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;

namespace Luthetus.TextEditor.RazorLib.Events.Models;

public class OnScrollHorizontal : ITextEditorTask
{
    private readonly TextEditorEvents _events;

    public OnScrollHorizontal(
        double scrollLeft,
        TextEditorEvents events,
        Key<TextEditorViewModel> viewModelKey)
    {
        _events = events;

        ScrollLeft = scrollLeft;
        ViewModelKey = viewModelKey;
    }

    public Key<BackgroundTask> BackgroundTaskKey { get; } = Key<BackgroundTask>.NewKey();
    public Key<BackgroundTaskQueue> QueueKey { get; } = ContinuousBackgroundTaskWorker.GetQueueKey();
    public string Name { get; } = nameof(OnScrollHorizontal);
	public string? Redundancy { get; } = null;
	public TextEditorEdit Edit { get; }
    public Task? WorkProgress { get; }
    public double ScrollLeft { get; }
    public Key<TextEditorViewModel> ViewModelKey { get; }

    public TimeSpan ThrottleTimeSpan => TextEditorEvents.ThrottleDelayDefault;

    public Task InvokeWithEditContext(IEditContext editContext)
    {
        var viewModelModifier = editContext.GetViewModelModifier(ViewModelKey);
        if (viewModelModifier is null)
            return Task.CompletedTask;

        return editContext.TextEditorService.ViewModelApi
            .SetScrollPositionFactory(ViewModelKey, ScrollLeft, null)
            .Invoke(editContext);
    }

    public IBackgroundTask? BatchOrDefault(IBackgroundTask oldEvent)
    {
        return this;
    }

	public IBackgroundTask? DequeueBatchOrDefault(IBackgroundTask oldEvent)
	{
		return null;
	}

    public Task HandleEvent(CancellationToken cancellationToken)
    {
        throw new NotImplementedException($"{nameof(ITextEditorTask)} should not implement {nameof(HandleEvent)}" +
            "because they instead are contained within an 'IBackgroundTask' that came from the 'TextEditorService'");
    }
}
