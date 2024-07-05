using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
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
        ThrottleTimeSpan = throttleTimeSpan ?? TextEditorComponentData.ThrottleDelayDefault;
    }

	public string Name { get; set; }
    public Key<IBackgroundTask> BackgroundTaskKey { get; set; } = Key<IBackgroundTask>.NewKey();
    public Key<IBackgroundTaskQueue> QueueKey { get; set; } = ContinuousBackgroundTaskWorker.GetQueueKey();
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
		// TODO: This 'SimpleBatchTextEditorTask' is causing a great deal of bugs...
		//       ...because if one of the batched tasks fails, then the others are not executed,
		//       and yet some of the not executed tasks were 1 opportunity tasks that will
		//       never be asked to run again. Just get rid of this class entirely?
		//       Because even if I used a try catch inside the foreach loop,
		//       should I even do that? The EditContext would've been partially modified,
		//       by the inner edit that threw an exception, and thus be in a corrupt state.
		//       What I'm really looking for with this class, is to avoid the UI re-rendering
		//       between all the edits unnecessarily.
		//
		//       The actually desired behavior for this class would be achieved by
		//       telling the ITextEditorService to invoke an enumerable of text editor edits,
		//       prior to re-rendering.
		//
		//       This being in place of the current ITextEditorService code which is
		//       re-rendering after every text editor edit.
		//
		// 	  But the takeaway here is that this type has "less" influence on the enumerable.
		//       Maybe it stores the enumerable, but at the end of the day its the ITextEditorService
		//       that will do the iteration.
		//
		//       And yet, if an inner edit throws an exception, how does the ITextEditorService
		//       restore the EditContext to what it was prior to invoking the edit that
		//       threw an exception.
		//
		//       Perhaps more effort should be put into making the IBackgroundTask
		//       in and of itself more optimized, rather than trying to batch arbitrary
		//       tasks together?
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
