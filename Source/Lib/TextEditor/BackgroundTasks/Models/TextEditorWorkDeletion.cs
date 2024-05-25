using System.Text;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;

public class TextEditorWorkDeletion : ITextEditorWork
{
	public TextEditorWorkDeletion(
		ResourceUri resourceUri,
		Key<TextEditorCursor> cursorKey,
		Func<IEditContext, Key<TextEditorCursor>, TextEditorCursor> getCursorFunc,
		int columnCount,
		TextEditorModelModifier.DeleteKind deleteKind)
	{
		ResourceUri = resourceUri;
		CursorKey = cursorKey;
		GetCursorFunc = getCursorFunc;
		ColumnCount = columnCount;
		DeleteKind = deleteKind;
	}

	public TextEditorWorkKind TextEditorWorkKind => TextEditorWorkKind.Deletion;

	public string Name => "Deletion";

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

	/// <summary>
	/// How many user-characters should be deleted from the starting position.
	/// For example: "\r\n" is 1 user-character, yet 2 chars.
	/// </summary>
	public int ColumnCount { get; set; }

	public TextEditorModelModifier.DeleteKind DeleteKind { get; }

	public ITextEditorWork? BatchEnqueue(
		ITextEditorWork upstreamWork)
	{
		if (upstreamWork.TextEditorWorkKind == TextEditorWorkKind.Deletion &&
			upstreamWork.CursorKey == CursorKey &&
			upstreamWork.ViewModelKey == ViewModelKey)
		{
			((TextEditorWorkDeletion)upstreamWork).ColumnCount += ColumnCount;
			return upstreamWork;
		}
		
		return null;
	}
	
	public ITextEditorWork? BatchDequeue(
		IEditContext editContext,
		ITextEditorWork upstreamWork)
	{
		if (upstreamWork.TextEditorWorkKind == TextEditorWorkKind.Deletion &&
			upstreamWork.CursorKey == CursorKey &&
			upstreamWork.ViewModelKey == ViewModelKey)
		{
			((TextEditorWorkDeletion)upstreamWork).ColumnCount += ColumnCount;
			return upstreamWork;
		}
		
		return null;
	}

	public Task Invoke(IEditContext editContext)
	{
		var modelModifier = editContext.GetModelModifier(ResourceUri);

		var cursorModifier = editContext.GetCursorModifier(
			CursorKey,
			GetCursorFunc);

		var cursorModifierBag = new CursorModifierBagTextEditor(
			Key<TextEditorViewModel>.Empty,
			new List<TextEditorCursorModifier> { cursorModifier });

		modelModifier.Delete(
	        cursorModifierBag,
	        ColumnCount,
	        false,
	        DeleteKind,
	        cancellationToken: default);

		return Task.CompletedTask;
	}
}
