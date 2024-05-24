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
    public string Name { get; }
    public Task? WorkProgress { get; }
	public TimeSpan ThrottleTimeSpan { get; }
	public bool WasEnqueued { get; private set; }

	/// <summary>
	/// The group of properties which provide data on which
	/// the <see cref="_workList"/> performs work.
	/// </summary>
	public ITextEditorService TextEditorService { get; }
	public IEditContext? EditContext { get; set; }

	public bool TryReusingSameInstance(ITextEditorWork work)
	{
		var wasAdded = false;

		lock (_lockWork)
		{
			if (!WasEnqueued)
			{
				var lastWork = _workList.Last();
				var newWork = work;
	
				if (lastWork.TextEditorWorkKind == newWork.TextEditorWorkKind)
				{
					if (lastWork.ResourceUri == newWork.ResourceUri &&
						lastWork.CursorKey == newWork.CursorKey)
					{
						switch (lastWork.TextEditorWorkKind)
						{
							case TextEditorWorkKind.Insertion:
								var insertionLastWork = (TextEditorWorkInsertion)lastWork;
								var insertionNewWork = (TextEditorWorkInsertion)newWork;
		
								insertionLastWork.Content.Append(insertionNewWork.Content);
								wasAdded = true;
								break;
							case TextEditorWorkKind.Deletion:
								var deletionLastWork = (TextEditorWorkDeletion)lastWork;
								var deletionNewWork = (TextEditorWorkDeletion)newWork;
		
								deletionLastWork.ColumnCount += deletionNewWork.ColumnCount;
								wasAdded = true;
								break;
						}
					}
				}

				if (!wasAdded)
				{
					_workList.Add(work);
					wasAdded = true;
				}
			}
		}

		Console.WriteLine($"wasAdded: {wasAdded}");
		Console.WriteLine($"_workList.Count: {_workList.Count}");
		return wasAdded;
	}

	public IBackgroundTask? BatchOrDefault(IBackgroundTask oldEvent)
	{
		lock (_lockWork)
		{
			Console.WriteLine($"BatchOrDefault");
			// This method is invoked from within a semaphore,
			// of which blocks the queue from being processed.
			//
			// So, if we see that the last enqueued item
			// is of this same 'Type', we can share the 'ITextEditorEditContext'
			//
			// By sharing the 'EditContext' amongst two tasks, we reduce
			// 2 UI renders down to only 1.
			if (oldEvent is TextEditorBackgroundTask oldTextEditorBackgroundTask)
			{
				Console.WriteLine($"oldTextEditorBackgroundTask._workList.Count: {oldTextEditorBackgroundTask._workList.Count}");
				Console.WriteLine($"_workList.Count: {_workList.Count}");

				var lastWork = oldTextEditorBackgroundTask._workList.Last();
				var newWork = _workList.First();
	
				if (lastWork.TextEditorWorkKind == newWork.TextEditorWorkKind)
				{
					if (lastWork.ResourceUri == newWork.ResourceUri &&
						lastWork.CursorKey == newWork.CursorKey)
					{
						switch (lastWork.TextEditorWorkKind)
						{
							case TextEditorWorkKind.Insertion:
								var insertionLastWork = (TextEditorWorkInsertion)lastWork;
								var insertionNewWork = (TextEditorWorkInsertion)newWork;
		
								insertionLastWork.Content.Append(insertionNewWork.Content);
								_workList.RemoveAt(0);
								break;
							case TextEditorWorkKind.Deletion:
								var deletionLastWork = (TextEditorWorkDeletion)lastWork;
								var deletionNewWork = (TextEditorWorkDeletion)newWork;
		
								deletionLastWork.ColumnCount += deletionNewWork.ColumnCount;
								_workList.RemoveAt(0);
								break;
						}
					}
				}
	
				oldTextEditorBackgroundTask._workList.AddRange(_workList);

				Console.WriteLine($"2oldTextEditorBackgroundTask._workList.Count: {oldTextEditorBackgroundTask._workList.Count}");
				Console.WriteLine($"2_workList.Count: {_workList.Count}");
				return oldTextEditorBackgroundTask;
			}
			else
			{
				WasEnqueued = true;
				return null;
			}
		}
	}
	
	public async Task HandleEvent(CancellationToken cancellationToken)
	{
		Console.WriteLine("backgroundTask-HandleEvent");

		if (TextEditorService is null)
		{
			Console.WriteLine("backgroundTask-TextEditorService-wasNull");
		}

		EditContext ??= TextEditorService.OpenEditContext();

		Console.WriteLine("backgroundTask-AfterOpenEditContext");

		foreach (var work in _workList)
		{
			await work.Invoke(EditContext).ConfigureAwait(false);
		}

		await TextEditorService.CloseEditContext(EditContext);
	}
}
