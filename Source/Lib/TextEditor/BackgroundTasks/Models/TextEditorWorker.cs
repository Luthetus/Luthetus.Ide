using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Events.Models;

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
public class TextEditorWorker : IBackgroundTask
{
	public Key<IBackgroundTask> BackgroundTaskKey { get; }
    public Key<IBackgroundTaskQueue> QueueKey { get; }
    public string Name { get; }
    public bool EarlyBatchEnabled { get; } = false;
    public bool __TaskCompletionSourceWasCreated { get; set; }
    
    public Queue<OnDoubleClick> OnDoubleClickQueue { get; } = new();
    public Queue<OnKeyDownLateBatching> OnKeyDownLateBatchingQueue { get; } = new();
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
	
	public ValueTask HandleEvent(CancellationToken cancellationToken)
	{
		if (!WorkKindQueue.TryDequeue(out var workKind))
			return ValueTask.CompletedTask;
			
		switch (workKind)
		{
			case TextEditorWorkKind.OnDoubleClick:
				var onDoubleClick = OnDoubleClickQueue.Dequeue();
				return onDoubleClick.HandleEvent(cancellationToken);
		    case TextEditorWorkKind.OnKeyDownLateBatching:
		    	var onKeyDownLateBatching = OnKeyDownLateBatchingQueue.Dequeue();
				return onKeyDownLateBatching.HandleEvent(cancellationToken);
			case TextEditorWorkKind.OnMouseDown:
				var onMouseDown = OnMouseDownQueue.Dequeue();
				return onMouseDown.HandleEvent(cancellationToken);
		    case TextEditorWorkKind.OnMouseMove:
		    	var onMouseMove = OnMouseMoveQueue.Dequeue();
				return onMouseMove.HandleEvent(cancellationToken);
		    case TextEditorWorkKind.OnScrollHorizontal:
		    	var onScrollHorizontal = OnScrollHorizontalQueue.Dequeue();
				return onScrollHorizontal.HandleEvent(cancellationToken);
			case TextEditorWorkKind.OnScrollVertical:
		    	var onScrollVertical = OnScrollVerticalQueue.Dequeue();
				return onScrollVertical.HandleEvent(cancellationToken);
			case TextEditorWorkKind.OnWheel:
		    	var onWheel = OnWheelQueue.Dequeue();
				return onWheel.HandleEvent(cancellationToken);
			case TextEditorWorkKind.OnWheelBatch:
		    	var onWheelBatch = OnWheelBatchQueue.Dequeue();
				return onWheelBatch.HandleEvent(cancellationToken);
				break;
			default:
				return ValueTask.CompletedTask;
		}
	}
}
