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

public class TextEditorWorkWheel : ITextEditorWork
{
	public TextEditorWorkWheel(
		ResourceUri resourceUri,
		Key<TextEditorCursor> cursorKey,
		Func<IEditContext, Key<TextEditorCursor>, TextEditorCursor> getCursorFunc,
		WheelEventArgs wheelEventArgs,
		TextEditorEvents events,
		Key<TextEditorViewModel> viewModelKey)
	{
		ResourceUri = resourceUri;
		CursorKey = cursorKey;
		GetCursorFunc = getCursorFunc;
		WheelEventArgs = wheelEventArgs;
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
	
	public WheelEventArgs WheelEventArgs { get; }

	public CommandNoType? Command { get; private set; }

	public TextEditorEvents Events { get; }

	public ITextEditorWork? BatchOrDefault(
		IEditContext editContext,
		TextEditorWorkWheel oldWorkWheel)
	{
		// If this method changes from acceping a 'TextEditorWorkMouseMove' to an 'ITextEditorWork'
		// Then it is vital that this pattern matching is performed.
		if (oldWorkWheel is TextEditorWorkWheel)
			return this;

		return null;
	}

	public Task Invoke(IEditContext editContext)
	{
		var modelModifier = editContext.GetModelModifier(ResourceUri, true);
        var viewModelModifier = editContext.GetViewModelModifier(ViewModelKey);

        if (modelModifier is null || viewModelModifier is null)
            return Task.CompletedTask;
		
		var (primaryCursorModifier, cursorModifierBag) = ITextEditorWork.GetCursorModifierAndBagTuple(
			editContext,
			ViewModelKey,
			CursorKey,
			GetCursorFunc);

		if (WheelEventArgs.ShiftKey)
        {
            return editContext.TextEditorService.ViewModelApi
                .MutateScrollHorizontalPositionFactory(ViewModelKey, WheelEventArgs.DeltaY)
                .Invoke(editContext);
        }
        else
        {
            return editContext.TextEditorService.ViewModelApi
                .MutateScrollVerticalPositionFactory(ViewModelKey, WheelEventArgs.DeltaY)
                .Invoke(editContext);
        }
	}
}
