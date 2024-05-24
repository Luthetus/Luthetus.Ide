using System.Text;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;

public interface ITextEditorWork
{
	public TextEditorWorkKind TextEditorWorkKind { get; }

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

	public Task Invoke(IEditContext editContext);

	public static (TextEditorCursorModifier cursorModifier, CursorModifierBagTextEditor cursorModifierBag) GetCursorModifierAndBagTuple(
		IEditContext editContext,
		Key<TextEditorViewModel> viewModelKey,
		Key<TextEditorCursor> cursorKey,
		Func<IEditContext, Key<TextEditorCursor>, TextEditorCursor> getCursorFunc)
	{
		TextEditorCursorModifier cursorModifier;
		CursorModifierBagTextEditor cursorModifierBag;

		if (getCursorFunc is not null && cursorKey != Key<TextEditorCursor>.Empty)
		{
			cursorModifier = editContext.GetCursorModifier(
				cursorKey,
				getCursorFunc);

			cursorModifierBag = new CursorModifierBagTextEditor(
				Key<TextEditorViewModel>.Empty,
				new List<TextEditorCursorModifier> { cursorModifier });
		}
		else
		{
			var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);
			cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
			var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);

			if (primaryCursorModifier is null)
				Console.WriteLine("primaryCursorModifier was null");

			cursorModifier = primaryCursorModifier;
		}

		return (cursorModifier, cursorModifierBag);
	}
}
