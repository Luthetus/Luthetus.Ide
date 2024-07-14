using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

namespace Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;

/// <summary>
/// There has existed a type named <see cref="SimpleBatchTextEditorTask"/>
/// for a good bit of time, prior to this comment being written.
///
/// It however is now believed that the <see cref="SimpleBatchTextEditorTask"/> type
/// is an extremely bug-prone idea.
///
/// The following is a comment which is copy and pasted from the <see cref="SimpleBatchTextEditorTask"/>
/// (this comment was made in response to the discovery of how bug-prone the type is).
///
/// #### Start of the referenced comment ####
/// TODO: This 'SimpleBatchTextEditorTask' is causing a great deal of bugs...
///       ...because if one of the batched tasks fails, then the others are not executed,
///       and yet some of the not executed tasks were 1 opportunity tasks that will
///       never be asked to run again. Just get rid of this class entirely?
///       Because even if I used a try catch inside the foreach loop,
///       should I even do that? The EditContext would've been partially modified,
///       by the inner edit that threw an exception, and thus be in a corrupt state.
///       What I'm really looking for with this class, is to avoid the UI re-rendering
///       between all the edits unnecessarily.
///
///       The actually desired behavior for this class would be achieved by
///       telling the ITextEditorService to invoke an enumerable of text editor edits,
///       prior to re-rendering.
///
///       This being in place of the current ITextEditorService code which is
///       re-rendering after every text editor edit.
///
/// 	  But the takeaway here is that this type has "less" influence on the enumerable.
///       Maybe it stores the enumerable, but at the end of the day its the ITextEditorService
///       that will do the iteration.
///
///       And yet, if an inner edit throws an exception, how does the ITextEditorService
///       restore the EditContext to what it was prior to invoking the edit that
///       threw an exception.
///
///       Perhaps more effort should be put into making the IBackgroundTask
///       in and of itself more optimized, rather than trying to batch arbitrary
///       tasks together?
/// #### End of the referenced comment ####
///
/// So, the idea is that a safe implementation of the <see cref="SimpleBatchTextEditorTask"/>
/// either restores the EditContext to the state it was at prior to invoking the edit
/// which threw an exception.
///
/// Or, the if only readonly tasks were allowed to be "simply batched" into a foreach loop,
/// then the worry of a corrupt EditContext is no longer an issue.
///
/// One problem with this readonly task idea, is that the 'readonly' nature of a
/// text editor task is to a degree opt in.
///
/// In a sense its an honor policy, if you might edit the text editor,
/// then you pass editContext.GetModelModifier() the boolean, isReadonly set to false.
///
/// This still seems like a step in the right direction so it will be seen how things go.
///
/// The idea is for the compiler services to use this task when parsing the text editor.
/// As, the only edit they make is to the compiler service presentation layer.
/// Of which, if it encounters an exception, could be made to set the presentation layer
/// to an empty list of text spans, and just use the Lexer output
/// (if the lexer didn't throw an exception, but only the parser did).
/// </summary>
public sealed class ReadOnlyTextEditorTask : ITextEditorTask
{
    private readonly List<TextEditorEdit> _textEditorEditList;

    public ReadOnlyTextEditorTask(
            string name,
            TextEditorEdit textEditorEdit,
            TimeSpan? throttleTimeSpan = null)
        : this(name, new List<TextEditorEdit>() { textEditorEdit }, throttleTimeSpan)
    {
    }

    public ReadOnlyTextEditorTask(
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
        if (oldEvent is not ReadOnlyTextEditorTask oldReadOnlyTextEditorTask)
            return null;

        oldReadOnlyTextEditorTask._textEditorEditList.AddRange(_textEditorEditList);
        oldReadOnlyTextEditorTask.Name += '_' + Name;
        return oldReadOnlyTextEditorTask;
    }

    public async Task HandleEvent(CancellationToken cancellationToken)
    {
		try
		{
			foreach (var edit in _textEditorEditList)
	        {
				try
				{
					await edit
		                .Invoke(EditContext)
		                .ConfigureAwait(false);
				}
				catch (Exception)
				{
					// Console.WriteLine($"{nameof(ReadOnlyTextEditorTask)}: {e.ToString()}");
					// TODO: Eating the exception is odd
				}
	        }
		}
		finally
		{
			await EditContext.TextEditorService.FinalizePost(EditContext);
		}
    }
}

