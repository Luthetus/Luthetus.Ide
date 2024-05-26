using System.Text;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;

namespace Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;

public class TextEditorBackgroundTask : IBackgroundTask
{
	public readonly List<ITextEditorWork> _workList = new();

	public TextEditorBackgroundTask(ITextEditorService textEditorService, ITextEditorWork work)
	{
		TextEditorService = textEditorService;
		_workList.Add(work);
	}

	/// <summary>
	/// The group of properties which are implemented from 'IBackgroundTask'
	/// </summary>
	public Key<BackgroundTask> BackgroundTaskKey { get; }
    public Key<BackgroundTaskQueue> QueueKey { get; } = ContinuousBackgroundTaskWorker.GetQueueKey();
    public string Name => nameof(TextEditorBackgroundTask) + ' ' + string.Join(' ', _workList.Select(x => x.Name));
    public Task? WorkProgress { get; }
	public TimeSpan ThrottleTimeSpan { get; } = TimeSpan.Zero;

	/// <summary>
	/// The group of properties which provide data on which
	/// the <see cref="_workList"/> performs work.
	/// </summary>
	public ITextEditorService TextEditorService { get; }
	public IEditContext? EditContext { get; set; }

	public IBackgroundTask? BatchOrDefault(IBackgroundTask upstreamTask)
	{
		// This method is invoked from within a semaphore, of which blocks the queue from being
		// processed. So, if we see that the last enqueued item is of this same 'Type',
		// we can share the 'ITextEditorEditContext'
		//
		// By sharing the 'EditContext' amongst two tasks, we reduce 2 UI renders down to only 1.
		if (upstreamTask is TextEditorBackgroundTask upstreamTextEditorBackgroundTask &&
			_workList.Count == 1)
		{
			var upstreamWork = upstreamTextEditorBackgroundTask._workList.Last();
			var downstreamWork = _workList.Single();

			var wasBatched = false;
			if (upstreamWork.TextEditorWorkKind == downstreamWork.TextEditorWorkKind)
			{
				var batchedWork = downstreamWork.BatchEnqueue(upstreamWork);

				if (batchedWork is not null)
				{
					upstreamTextEditorBackgroundTask._workList[^1] = batchedWork;
					wasBatched = true;
				}
			}

			if (!wasBatched)
				upstreamTextEditorBackgroundTask._workList.Add(downstreamWork);

			return upstreamTextEditorBackgroundTask;
		}
		else
		{
			return null;
		}
	}
	
	public async Task HandleEvent(CancellationToken cancellationToken)
	{
		EditContext ??= TextEditorService.OpenEditContext();

		for (var i = 0; i < _workList.Count; i++)
		{
			var workCurrent = _workList[i];

			ITextEditorWork workDue;
			
			while (true)
			{
				if (i == _workList.Count - 1)
				{
					workDue = workCurrent;
					break;
				}

				var workDownstream = _workList[i + 1];
				var workBatched = workDownstream.BatchDequeue(EditContext, workCurrent);

				if (workBatched is null)
				{
					workDue = workCurrent;
					break;
				}
				else
				{
					workCurrent = workBatched;
					i++;
				}
			}
			
			workDue.Invoke(EditContext);
		}

		await TextEditorService.CloseEditContext(EditContext);
	}
}
