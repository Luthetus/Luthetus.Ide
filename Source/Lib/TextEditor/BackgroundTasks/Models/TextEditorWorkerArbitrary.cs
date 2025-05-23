using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Events.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;

/// <summary>
/// Just use the same instance of this everytime you need to enqueue?
///
/// All this instance does is communicate to the 'ITextEditorService'
/// when it is their "turn" to run code on the background task worker.
///
/// At that point the ITextEditorService dequeues from its own queue
/// (which is more optimized) until it finds a non-match contiguous EventKind.
///
/// At that point there is an IEnumerable<"EventData"> that can
/// be batched.
///
/// After all the dequeues of the same EventKind are handled
/// then return back to the IBackgroundTaskService.
///
/// By doing this the 'IBackgroundTaskService' will
/// have many references to this type in the queue
/// in the sense that,
///
/// 6 events are enqueued,
/// 6 references to the same TextEditorWorker are enqueued
/// All 6 events are batched into a single work item.
/// There are still 5 more references to this TextEditorWorker enqueued
/// Each one when dequeued returns Task.CompletedTask because the previous work item
/// encompassed all 6 events.
///
/// You have to always enqueue a reference to this because you are dequeue batching
/// you don't know if the EventKinds will match when enqueueing.
/// 
/// ???
///
/// The ITextEditorService has a queue foreach of the 'EventData kinds'
/// then one queue that every event queues to that says that something happened
/// and where the event data can be dequeued from.
/// </summary>
public class TextEditorWorkerArbitrary : IBackgroundTaskGroup
{
	private readonly object _workKindQueueLock = new();
	private readonly TextEditorService _textEditorService;
	
	public TextEditorWorkerArbitrary(TextEditorService textEditorService)
	{
		_textEditorService = textEditorService;
	}
	
	public Key<IBackgroundTaskGroup> BackgroundTaskKey { get; } = Key<IBackgroundTaskGroup>.NewKey();
    public Key<BackgroundTaskQueue> QueueKey { get; } = BackgroundTaskFacts.ContinuousQueueKey;
    
    // Nervous about this not being considered an interpolated constant string.
    public string Name { get; } = "TextEditorWorker";
    
    public bool EarlyBatchEnabled { get; } = false;
    
    public bool __TaskCompletionSourceWasCreated { get; set; }
    
    public Queue<RedundantTextEditorWork> RedundantTextEditorWorkQueue { get; } = new();
    public Queue<UniqueTextEditorWork> UniqueTextEditorWorkQueue { get; } = new();
	
	/// <summary>
	/// If multiple EventKind of the same are enqueued one after another then
	/// better to have this Queue be a struct that has the count of contiguous work kind enqueues?
	/// </summary>
	public Queue<TextEditorWorkArbitraryKind> WorkKindQueue { get; } = new();
	
	public IBackgroundTaskGroup? EarlyBatchOrDefault(IBackgroundTaskGroup oldEvent)
	{
		return null;
	}
	
	public void PostRedundant(
        string name,
		ResourceUri resourceUri,
        Key<TextEditorViewModel> viewModelKey,
        Func<TextEditorEditContext, ValueTask> textEditorFunc)
    {
    	EnqueueRedundantTextEditorWork(
    		new RedundantTextEditorWork(
	            name,
				resourceUri,
	            viewModelKey,
	            _textEditorService,
	            textEditorFunc));
    }
	
	public void PostUnique(
        string name,
        Func<TextEditorEditContext, ValueTask> textEditorFunc)
    {
    	EnqueueUniqueTextEditorWork(
    		new UniqueTextEditorWork(
	            name,
	            _textEditorService,
	            textEditorFunc));
    }
	
	public void EnqueueRedundantTextEditorWork(RedundantTextEditorWork redundantTextEditorWork)
	{
		lock (_workKindQueueLock)
		{
			WorkKindQueue.Enqueue(TextEditorWorkArbitraryKind.RedundantTextEditorWork);
			RedundantTextEditorWorkQueue.Enqueue(redundantTextEditorWork);
			_textEditorService.BackgroundTaskService.EnqueueGroup(this);
		}
	}
	
	public void EnqueueUniqueTextEditorWork(UniqueTextEditorWork uniqueTextEditorWork)
	{
		lock (_workKindQueueLock)
		{
			WorkKindQueue.Enqueue(TextEditorWorkArbitraryKind.UniqueTextEditorWork);
			UniqueTextEditorWorkQueue.Enqueue(uniqueTextEditorWork);
			_textEditorService.BackgroundTaskService.EnqueueGroup(this);
		}
	}
	
	public ValueTask HandleEvent(CancellationToken cancellationToken)
	{
		TextEditorWorkArbitraryKind workKind;
	
		// avoid UI infinite loop enqueue dequeue single work item
		// by getting the count prior to starting the yield return deqeue
		// then only dequeueing at most 'count' times.
		
		lock (_workKindQueueLock)
		{
			if (!WorkKindQueue.TryDequeue(out workKind))
				return ValueTask.CompletedTask;
		}
			
		switch (workKind)
		{
			case TextEditorWorkArbitraryKind.RedundantTextEditorWork:
				return RedundantTextEditorWorkQueue.Dequeue().HandleEvent(cancellationToken);
			case TextEditorWorkArbitraryKind.UniqueTextEditorWork:
				return UniqueTextEditorWorkQueue.Dequeue().HandleEvent(cancellationToken);
			default:
				Console.WriteLine($"{nameof(TextEditorWorkerArbitrary)} {nameof(HandleEvent)} default case");
				return ValueTask.CompletedTask;
		}
	}
}

