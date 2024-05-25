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

	public ITextEditorWork? BatchEnqueue(
		ITextEditorWork precedentWork);

	public ITextEditorWork? BatchDequeue(
		IEditContext editContext,
		ITextEditorWork precedentWork);

	/// <summary>
	/// There is a mismatch in how I interact with the text editor from:
	///     -A unit test
	///     -The UI
	///
	/// When writing a unit test, I prefer avoiding the TextEditorViewModel altogether.
	///
	/// Whereas, with the UI, I want my cursor to be synced with the TextEditorViewModel.
	///
	/// As of this comment, I've put arguments in the constructors of various <see cref="ITextEditorTask"/>
	/// implementations, to support either interaction. With the expectation that one somehow
	/// knows that you choose some parameters, or another group of parameters, but to not fill out both.
	///
	/// This results in very confusing API.
	///
	/// TODO: Decide how to handle the mismatch between wanting to use the "TextEditorViewModel's cursors"...
	///       ...versus using a personal cursor, (perhaps constructed as a one off use).
	/// </summary>
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
