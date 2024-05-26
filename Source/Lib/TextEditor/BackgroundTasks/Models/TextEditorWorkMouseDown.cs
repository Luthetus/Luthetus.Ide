using System.Text;
using Microsoft.AspNetCore.Components.Web;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Displays;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.Options.Models;
using Luthetus.TextEditor.RazorLib.Commands.Models;

namespace Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;

public class TextEditorWorkMouseDown : ITextEditorWork
{
	public TextEditorWorkMouseDown(
		ResourceUri resourceUri,
		Key<TextEditorCursor> cursorKey,
		Func<IEditContext, Key<TextEditorCursor>, TextEditorCursor> getCursorFunc,
		MouseEventArgs mouseEventArgs,
		TextEditorEvents events,
		Key<TextEditorViewModel> viewModelKey)
	{
		ResourceUri = resourceUri;
		CursorKey = cursorKey;
		GetCursorFunc = getCursorFunc;
		MouseEventArgs = mouseEventArgs;
		Events = events;
		ViewModelKey = viewModelKey;
	}

	public TextEditorWorkKind TextEditorWorkKind => TextEditorWorkKind.Complex;

	public string Name => "MouseDown";

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
	
	public MouseEventArgs MouseEventArgs { get; }

	public CommandNoType? Command { get; private set; }

	public TextEditorEvents Events { get; }

	public ITextEditorWork? BatchEnqueue(
		ITextEditorWork upstreamWork)
	{
		if (upstreamWork.CursorKey == CursorKey &&
			upstreamWork.ViewModelKey == ViewModelKey &&
			upstreamWork is TextEditorWorkMouseDown)
		{
			// Replace the upstreamWork with the more recent event
			// because the events are redundant when consecutive.
			return this;
		}

		return null;
	}

	public ITextEditorWork? BatchDequeue(
		IEditContext editContext,
		ITextEditorWork upstreamWork)
	{
		if (upstreamWork.CursorKey == CursorKey &&
			upstreamWork.ViewModelKey == ViewModelKey &&
			upstreamWork is TextEditorWorkMouseDown)
		{
			// Replace the upstreamWork with the more recent event
			// because the events are redundant when consecutive.
			return this;
		}

		return null;
	}

	public async Task Invoke(IEditContext editContext)
	{
		Console.WriteLine($"{nameof(TextEditorWorkMouseDown)} Invoke");
		var modelModifier = editContext.GetModelModifier(ResourceUri, true);
        var viewModelModifier = editContext.GetViewModelModifier(ViewModelKey);

        if (modelModifier is null || viewModelModifier is null)
		{
			Console.WriteLine($"{nameof(TextEditorWorkMouseDown)} Invoke returned because modelModifier is null || viewModelModifier is null");
			return;
		}
		
		Console.WriteLine($"{nameof(TextEditorWorkMouseDown)} pre GetCursorModifierAndBagTuple");
		var (primaryCursorModifier, cursorModifierBag) = ITextEditorWork.GetCursorModifierAndBagTuple(
			editContext,
			ViewModelKey,
			CursorKey,
			GetCursorFunc);
		Console.WriteLine($"{nameof(TextEditorWorkMouseDown)} post GetCursorModifierAndBagTuple");

		viewModelModifier.ViewModel.UnsafeState.ShouldRevealCursor = false;

        var hasSelectedText = TextEditorSelectionHelper.HasSelectedText(primaryCursorModifier);

        if ((MouseEventArgs.Buttons & 1) != 1 && hasSelectedText)
		{
			Console.WriteLine($"{nameof(TextEditorWorkMouseDown)} Invoke returned because (MouseEventArgs.Buttons & 1) != 1 && hasSelectedText");
			return; // Not pressing the left mouse button so assume ContextMenu is desired result.
		}

		// Hacky(Mild, Medium, [Hot]) (2024-05-25)
		//
		// await Events.CursorSetShouldDisplayMenuAsyncFunc.Invoke(MenuKind.None, false);

        // Remember the current cursor position prior to doing anything
		Console.WriteLine($"current ({primaryCursorModifier.LineIndex}, {primaryCursorModifier.ColumnIndex})");
		Console.WriteLine($"current {primaryCursorModifier.Key}");
        var inRowIndex = primaryCursorModifier.LineIndex;
        var inColumnIndex = primaryCursorModifier.ColumnIndex;

        
		// Hacky(Mild, [Medium], Hot) (2024-05-25)
		//
		// Move the cursor position
        var rowAndColumnIndex = await Events.CalculateRowAndColumnIndex(MouseEventArgs);

		Console.WriteLine("rowIndex: " + rowAndColumnIndex.rowIndex);
		Console.WriteLine("columnIndex: " + rowAndColumnIndex.columnIndex);

        primaryCursorModifier.LineIndex = rowAndColumnIndex.rowIndex;
        primaryCursorModifier.ColumnIndex = rowAndColumnIndex.columnIndex;
        primaryCursorModifier.PreferredColumnIndex = rowAndColumnIndex.columnIndex;

		// Hacky(Mild, Medium, [Hot]) (2024-05-25)
		//
        // Events.CursorPauseBlinkAnimationAction.Invoke();

        var cursorPositionIndex = modelModifier.GetPositionIndex(new TextEditorCursor(
            rowAndColumnIndex.rowIndex,
            rowAndColumnIndex.columnIndex,
            true));

        if (MouseEventArgs.ShiftKey)
        {
            if (!hasSelectedText)
            {
                // If user does not yet have a selection then place the text selection anchor were they were
                primaryCursorModifier.SelectionAnchorPositionIndex = modelModifier
                    .GetPositionIndex(inRowIndex, inColumnIndex);
            }

            // If user ALREADY has a selection then do not modify the text selection anchor
        }
        else
        {
            primaryCursorModifier.SelectionAnchorPositionIndex = cursorPositionIndex;
        }

        primaryCursorModifier.SelectionEndingPositionIndex = cursorPositionIndex;

		Console.WriteLine($"{nameof(TextEditorWorkMouseDown)} invoke end");
	}
}
