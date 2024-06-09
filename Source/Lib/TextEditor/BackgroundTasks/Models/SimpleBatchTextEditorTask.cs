using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;

namespace Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;

/// <summary>
/// This class will take contiguous events of this same type, and combine them to
/// all to run in the same <see cref="IEditContext"/>, provided they share
/// the same <see cref="Identifier"/>.
/// The result of this is that the UI will not be notified to re-render
/// until after the batch has all done their edits.
/// </summary>
public sealed class SimpleBatchTextEditorTask : ITextEditorTask
{
    private readonly List<TextEditorEdit> _textEditorEditList;

    public SimpleBatchTextEditorTask(
            string name,
            TextEditorEdit textEditorEdit,
            TimeSpan? throttleTimeSpan = null)
        : this(name, new List<TextEditorEdit>() { textEditorEdit }, throttleTimeSpan)
    {
    }

    public SimpleBatchTextEditorTask(
        string name,
        List<TextEditorEdit> textEditorEditList,
        TimeSpan? throttleTimeSpan = null)
    {
        _textEditorEditList = textEditorEditList;

        Name = name;
        ThrottleTimeSpan = throttleTimeSpan ?? TextEditorViewModel.ThrottleDelayDefault;
    }

	public string Name { get; set; }
    public Key<BackgroundTask> BackgroundTaskKey { get; set; } = Key<BackgroundTask>.NewKey();
    public Key<BackgroundTaskQueue> QueueKey { get; set; } = ContinuousBackgroundTaskWorker.GetQueueKey();
    public TimeSpan ThrottleTimeSpan { get; set; }
    public Task? WorkProgress { get; set; }

	public IEditContext EditContext { get; set; }

    public IBackgroundTask? BatchOrDefault(IBackgroundTask oldEvent)
    {
        if (oldEvent is not SimpleBatchTextEditorTask oldSimpleBatchTextEditorTask)
            return null;

        oldSimpleBatchTextEditorTask._textEditorEditList.AddRange(_textEditorEditList);
        oldSimpleBatchTextEditorTask.Name += '_' + Name;
        return oldSimpleBatchTextEditorTask;
    }

    public async Task HandleEvent(CancellationToken cancellationToken)
    {
		try
		{
			foreach (var edit in _textEditorEditList)
	        {
	            await edit
	                .Invoke(EditContext)
	                .ConfigureAwait(false);
	        }
		}
		finally
		{
			await EditContext.TextEditorService.FinalizePost(EditContext);
		}
    }
}
