using System.Text;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.Options.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models;

namespace Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;

public class TextEditorWorkKeyDown : ITextEditorWork
{

/*
Goals (2024-05-24)
==================
[]EnqueueBatch
	[]The IBackgroundTaskService is written
	      in terms of IBackgroundTask object instances.
	[]Each .Enqueue(...) requires one to provide an IBackgroundTask.
	[]The issue with this approach is that any batching logic,
	      is being done at the 'IBackgroundTask' level.
	[]I'm thinking about having each 'IBackgroundTask'
          provide a list of 'IBackgroundWork'.
	[]Then, each enqueue could be either at an 'IBackgroundTask' level
	      or an 'IBackgroundWork' level.
	[]For example, I could enqueue 'TextEditorWorkInsertion' twice in a row.
	    []Queue: Empty
		[]Enqueue('TextEditorWorkInsertion')
	    []Queue: TextEditorBackgroundTask('TextEditorWorkInsertion')
		[]Enqueue('TextEditorWorkInsertion')
		[]See the item prior in the queue is 'TextEditorBackgroundTask'
		      []Check the work in the task
			  []The work is of the same type
			  []Merge the work
	    []Queue: TextEditorBackgroundTask('TextEditorWorkInsertion')
		      []Now, there is still only 1 TextEditorBackgroundTask
			  []However, the amount inserted is the combination of both work items.
			  []And the work is done in a single step instead of 2.
	[]An issue here is that, the second Enqueue('TextEditorWorkInsertion')
	      requires a 'TextEditorBackgroundTask' wrapper to be constructed,
		  just for that wrapper to immediately no longer be used once the work is merged.
	      []The queue is a hotspot for optimization.
		  []Imagine a 'onmousemove' event.
		  []Without any throttling or other such logic, hundreds of these events are firing
		        every second as the user moves their mouse.
	      []The garbage collection overhead is presumed to be of concern here.
		  []It is thought that a 'TextEditorBackgroundTask' being a heap allocation,
		        and the 'ITextEditorWork' being a struct, would greatly
				aid in performance.
		  []Because then, one can construct the initial 'TextEditorBackgroundTask' on the heap.
		  []Then, every onmousemove event would only need to construct the struct that
		        contains the data for the event, and merge with the existing 'TextEditorBackgroundTask'.
	[]But, how do I change the 'IBackgroundTaskService', such that I can block the queue from being
	      processed while I view the final entry of the queue, and decide whether to merge
		  my work with that existing background task, or create a new 'IBackgroundTask' to wrap the work?
[]DequeueBatch
	[]The question of whether to "batch on enqueue" or "batch on dequeue"
	      is often coming up.
	[]I've written both implementations and I believe it comes down to:
	      []"batch on enqueue" - reduce the amount of objects constructed
		        []Example: I need to create a 'TextEditorBackgroundTask' everytime I want to
				      enqueue an 'ITextEditorWork'.
                []And yet, it is often the case that there already exists a 'TextEditorBackgroundTask'
				      as the final item on the queue.
                []Then, I move the work into that existing 'TextEditorBackgroundTask'
				      and the 'TextEditorBackgroundTask' I just created was all for nothing.
					  Yet now needs to be garbage collected.
		  []"batch on dequeue" - handle ambiguous work once the state is fully understood
		        []Example: 'TextEditorWorkKeyDown'
					  []Create a queue, but do not begin processing it.
				      []Send a { Key="ArrowRight", ShiftKey=True } keyboardEventArgs
					  []Now send a { Key="Tab" } keyboardEventArgs
			    []If there is text to the right of the cursor, then the
				      { Key="ArrowRight", ShiftKey=True } selects
				      a character.
					  []Now the { Key="Tab" } sees that the 'hasSelection' keymap layer
						    is activated, therefore that line is indented.
				[]If we were to batch on enqueue, we'd see
				      []{ Key="ArrowRight", ShiftKey=True }
					  []and
					  []{ Key="Tab" }
				[]Cannot be batched, because the action to take when hitting "Tab"
				      is dependent on whether the { Key="ArrowRight", ShiftKey=True }
					  selects text or not.
			    []One cannot say, "shift key movement will always select text",
				      if there is no text to the right of the cursor, it will hit end of file,
					  and not select anything.
			    []Furthermore, one cannot say, "well then check if there is text to the right
				      of the cursor" because there are increasingly complex scenarios to take
					 into account.
				[]For example, a 'Vim' keymap. Is it possible to, in a sane manner, predict
				      the result of previous work items that have not yet been handled.
				[]One is essentially calculating twice every keyboard event,
				      once to make a prediction about whether it is batchable,
					  then another time to actually perform the keyboard event.
			    []I believe the only sane solution here is it allow for "batch on dequeue"
				      []This lets one look at the state of the text editor, and clearly
						    determine, "it currently has a text selection", "therefore tab will indent".
				[]Then one can batch as many keyboard events they can until ambiguity comes up.
				[]In which case they can "batch via a foreach loop".
				[]There is the idea of batching a 'TextEditorWorkInsertion' via
				      saying "they both are the same text editor model, and cursor,
					  one is inserting "abc" the other is inserting "123"
                      therefore insert "abc123" in one function invocation.
				[]And then the idea that every 'TextEditorBackgroundTask'
				      will end in a re-render of the UI.
				[]Therefore, worst case batching is to say:
				      []One is inserting "abc", the other is inserting "123",
					  []Well, don't insert "abc" then re-render the UI.
					  []At the least invoke 'Insert(...)' twice prior to the next re-render of the UI.
[]Change queue to a linked list
	[]The current implementation of the queue is a List<IBackgroundTask>.
	[]This results in constant shifting of the entries.
		[]Because index 0 is the next item to dequeue,
		      then once that is dequeue'd the entire list then must shift down 1 index.
	[]Given the "hotspot" nature of the queue, I view this optimization as being important.
*/

	public TextEditorWorkKeyDown(
		ResourceUri resourceUri,
		Key<TextEditorCursor> cursorKey,
		Func<IEditContext, Key<TextEditorCursor>, TextEditorCursor> getCursorFunc,
		KeyboardEventArgs keyboardEventArgs,
		TextEditorEvents events,
		Key<TextEditorViewModel> viewModelKey)
	{
		ResourceUri = resourceUri;
		CursorKey = cursorKey;
		GetCursorFunc = getCursorFunc;
		KeyboardEventArgs = keyboardEventArgs;
		Events = events;
		ViewModelKey = viewModelKey;
	}

	public TextEditorWorkKind TextEditorWorkKind => TextEditorWorkKind.Complex;

	/// <summary>
	/// The resource uri of the model which is to be worked upon.
	/// </summary>
	public ResourceUri ResourceUri { get; }

	/// <summary>
	/// This property is optional, and can be Key<TextEditorViewModel>.Empty,
	/// if one does not make use of it.
	///
	/// The key of the view model which is to be worked upon.
	/// </summary>
	public Key<TextEditorViewModel> ViewModelKey { get; }

	/// <summary>
	/// This property is optional, and can be Key<TextEditorCursor>.Empty,
	/// if one does not make use of it.
	///
	/// Track where the content should be inserted.
	/// </summary>
	public Key<TextEditorCursor> CursorKey { get; }

	/// <summary>
	/// If the cursor is not already registered within the ITextEditorEditContext,
	/// then invoke this Func, and then register a CursorModifier in the
	/// ITextEditorEditContext.
	/// </summary>
	public Func<IEditContext, Key<TextEditorCursor>, TextEditorCursor> GetCursorFunc { get; }
	
	public KeyboardEventArgs KeyboardEventArgs { get; }

	public KeyboardEventArgsKind KeyboardEventArgsKind { get; private set; } = KeyboardEventArgsKind.None;

	public CommandNoType? Command { get; private set; }

	public TextEditorEvents Events { get; }

	public ITextEditorWork? BatchOrDefault(
		IEditContext editContext,
		TextEditorWorkKeyDown oldWorkKeyDown)
	{
		var (cursorModifier, cursorModifierBag) = ITextEditorWork.GetCursorModifierAndBagTuple(
			editContext,
			ViewModelKey,
			CursorKey,
			GetCursorFunc);

        if (cursorModifier is null)
            return null;

		// Step 1:
		// ------
		// Determine whether 'this' is simplifiable to 'TextEditorWorkInsertion'
		{
            var hasSelection = TextEditorSelectionHelper.HasSelectedText(cursorModifier);

            KeyboardEventArgsKind = TextEditorWorkUtils.GetKeyboardEventArgsKind(
                Events.Options,
                KeyboardEventArgs,
                hasSelection,
                editContext.TextEditorService,
                out var localCommand);

            Command = localCommand;
		}

		// Step 2:
		// ------
		// Determine whether 'oldWorkKeyDown' is simplifiable to 'TextEditorWorkInsertion'
		{
			var hasSelection = TextEditorSelectionHelper.HasSelectedText(cursorModifier);

            oldWorkKeyDown.KeyboardEventArgsKind = TextEditorWorkUtils.GetKeyboardEventArgsKind(
                oldWorkKeyDown.Events.Options,
                oldWorkKeyDown.KeyboardEventArgs,
                hasSelection,
                editContext.TextEditorService,
                out var localCommand);

            Command = localCommand;
		}

		// Step 3:
		// ------
		// If they both simplify to 'TextEditorWorkInsertion',
		// then return a single instance of 'TextEditorWorkInsertion'
		// which will insert both keyboard event keys that were pressed.
		if (KeyboardEventArgsKind == oldWorkKeyDown.KeyboardEventArgsKind)
		{
			switch (KeyboardEventArgsKind)
			{
				case KeyboardEventArgsKind.Text:
					return new TextEditorWorkInsertion(
						ResourceUri,
						CursorKey,
						GetCursorFunc,
						new StringBuilder(oldWorkKeyDown.KeyboardEventArgs.Key + KeyboardEventArgs.Key),
						ViewModelKey);
			}
		}
		
		// Return null because they could not be batched.
		// This tells the caller to leave the two items as is.
		return null;
	}

	public async Task Invoke(IEditContext editContext)
	{
		var modelModifier = editContext.GetModelModifier(ResourceUri);
		
		var (cursorModifier, cursorModifierBag) = ITextEditorWork.GetCursorModifierAndBagTuple(
			editContext,
			ViewModelKey,
			CursorKey,
			GetCursorFunc);

		if (KeyboardEventArgsKind == KeyboardEventArgsKind.None)
		{
			var hasSelection = TextEditorSelectionHelper.HasSelectedText(cursorModifier);

			KeyboardEventArgsKind = TextEditorWorkUtils.GetKeyboardEventArgsKind(
                Events.Options,
                KeyboardEventArgs,
                hasSelection,
                editContext.TextEditorService,
                out var localCommand);

			Command = localCommand;
		}

		switch (KeyboardEventArgsKind)
		{
			case KeyboardEventArgsKind.None:
				Console.WriteLine("KeyboardEventArgsKind.None");
				break;
			case KeyboardEventArgsKind.Movement:
				await editContext.TextEditorService.ViewModelApi.MoveCursorFactory(
                            KeyboardEventArgs,
                            modelModifier.ResourceUri,
                            ViewModelKey)
                        .Invoke(editContext)
                    .ConfigureAwait(false);
				break;
			case KeyboardEventArgsKind.ContextMenu:
				Console.WriteLine("KeyboardEventArgsKind.ContextMenu");
				break;
			case KeyboardEventArgsKind.Command:
				await Command.CommandFunc.Invoke(new TextEditorCommandArgs(
                        modelModifier.ResourceUri,
                        ViewModelKey,
                        TextEditorSelectionHelper.HasSelectedText(cursorModifier),
                        Events.ClipboardService,
                        editContext.TextEditorService,
                        Events.Options,
                        Events,
                        Events.HandleMouseStoppedMovingEventAsync,
                        Events.JsRuntime,
                        Events.Dispatcher,
                        Events.ServiceProvider,
                        Events.TextEditorConfig))
                    .ConfigureAwait(false);
				break;
			case KeyboardEventArgsKind.Text:
				modelModifier.Insert(
			        KeyboardEventArgs.Key,
					cursorModifierBag,
			        useLineEndKindPreference: false,
			        cancellationToken: default);
				break;
			case KeyboardEventArgsKind.Other:
				Console.WriteLine("KeyboardEventArgsKind.Other");
				break;
			default:
				Console.WriteLine("KeyboardEventArgsKind switch hit default");
				break;
		}
	}
}
