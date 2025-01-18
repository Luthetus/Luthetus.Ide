using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

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
/// For further control over the batching, one needs to implement <see cref="ITextEditorWork"/>
/// and implement the method: <see cref="IBackgroundTask.BatchOrDefault"/>.
/// </remarks>
public struct RedundantTextEditorWork : ITextEditorWork
{
    private readonly Func<ITextEditorEditContext, ValueTask> _textEditorFunc;

    public RedundantTextEditorWork(
        string name,
        ResourceUri resourceUri,
        Key<TextEditorViewModel> viewModelKey,
        ITextEditorService textEditorService,
        Func<ITextEditorEditContext, ValueTask> textEditorFunc)
    {
        _textEditorFunc = textEditorFunc;

        Name = name;
        ResourceUri = resourceUri;
        ViewModelKey = viewModelKey;
        
        TextEditorService = textEditorService;
    }

	public string Name { get; set; }
	public ResourceUri ResourceUri { get; set; }
    public Key<TextEditorViewModel> ViewModelKey { get; set; }
    public Key<IBackgroundTask> BackgroundTaskKey { get; set; } = Key<IBackgroundTask>.NewKey();
    public Key<IBackgroundTaskQueue> QueueKey { get; set; } = ContinuousBackgroundTaskWorker.GetQueueKey();
    public bool EarlyBatchEnabled { get; set; } = true;
    public bool LateBatchEnabled { get; set; }
    public ITextEditorService TextEditorService { get; }

	public ITextEditorEditContext EditContext { get; private set; }

    public IBackgroundTask? EarlyBatchOrDefault(IBackgroundTask oldEvent)
    {
        if (oldEvent is not RedundantTextEditorWork oldRedundantTextEditorWork)
        {
            // Keep both events
            return null;
        }

        if (oldRedundantTextEditorWork.Name == Name &&
		    oldRedundantTextEditorWork.ResourceUri == ResourceUri &&
            oldRedundantTextEditorWork.ViewModelKey == ViewModelKey)
        {
            // Keep this event (via replacement)
            return this;
        }

        // Keep both events
        return null;
    }
    
    public IBackgroundTask? LateBatchOrDefault(IBackgroundTask oldEvent)
    {
    	return null;
    }

    public async ValueTask HandleEvent(CancellationToken cancellationToken)
    {
    	EditContext = new TextEditorService.TextEditorEditContext(
            TextEditorService,
            Luthetus.TextEditor.RazorLib.TextEditorService.AuthenticatedActionKey);
    
		await _textEditorFunc
            .Invoke(EditContext)
            .ConfigureAwait(false);
            
        await EditContext.TextEditorService
        	.FinalizePost(EditContext)
        	.ConfigureAwait(false);
    }
}
