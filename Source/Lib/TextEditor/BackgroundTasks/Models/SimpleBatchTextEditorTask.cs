using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;

/// <summary>
/// This class will take contiguous events of this same type, and combine them to
/// all to run in the same <see cref="IEditContext"/>, provided they share
/// the same <see cref="Identifier"/>.
/// The result of this is that the UI will not be notified to re-render
/// until after the batch has all done their edits.
/// </summary>
public class SimpleBatchTextEditorTask : ITextEditorTask
{
    protected readonly List<ITextEditorTask> _textEditorTaskList = new();

    public SimpleBatchTextEditorTask(
            string name,
            string identifier,
            string? redundancy,
            TextEditorEdit textEditorEdit,
            TimeSpan? throttleTimeSpan = null)
    {
		Name = name;
        Identifier = identifier;
        Redundancy = redundancy;
		Edit = textEditorEdit;
        ThrottleTimeSpan = throttleTimeSpan ?? TextEditorViewModelDisplay.TextEditorEvents.ThrottleDelayDefault;
    }

	public virtual string Name { get; protected set; }
    public virtual string Identifier { get; protected set; }
    public virtual string? Redundancy { get; protected set; }
	public TextEditorEdit Edit { get; }
    public virtual Key<BackgroundTask> BackgroundTaskKey { get; protected set; } = Key<BackgroundTask>.NewKey();
    public virtual Key<BackgroundTaskQueue> QueueKey { get; protected set; } = ContinuousBackgroundTaskWorker.GetQueueKey();
    public virtual TimeSpan ThrottleTimeSpan { get; protected set; }
    public virtual Task? WorkProgress { get; protected set; }

    public async Task InvokeWithEditContext(IEditContext editContext)
    {
		// First, await self
		await Edit.Invoke(editContext).ConfigureAwait(false);

		// Then await others
        foreach (var textEditorTask in _textEditorTaskList)
        {
            await textEditorTask.Edit
                .Invoke(editContext)
                .ConfigureAwait(false);
        }
    }

    public virtual IBackgroundTask? BatchOrDefault(IBackgroundTask oldEvent)
    {
        if (oldEvent is SimpleBatchTextEditorTask oldSimpleBatchTextEditorTask)
		{
			if (Redundancy is not null)
			{
				if (oldSimpleBatchTextEditorTask._textEditorTaskList.Count == 0 &&
					Redundancy == oldSimpleBatchTextEditorTask.Redundancy)
				{
					return this;
				}
				else if (Redundancy == oldSimpleBatchTextEditorTask._textEditorTaskList.Last().Redundancy)
				{
					oldSimpleBatchTextEditorTask._textEditorTaskList[^1] = this;
					return oldSimpleBatchTextEditorTask;
				}
			}
			else
			{
				oldSimpleBatchTextEditorTask._textEditorTaskList.AddRange(_textEditorTaskList);
		        oldSimpleBatchTextEditorTask.Name += '_' + Name;
		        return oldSimpleBatchTextEditorTask;
			}
		}

        return null;
    }

    public virtual Task HandleEvent(CancellationToken cancellationToken)
    {
        throw new NotImplementedException($"{nameof(ITextEditorTask)} should not implement {nameof(HandleEvent)}" +
            "because they instead are contained within an 'IBackgroundTask' that came from the 'TextEditorService'");
    }
}
