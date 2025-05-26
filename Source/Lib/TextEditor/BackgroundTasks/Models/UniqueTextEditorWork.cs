using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;

public class UniqueTextEditorWork : IBackgroundTaskGroup
{
    private readonly Func<TextEditorEditContext, ValueTask> _textEditorFunc;

    public UniqueTextEditorWork(
        TextEditorService textEditorService,
        Func<TextEditorEditContext, ValueTask> textEditorFunc)
    {
        _textEditorFunc = textEditorFunc;
        TextEditorService = textEditorService;
    }

    public Key<IBackgroundTaskGroup> BackgroundTaskKey { get; set; } = Key<IBackgroundTaskGroup>.Empty;
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
