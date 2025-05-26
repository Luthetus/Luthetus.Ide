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
public struct RedundantTextEditorWork : IBackgroundTaskGroup
{
    private readonly Func<TextEditorEditContext, ValueTask> _textEditorFunc;

    public RedundantTextEditorWork(
        string name,
        ResourceUri resourceUri,
        Key<TextEditorViewModel> viewModelKey,
        TextEditorService textEditorService,
        Func<TextEditorEditContext, ValueTask> textEditorFunc)
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
    public Key<IBackgroundTaskGroup> BackgroundTaskKey { get; set; } = Key<IBackgroundTaskGroup>.NewKey();
    public bool EarlyBatchEnabled { get; set; } = true;
    public bool __TaskCompletionSourceWasCreated { get; set; }
    public TextEditorService TextEditorService { get; }

    public async ValueTask HandleEvent(CancellationToken cancellationToken)
    {
    	var editContext = new TextEditorEditContext(TextEditorService);
    
		await _textEditorFunc
            .Invoke(editContext)
            .ConfigureAwait(false);
            
        await editContext.TextEditorService
        	.FinalizePost(editContext)
        	.ConfigureAwait(false);
    }
}
