using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;

namespace Luthetus.TextEditor.RazorLib.Events;

/// <summary>
/// This class allows for easy creation of a <see cref="ITextEditorTask"/>, that comes with "redundancy" checking in the queue.
/// That is, if when enqueueing an instance of this type, the last item in the queue is already an instance of this type
/// with the same <see cref="Name"/>, and same <see cref="ViewModelKey"/> then this instance will overwrite the last item in
/// the queue, because the logic has no value if ran many times one after another, therefore, just take the most recent event.
/// </summary>
public class IdempotentTextEditorTask : ITextEditorTask
{
    private readonly TextEditorViewModelDisplay.TextEditorEvents _events;
    private readonly TextEditorEdit _textEditorEdit;

    public IdempotentTextEditorTask(
        string name,
        TextEditorViewModelDisplay.TextEditorEvents events,
        Key<TextEditorViewModel> viewModelKey,
        TextEditorEdit textEditorEdit,
        TimeSpan? throttleTimeSpan = null)
    {
        Name = name;
        ViewModelKey = viewModelKey;

        _events = events;
        _textEditorEdit = textEditorEdit;

        ThrottleTimeSpan = throttleTimeSpan ?? TextEditorViewModelDisplay.TextEditorEvents.ThrottleDelayDefault;
    }

    public Key<BackgroundTask> BackgroundTaskKey { get; } = Key<BackgroundTask>.NewKey();
    public Key<BackgroundTaskQueue> QueueKey { get; } = ContinuousBackgroundTaskWorker.GetQueueKey();
    public string Name { get; } = nameof(OnScrollVertical);
    public Task? WorkProgress { get; }
    public Key<TextEditorViewModel> ViewModelKey { get; }

    public TimeSpan ThrottleTimeSpan { get; }

    public async Task InvokeWithEditContext(IEditContext editContext)
    {
        await _textEditorEdit
            .Invoke(editContext)
            .ConfigureAwait(false);
    }

    public IBackgroundTask? BatchOrDefault(IBackgroundTask oldEvent)
    {
        if (oldEvent is IdempotentTextEditorTask oldIdempotentTextEditorTask)
        {
            if (oldIdempotentTextEditorTask.Name == Name &&
                oldIdempotentTextEditorTask.ViewModelKey == ViewModelKey)
            {
                // Replace the oldEvent with this one
                return this;
            }
        }

        // Keep the oldEvent, and enqueue this one
        return null;
    }

    public Task HandleEvent(CancellationToken cancellationToken)
    {
        throw new NotImplementedException($"{nameof(ITextEditorTask)} should not implement {nameof(HandleEvent)}" +
            "because they instead are contained within an 'IBackgroundTask' that came from the 'TextEditorService'");
    }
}
