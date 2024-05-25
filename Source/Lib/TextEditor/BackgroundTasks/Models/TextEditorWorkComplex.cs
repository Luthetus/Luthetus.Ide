using System.Text;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;

public class TextEditorWorkComplex : ITextEditorWork
{
	public TextEditorWorkComplex(
		ResourceUri resourceUri,
		Key<TextEditorCursor> cursorKey,
		Func<IEditContext, Key<TextEditorCursor>, TextEditorCursor> getCursorFunc,
		TextEditorEdit textEditorEdit)
	{
		ResourceUri = resourceUri;
		CursorKey = cursorKey;
		GetCursorFunc = getCursorFunc;
		TextEditorEdit = textEditorEdit;
	}

	public TextEditorWorkComplex(
		ResourceUri resourceUri,
		Key<TextEditorViewModel> viewModelKey,
		TextEditorEdit textEditorEdit)
	{
		ResourceUri = resourceUri;
		ViewModelKey = viewModelKey;
		TextEditorEdit = textEditorEdit;
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
	
	public TextEditorEdit TextEditorEdit { get; }

	public ITextEditorWork? BatchEnqueue(
		ITextEditorWork precedentWork)
	{
		return null;
	}

	public ITextEditorWork? BatchDequeue(
		IEditContext editContext,
		ITextEditorWork precedentWork)
	{
		return null;
	}

	public Task Invoke(IEditContext editContext)
	{
		return TextEditorEdit.Invoke(editContext);
	}
}
