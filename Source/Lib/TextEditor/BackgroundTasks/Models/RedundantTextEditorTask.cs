using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

namespace Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;

/// <summary>
/// Given two contiguous background tasks. If both of the tasks are of this type,
/// then compare their <see cref="Name"/>, <see cref="ResourceUri"/>,
/// and <see cref="ViewModelKey"/> against eachother.
///
/// If all the identifying properties are equal, then the "upstream"/"first to occurrance"
/// task will be REMOVED from the background task queue, and NEVER be invoked.
///
/// In its place will be the "downstream"/"more recent occurrance" task.
///
/// The reason for this behavior, is that it would be redundant to calculate
/// the upstream event, because the next event is of the same kind, and on the same
/// data, and will entirely overwrite the upstream event's result.
/// </summary>
/// <remarks>
/// For further control over the batching, one needs to implement <see cref="ITextEditorTask"/>
/// and implement the method: <see cref="IBackgroundTask.BatchOrDefault"/>.
/// </remarks>
public sealed class RedundantTextEditorTask : ITextEditorTask
{
    private readonly TextEditorEditAsync _textEditorEditAsync;

    public RedundantTextEditorTask(
        string name,
        ResourceUri resourceUri,
        Key<TextEditorViewModel> viewModelKey,
        TextEditorEditAsync textEditorEditAsync,
        TimeSpan? throttleTimeSpan = null)
    {
        _textEditorEditAsync = textEditorEditAsync;

        Name = name;
        ResourceUri = resourceUri;
        ViewModelKey = ViewModelKey;
        ThrottleTimeSpan = throttleTimeSpan ?? TextEditorComponentData.ThrottleDelayDefault;
    }

	public string Name { get; set; }
	public ResourceUri ResourceUri { get; set; }
    public Key<TextEditorViewModel> ViewModelKey { get; set; }
    public Key<IBackgroundTask> BackgroundTaskKey { get; set; } = Key<IBackgroundTask>.NewKey();
    public Key<IBackgroundTaskQueue> QueueKey { get; set; } = ContinuousBackgroundTaskWorker.GetQueueKey();
    public TimeSpan ThrottleTimeSpan { get; set; }
    public Task? WorkProgress { get; set; }

	public IEditContext EditContext { get; set; }

    public IBackgroundTask? BatchOrDefault(IBackgroundTask oldEvent)
    {
        if (oldEvent is not RedundantTextEditorTask oldRedundantTextEditorTask)
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
            await _textEditorEditAsync
                .Invoke(EditContext)
                .ConfigureAwait(false);
                
            await EditContext.TextEditorService
            	.FinalizePost(EditContext)
            	.ConfigureAwait(false);
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
		}
    }
}
