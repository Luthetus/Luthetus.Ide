using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;

namespace Luthetus.TextEditor.RazorLib.Events;

/// <summary>
/// This class allows for easy creation of a <see cref="ITextEditorTask"/>, that comes with "redundancy" checking in the queue.
/// That is, if when enqueueing an instance of this type, the last item in the queue is already an instance of this type
/// with the same <see cref="Identifier"/>, then this instance will overwrite the last item in
/// the queue, because the logic has no value if ran many times one after another, therefore, just take the most recent event.
/// </summary>
public class TakeMostRecentTextEditorTask : ITextEditorTask
{
    private readonly TextEditorEdit _textEditorEdit;

    public TakeMostRecentTextEditorTask(
        string name,
        string redundancyIdentifier,
        TextEditorEdit textEditorEdit,
        TimeSpan? throttleTimeSpan = null)
    {
        _textEditorEdit = textEditorEdit;

        Name = name;
        Identifier = redundancyIdentifier;
        ThrottleTimeSpan = throttleTimeSpan ?? TextEditorViewModelDisplay.TextEditorEvents.ThrottleDelayDefault;
    }

    public string Name { get; }
    public string Identifier { get; }
    public Key<BackgroundTask> BackgroundTaskKey { get; } = Key<BackgroundTask>.NewKey();
    public Key<BackgroundTaskQueue> QueueKey { get; } = ContinuousBackgroundTaskWorker.GetQueueKey();
    public TimeSpan ThrottleTimeSpan { get; }
    public Task? WorkProgress { get; }

    public async Task InvokeWithEditContext(IEditContext editContext)
    {
        await _textEditorEdit
            .Invoke(editContext)
            .ConfigureAwait(false);
    }

    public IBackgroundTask? BatchOrDefault(IBackgroundTask oldEvent)
    {
        if (oldEvent is not TakeMostRecentTextEditorTask oldRedundantTextEditorTask)
        {
            // Keep both events
            return null;
        }

        if (oldRedundantTextEditorTask.Name == Name)
        {
            // Keep this event (via replacement)
            return this;
        }

        // Keep both events
        return null;
    }

    public Task HandleEvent(CancellationToken cancellationToken)
    {
        throw new NotImplementedException($"{nameof(ITextEditorTask)} should not implement {nameof(HandleEvent)}" +
            "because they instead are contained within an 'IBackgroundTask' that came from the 'TextEditorService'");
    }
}
