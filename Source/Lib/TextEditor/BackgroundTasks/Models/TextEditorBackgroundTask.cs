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
	private readonly object _lockWork = new();

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
    public string Name => nameof(TextEditorBackgroundTask) + ' ' + _workList.Count;
    public Task? WorkProgress { get; }
	public TimeSpan ThrottleTimeSpan { get; }
	public bool WasEnqueued { get; private set; }
	public bool IsCompleted { get; private set; }

	/// <summary>
	/// The group of properties which provide data on which
	/// the <see cref="_workList"/> performs work.
	/// </summary>
	public ITextEditorService TextEditorService { get; }
	public IEditContext? EditContext { get; set; }

	public bool TryReusingSameInstance(ITextEditorWork downstreamWork)
	{
		Console.WriteLine("TryReusingSameInstance");

		var wasBatched = false;
		var wasAdded = false;

		lock (_lockWork)
		{
			Console.WriteLine($"if (!WasEnqueued): if ({!WasEnqueued})");
			if (!WasEnqueued && !IsCompleted)
			{
				var precedentWork = _workList.Last();
	
				if (precedentWork.TextEditorWorkKind == downstreamWork.TextEditorWorkKind)
				{
					var batchedWork = downstreamWork.BatchEnqueue(precedentWork);

					if (batchedWork is not null)
					{
						_workList[^1] = batchedWork;
						wasBatched = true;
					}
				}

				if (!wasBatched)
				{
					_workList.Add(downstreamWork);
					wasAdded = true;
				}
			}
		}

		var success = wasBatched || wasAdded;
		return success;
	}

	public IBackgroundTask? BatchOrDefault(IBackgroundTask precedentTask)
	{
		lock (_lockWork)
		{
			// This method is invoked from within a semaphore, of which blocks the queue from being
			// processed. So, if we see that the last enqueued item is of this same 'Type',
			// we can share the 'ITextEditorEditContext'
			//
			// By sharing the 'EditContext' amongst two tasks, we reduce 2 UI renders down to only 1.
			if (precedentTask is TextEditorBackgroundTask precedentTextEditorBackgroundTask)
			{
				var precedentWork = precedentTextEditorBackgroundTask._workList.Last();
				var downstreamWork = _workList.First();
	
				if (precedentWork.TextEditorWorkKind == downstreamWork.TextEditorWorkKind)
				{
					var batchedWork = downstreamWork.BatchEnqueue(precedentWork);

					if (batchedWork is not null)
						precedentTextEditorBackgroundTask._workList[^1] = batchedWork;
				}
	
				precedentTextEditorBackgroundTask._workList.AddRange(_workList);
				return precedentTextEditorBackgroundTask;
			}
			else
			{
				WasEnqueued = true;
				return null;
			}
		}
	}

	public IBackgroundTask? DequeueBatchOrDefault(IBackgroundTask precedentTask)
	{
		Console.WriteLine($"DequeueBatchOrDefault_workList.Count: {_workList.Count}");
		return null;
	}
	
	public async Task HandleEvent(CancellationToken cancellationToken)
	{
		lock (_lockWork)
		{
			IsCompleted = true;
		}

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
			
			workDue.Invoke(EditContext).ConfigureAwait(false);
		}

		await TextEditorService.CloseEditContext(EditContext);
	}
}
