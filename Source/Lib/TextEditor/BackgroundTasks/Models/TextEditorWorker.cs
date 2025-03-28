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
public class TextEditorWorker : IBackgroundTaskGroup
{
	private readonly object _workKindQueueLock = new();
	private readonly ITextEditorService _textEditorService;
	
	public TextEditorWorker(ITextEditorService textEditorService)
	{
		_textEditorService = textEditorService;
	}
	
	private bool _taskCompletionSourceWasCreated;

	public Key<IBackgroundTask> BackgroundTaskKey { get; } = Key<IBackgroundTask>.NewKey();
    public Key<IBackgroundTaskQueue> QueueKey { get; } = BackgroundTaskFacts.ContinuousQueueKey;
    
    // Nervous about this not being considered an interpolated constant string.
    public string Name { get; } = "TextEditorWorker";
    
    public bool EarlyBatchEnabled { get; } = false;
    
    public bool __TaskCompletionSourceWasCreated
    {
    	get => _taskCompletionSourceWasCreated;
    	set => _ = value;
    }
    
    public Queue<RedundantTextEditorWork> RedundantTextEditorWorkQueue { get; } = new();
    public Queue<UniqueTextEditorWork> UniqueTextEditorWorkQueue { get; } = new();
    public Queue<OnDoubleClick> OnDoubleClickQueue { get; } = new();
    public Queue<OnKeyDown> OnKeyDownQueue { get; } = new();
	public Queue<OnMouseDown> OnMouseDownQueue { get; } = new();
    public Queue<OnMouseMove> OnMouseMoveQueue { get; } = new();
    public Queue<OnScrollHorizontal> OnScrollHorizontalQueue { get; } = new();
	public Queue<OnScrollVertical> OnScrollVerticalQueue { get; } = new();
	public Queue<OnWheel> OnWheelQueue { get; } = new();
	public Queue<OnWheelBatch> OnWheelBatchQueue { get; } = new();
	
	/// <summary>
	/// If multiple EventKind of the same are enqueued one after another then
	/// better to have this Queue be a struct that has the count of contiguous work kind enqueues?
	/// </summary>
	public Queue<TextEditorWorkKind> WorkKindQueue { get; } = new();
	
	public IBackgroundTask? EarlyBatchOrDefault(IBackgroundTask oldEvent)
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
			WorkKindQueue.Enqueue(TextEditorWorkKind.RedundantTextEditorWork);
			RedundantTextEditorWorkQueue.Enqueue(redundantTextEditorWork);
			_textEditorService.BackgroundTaskService.EnqueueGroup(this);
		}
	}
	
	public Task EnqueueTextEditorWorkAsync(AsyncTextEditorWork asyncTextEditorWork)
	{
		return _textEditorService.BackgroundTaskService.EnqueueAsync(asyncTextEditorWork);
	}
	
	public void EnqueueUniqueTextEditorWork(UniqueTextEditorWork uniqueTextEditorWork)
	{
		lock (_workKindQueueLock)
		{
			WorkKindQueue.Enqueue(TextEditorWorkKind.UniqueTextEditorWork);
			UniqueTextEditorWorkQueue.Enqueue(uniqueTextEditorWork);
			_textEditorService.BackgroundTaskService.EnqueueGroup(this);
		}
	}
	
	public void EnqueueOnDoubleClick(OnDoubleClick onDoubleClick)
	{
		lock (_workKindQueueLock)
		{
			WorkKindQueue.Enqueue(TextEditorWorkKind.OnDoubleClick);
			OnDoubleClickQueue.Enqueue(onDoubleClick);
			_textEditorService.BackgroundTaskService.EnqueueGroup(this);
		}
	}
	
	public void EnqueueOnKeyDown(OnKeyDown onKeyDown)
	{
		lock (_workKindQueueLock)
		{
			WorkKindQueue.Enqueue(TextEditorWorkKind.OnKeyDown);
			OnKeyDownQueue.Enqueue(onKeyDown);
			_textEditorService.BackgroundTaskService.EnqueueGroup(this);
		}
	}
	
	public void EnqueueOnMouseDown(OnMouseDown onMouseDown)
	{
		lock (_workKindQueueLock)
		{
			WorkKindQueue.Enqueue(TextEditorWorkKind.OnMouseDown);
			OnMouseDownQueue.Enqueue(onMouseDown);
			_textEditorService.BackgroundTaskService.EnqueueGroup(this);
		}
	}
	
	public void EnqueueOnMouseMove(OnMouseMove onMouseMove)
	{
		lock (_workKindQueueLock)
		{
			WorkKindQueue.Enqueue(TextEditorWorkKind.OnMouseMove);
			OnMouseMoveQueue.Enqueue(onMouseMove);
			_textEditorService.BackgroundTaskService.EnqueueGroup(this);
		}
	}
	
	public void EnqueueOnScrollHorizontal(OnScrollHorizontal onScrollHorizontal)
	{
		lock (_workKindQueueLock)
		{
			WorkKindQueue.Enqueue(TextEditorWorkKind.OnScrollHorizontal);
			OnScrollHorizontalQueue.Enqueue(onScrollHorizontal);
			_textEditorService.BackgroundTaskService.EnqueueGroup(this);
		}
	}
	
	public void EnqueueOnScrollVertical(OnScrollVertical onScrollVertical)
	{
		lock (_workKindQueueLock)
		{
			WorkKindQueue.Enqueue(TextEditorWorkKind.OnScrollVertical);
			OnScrollVerticalQueue.Enqueue(onScrollVertical);
			_textEditorService.BackgroundTaskService.EnqueueGroup(this);
		}
	}
	
	public void EnqueueOnWheel(OnWheel onWheel)
	{
		lock (_workKindQueueLock)
		{
			WorkKindQueue.Enqueue(TextEditorWorkKind.OnWheel);
			OnWheelQueue.Enqueue(onWheel);
			_textEditorService.BackgroundTaskService.EnqueueGroup(this);
		}
	}
	
	public void EnqueueOnWheelBatch(OnWheelBatch onWheelBatch)
	{
		lock (_workKindQueueLock)
		{
			WorkKindQueue.Enqueue(TextEditorWorkKind.OnWheelBatch);
			OnWheelBatchQueue.Enqueue(onWheelBatch);
			_textEditorService.BackgroundTaskService.EnqueueGroup(this);
		}
	}
	
	public ValueTask HandleEvent(CancellationToken cancellationToken)
	{
		TextEditorWorkKind workKind;
	
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
			case TextEditorWorkKind.RedundantTextEditorWork:
				var redundantTextEditorWork = RedundantTextEditorWorkQueue.Dequeue();
				_taskCompletionSourceWasCreated = redundantTextEditorWork.__TaskCompletionSourceWasCreated;
				return redundantTextEditorWork.HandleEvent(cancellationToken);
			case TextEditorWorkKind.UniqueTextEditorWork:
				var uniqueTextEditorWork = UniqueTextEditorWorkQueue.Dequeue();
				_taskCompletionSourceWasCreated = uniqueTextEditorWork.__TaskCompletionSourceWasCreated;
				return uniqueTextEditorWork.HandleEvent(cancellationToken);
			case TextEditorWorkKind.OnDoubleClick:
				var onDoubleClick = OnDoubleClickQueue.Dequeue();
				_taskCompletionSourceWasCreated = false;
				return onDoubleClick.HandleEvent(cancellationToken);
		    case TextEditorWorkKind.OnKeyDown:
		    	var onKeyDown = OnKeyDownQueue.Dequeue();
				_taskCompletionSourceWasCreated = false;
				return onKeyDown.HandleEvent(cancellationToken);
			case TextEditorWorkKind.OnMouseDown:
				var onMouseDown = OnMouseDownQueue.Dequeue();
				_taskCompletionSourceWasCreated = false;
				return onMouseDown.HandleEvent(cancellationToken);
		    case TextEditorWorkKind.OnMouseMove:
		    	var onMouseMove = OnMouseMoveQueue.Dequeue();
				_taskCompletionSourceWasCreated = false;
				return onMouseMove.HandleEvent(cancellationToken);
		    case TextEditorWorkKind.OnScrollHorizontal:
		    	var onScrollHorizontal = OnScrollHorizontalQueue.Dequeue();
				_taskCompletionSourceWasCreated = false;
				return onScrollHorizontal.HandleEvent(cancellationToken);
			case TextEditorWorkKind.OnScrollVertical:
		    	var onScrollVertical = OnScrollVerticalQueue.Dequeue();
				_taskCompletionSourceWasCreated = false;
				return onScrollVertical.HandleEvent(cancellationToken);
			case TextEditorWorkKind.OnWheel:
		    	var onWheel = OnWheelQueue.Dequeue();
				_taskCompletionSourceWasCreated = false;
				return onWheel.HandleEvent(cancellationToken);
			case TextEditorWorkKind.OnWheelBatch:
		    	var onWheelBatch = OnWheelBatchQueue.Dequeue();
				_taskCompletionSourceWasCreated = false;
				return onWheelBatch.HandleEvent(cancellationToken);
			default:
				Console.WriteLine($"{nameof(TextEditorWorker)} {nameof(HandleEvent)} default case");
				return ValueTask.CompletedTask;
		}
	}
}
