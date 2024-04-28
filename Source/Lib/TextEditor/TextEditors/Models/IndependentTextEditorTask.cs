using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;

namespace Luthetus.TextEditor.RazorLib.Events;

/// <summary>
/// This class allows for easy creation of a <see cref="ITextEditorTask"/>, that has NO "redundancy" checking in the queue.
/// That is, if when enqueueing an instance of this type, there is a last item in the queue, then this type under no circumstances
/// will batch, replace, or other, with that existing queue'd item.
/// </summary>
/// <remarks>
/// There is nothing stopping someone from creating an implementation of <see cref="ITextEditorTask"/>
/// that sees the last item in the queue to be of this type, and then batching or etc with it.
/// One should NOT do this, but it is possible, and should be remarked about.
/// </remarks>
public class IndependentTextEditorTask : ITextEditorTask
{
    private readonly TextEditorEdit _textEditorEdit;

    public IndependentTextEditorTask(
        string name,
        TextEditorEdit textEditorEdit,
        TimeSpan? throttleTimeSpan = null)
    {
        _textEditorEdit = textEditorEdit;

        Name = name;
        ThrottleTimeSpan = throttleTimeSpan ?? TextEditorViewModelDisplay.TextEditorEvents.ThrottleDelayDefault;
    }

    public string Name { get; }
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
        // Keep both events
        return null;
    }

    public Task HandleEvent(CancellationToken cancellationToken)
    {
        throw new NotImplementedException($"{nameof(ITextEditorTask)} should not implement {nameof(HandleEvent)}" +
            "because they instead are contained within an 'IBackgroundTask' that came from the 'TextEditorService'");
    }
}
