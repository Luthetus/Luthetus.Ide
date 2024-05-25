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

public class TextEditorWorkMouseMove : ITextEditorWork
{
	public TextEditorWorkMouseMove(
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
		ITextEditorWork precedentWork)
	{
		if (precedentWork.CursorKey == CursorKey &&
			precedentWork.ViewModelKey == ViewModelKey &&
			precedentWork is TextEditorWorkMouseMove)
		{
			// Replace the precedentWork with the more recent event
			// because the events are redundant when consecutive.
			return this;
		}

		return null;
	}

	public ITextEditorWork? BatchDequeue(
		IEditContext editContext,
		ITextEditorWork precedentWork)
	{
		if (precedentWork.CursorKey == CursorKey &&
			precedentWork.ViewModelKey == ViewModelKey &&
			precedentWork is TextEditorWorkMouseMove)
		{
			// Replace the precedentWork with the more recent event
			// because the events are redundant when consecutive.
			return this;
		}

		return null;
	}

	public async Task Invoke(IEditContext editContext)
	{
		var modelModifier = editContext.GetModelModifier(ResourceUri, true);
        var viewModelModifier = editContext.GetViewModelModifier(ViewModelKey);

        if (modelModifier is null || viewModelModifier is null)
            return;
		
		var (primaryCursorModifier, cursorModifierBag) = ITextEditorWork.GetCursorModifierAndBagTuple(
			editContext,
			ViewModelKey,
			CursorKey,
			GetCursorFunc);

		// Hacky(Mild, Medium, [Hot]) (2024-05-25)
		//
		// # In other places this code is medium but for on mouse move it feels
		// # more hacky considering the rate at which he event is firing.
		var rowAndColumnIndex = await Events.CalculateRowAndColumnIndex(MouseEventArgs).ConfigureAwait(false);

        primaryCursorModifier.LineIndex = rowAndColumnIndex.rowIndex;
        primaryCursorModifier.ColumnIndex = rowAndColumnIndex.columnIndex;
        primaryCursorModifier.PreferredColumnIndex = rowAndColumnIndex.columnIndex;

		// Hacky(Mild, Medium, [Hot]) (2024-05-25)
		//
		// # In other places this code is medium but for on mouse move it feels
		// # more hacky considering the rate at which he event is firing.
		//
        // Events.CursorPauseBlinkAnimationAction.Invoke();

        primaryCursorModifier.SelectionEndingPositionIndex = modelModifier.GetPositionIndex(primaryCursorModifier);
	}
}
