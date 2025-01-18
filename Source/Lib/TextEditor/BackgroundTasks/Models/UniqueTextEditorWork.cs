using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

namespace Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;

/// <summary>
/// Given two contiguous background tasks. If either, or both, of the
/// two are of this type, they will NOT be "batched"/"merged" into a singular task.
/// </summary>
/// <remarks>
/// For further control over the batching, one needs to implement <see cref="ITextEditorTask"/>
/// and implement the method: <see cref="IBackgroundTask.BatchOrDefault"/>.
/// </remarks>
public struct UniqueTextEditorWork : ITextEditorWork
{
    private readonly Func<ITextEditorEditContext, Task> _textEditorFunc;

    public UniqueTextEditorWork(
        string name,
        ITextEditorService textEditorService,
        Func<ITextEditorEditContext, Task> textEditorFunc)
    {
        _textEditorFunc = textEditorFunc;
        Name = name;
        TextEditorService = textEditorService;
    }

	public string Name { get; set; }
    public Key<IBackgroundTask> BackgroundTaskKey { get; set; } = Key<IBackgroundTask>.NewKey();
    public Key<IBackgroundTaskQueue> QueueKey { get; set; } = ContinuousBackgroundTaskWorker.GetQueueKey();
    public bool EarlyBatchEnabled { get; set; }
    public bool LateBatchEnabled { get; set; }
    public ITextEditorService TextEditorService { get; }

	public ITextEditorEditContext EditContext { get; private set; }

    public IBackgroundTask? EarlyBatchOrDefault(IBackgroundTask oldEvent)
    {
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
