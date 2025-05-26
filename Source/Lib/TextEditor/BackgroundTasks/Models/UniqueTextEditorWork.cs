using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;

/// <summary>
/// Given two contiguous background tasks. If either, or both, of the
/// two are of this type, they will NOT be "batched"/"merged" into a singular task.
/// </summary>
/// <remarks>
/// For further control over the batching, one needs to implement <see cref="ITextEditorTask"/>
/// and implement the method: <see cref="IBackgroundTask.BatchOrDefault"/>.
/// </remarks>
public class UniqueTextEditorWork : IBackgroundTaskGroup
{
    private readonly Func<TextEditorEditContext, ValueTask> _textEditorFunc;

    public UniqueTextEditorWork(
        string name,
        TextEditorService textEditorService,
        Func<TextEditorEditContext, ValueTask> textEditorFunc)
    {
        _textEditorFunc = textEditorFunc;
        Name = name;
        TextEditorService = textEditorService;
    }

	public string Name { get; set; }
    public Key<IBackgroundTaskGroup> BackgroundTaskKey { get; set; } = Key<IBackgroundTaskGroup>.Empty;
    public bool EarlyBatchEnabled { get; set; }
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
