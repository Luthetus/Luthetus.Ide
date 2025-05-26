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
public class TextEditorWorkerUi : IBackgroundTaskGroup
{
	private readonly TextEditorService _textEditorService;
	
	public TextEditorWorkerUi(TextEditorService textEditorService)
	{
		_textEditorService = textEditorService;
	}
	
	public Key<IBackgroundTaskGroup> BackgroundTaskKey { get; } = Key<IBackgroundTaskGroup>.NewKey();
    
    // Nervous about this not being considered an interpolated constant string.
    
    public bool __TaskCompletionSourceWasCreated { get; set; }
    
    public Queue<OnDoubleClick> OnDoubleClickQueue { get; } = new();
    public Queue<OnKeyDown> OnKeyDownQueue { get; } = new();
	public Queue<OnMouseDown> OnMouseDownQueue { get; } = new();
    public Queue<OnMouseMove> OnMouseMoveQueue { get; } = new();
    public Queue<OnScrollHorizontal> OnScrollHorizontalQueue { get; } = new();
	public Queue<OnScrollVertical> OnScrollVerticalQueue { get; } = new();
	public Queue<OnWheel> OnWheelQueue { get; } = new();
	
	/// <summary>
	/// If multiple EventKind of the same are enqueued one after another then
	/// better to have this Queue be a struct that has the count of contiguous work kind enqueues?
	/// </summary>
	public Queue<TextEditorWorkUiKind> WorkKindQueue { get; } = new();
	
	/// <summary>
	/// For thread safety you must ensure you invoke this "only" from the UI thread because there
	/// is no thread safety in the implementation.
	/// </summary>
	public void UnsafeEnqueueOnDoubleClick(OnDoubleClick onDoubleClick)
	{
		WorkKindQueue.Enqueue(TextEditorWorkUiKind.OnDoubleClick);
		OnDoubleClickQueue.Enqueue(onDoubleClick);
		_textEditorService.BackgroundTaskService.Continuous_EnqueueGroup(this);
	}
	
	/// <summary>
	/// For thread safety you must ensure you invoke this "only" from the UI thread because there
	/// is no thread safety in the implementation.
	/// </summary>
	public void UnsafeEnqueueOnKeyDown(OnKeyDown onKeyDown)
	{
		WorkKindQueue.Enqueue(TextEditorWorkUiKind.OnKeyDown);
		OnKeyDownQueue.Enqueue(onKeyDown);
		_textEditorService.BackgroundTaskService.Continuous_EnqueueGroup(this);
	}
	
	/// <summary>
	/// For thread safety you must ensure you invoke this "only" from the UI thread because there
	/// is no thread safety in the implementation.
	/// </summary>
	public void UnsafeEnqueueOnMouseDown(OnMouseDown onMouseDown)
	{
		WorkKindQueue.Enqueue(TextEditorWorkUiKind.OnMouseDown);
		OnMouseDownQueue.Enqueue(onMouseDown);
		_textEditorService.BackgroundTaskService.Continuous_EnqueueGroup(this);
	}
	
	/// <summary>
	/// For thread safety you must ensure you invoke this "only" from the UI thread because there
	/// is no thread safety in the implementation.
	/// </summary>
	public void UnsafeEnqueueOnMouseMove(OnMouseMove onMouseMove)
	{
		WorkKindQueue.Enqueue(TextEditorWorkUiKind.OnMouseMove);
		OnMouseMoveQueue.Enqueue(onMouseMove);
		_textEditorService.BackgroundTaskService.Continuous_EnqueueGroup(this);
	}
	
	/// <summary>
	/// For thread safety you must ensure you invoke this "only" from the UI thread because there
	/// is no thread safety in the implementation.
	/// </summary>
	public void UnsafeEnqueueOnScrollHorizontal(OnScrollHorizontal onScrollHorizontal)
	{
		WorkKindQueue.Enqueue(TextEditorWorkUiKind.OnScrollHorizontal);
		OnScrollHorizontalQueue.Enqueue(onScrollHorizontal);
		_textEditorService.BackgroundTaskService.Continuous_EnqueueGroup(this);
	}
	
	/// <summary>
	/// For thread safety you must ensure you invoke this "only" from the UI thread because there
	/// is no thread safety in the implementation.
	/// </summary>
	public void UnsafeEnqueueOnScrollVertical(OnScrollVertical onScrollVertical)
	{
		WorkKindQueue.Enqueue(TextEditorWorkUiKind.OnScrollVertical);
		OnScrollVerticalQueue.Enqueue(onScrollVertical);
		_textEditorService.BackgroundTaskService.Continuous_EnqueueGroup(this);
	}
	
	/// <summary>
	/// For thread safety you must ensure you invoke this "only" from the UI thread because there
	/// is no thread safety in the implementation.
	/// </summary>
	public void UnsafeEnqueueOnWheel(OnWheel onWheel)
	{
		WorkKindQueue.Enqueue(TextEditorWorkUiKind.OnWheel);
		OnWheelQueue.Enqueue(onWheel);
		_textEditorService.BackgroundTaskService.Continuous_EnqueueGroup(this);
	}
	
	public ValueTask HandleEvent(CancellationToken cancellationToken)
	{
		TextEditorWorkUiKind workKind;
	
		// avoid UI infinite loop enqueue dequeue single work item
		// by getting the count prior to starting the yield return deqeue
		// then only dequeueing at most 'count' times.
		
		if (!WorkKindQueue.TryDequeue(out workKind))
			return ValueTask.CompletedTask;
		
		switch (workKind)
		{
			case TextEditorWorkUiKind.OnDoubleClick:
				return OnDoubleClickQueue.Dequeue().HandleEvent(cancellationToken);
		    case TextEditorWorkUiKind.OnKeyDown:
				return OnKeyDownQueue.Dequeue().HandleEvent(cancellationToken);
			case TextEditorWorkUiKind.OnMouseDown:
				return OnMouseDownQueue.Dequeue().HandleEvent(cancellationToken);
		    case TextEditorWorkUiKind.OnMouseMove:
				return OnMouseMoveQueue.Dequeue().HandleEvent(cancellationToken);
		    case TextEditorWorkUiKind.OnScrollHorizontal:
				return OnScrollHorizontalQueue.Dequeue().HandleEvent(cancellationToken);
			case TextEditorWorkUiKind.OnScrollVertical:
				return OnScrollVerticalQueue.Dequeue().HandleEvent(cancellationToken);
			case TextEditorWorkUiKind.OnWheel:
				return OnWheelQueue.Dequeue().HandleEvent(cancellationToken);
			default:
				Console.WriteLine($"{nameof(TextEditorWorkerUi)} {nameof(HandleEvent)} default case");
				return ValueTask.CompletedTask;
		}
	}
}
