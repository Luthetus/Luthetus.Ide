using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;

namespace Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;

/// <summary>
/// This class allows for easy creation of a <see cref="ITextEditorTask"/>, that comes with "redundancy" checking in the queue.
/// That is, if when enqueueing an instance of this type, the last item in the queue
/// is an instance of this type, has the same <see cref="Name"/>, <see cref="ResourceUri"/>, and <see cref="ViewModelKey"/>,
/// then this instance will overwrite the last item in the queue, because the logic has no value if ran many times one after another,
/// therefore, just take the most recent event.
/// </summary>
public sealed class TakeMostRecentTextEditorTask : ITextEditorTask
{
    private readonly TextEditorEdit _textEditorEdit;

    public TakeMostRecentTextEditorTask(
        string name,
        ResourceUri resourceUri,
        Key<TextEditorViewModel> viewModelKey,
        TextEditorEdit textEditorEdit,
        TimeSpan? throttleTimeSpan = null)
    {
        _textEditorEdit = textEditorEdit;

        Name = name;
        ResourceUri = resourceUri;
        ViewModelKey = ViewModelKey;
        ThrottleTimeSpan = throttleTimeSpan ?? TextEditorComponentData.ThrottleDelayDefault;
    }

	public string Name { get; set; }
	public ResourceUri ResourceUri { get; set; }
    public Key<TextEditorViewModel> ViewModelKey { get; set; }
    public Key<BackgroundTask> BackgroundTaskKey { get; set; } = Key<BackgroundTask>.NewKey();
    public Key<BackgroundTaskQueue> QueueKey { get; set; } = ContinuousBackgroundTaskWorker.GetQueueKey();
    public TimeSpan ThrottleTimeSpan { get; set; }
    public Task? WorkProgress { get; set; }

	public IEditContext EditContext { get; set; }

    public IBackgroundTask? BatchOrDefault(IBackgroundTask oldEvent)
    {
        if (oldEvent is not TakeMostRecentTextEditorTask oldRedundantTextEditorTask)
        {
            // Keep both events
            return null;
        }

        if (oldRedundantTextEditorTask.Name == Name &&
		    oldRedundantTextEditorTask.ResourceUri == ResourceUri &&
            oldRedundantTextEditorTask.ViewModelKey == ViewModelKey)
        {
            // Keep this event (via replacement)
            return this;
        }

        // Keep both events
        return null;
    }

    public async Task HandleEvent(CancellationToken cancellationToken)
    {
		try
		{
            await _textEditorEdit
                .Invoke(EditContext)
                .ConfigureAwait(false);
		}
		finally
		{
			await EditContext.TextEditorService.FinalizePost(EditContext);
		}
    }
}
