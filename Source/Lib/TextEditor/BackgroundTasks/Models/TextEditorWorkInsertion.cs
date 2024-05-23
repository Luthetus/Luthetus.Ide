using System.Text;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;

public class TextEditorWorkInsertion : ITextEditorWork
{
	public TextEditorWorkInsertion(
		ResourceUri resourceUri,
		Key<TextEditorCursor> cursorKey,
		Func<Key<TextEditorCursor>, TextEditorCursor> getCursorFunc,
		StringBuilder content)
	{
		ResourceUri = resourceUri;
		CursorKey = cursorKey;
		GetCursorFunc = getCursorFunc;
		Content = content;
	}

	public TextEditorWorkKind TextEditorWorkKind => TextEditorWorkKind.Insertion;

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
	/// Track where the content should be inserted.
	/// </summary>
	public Key<TextEditorCursor> CursorKey { get; }

	/// <summary>
	/// If the cursor is not already registered within the ITextEditorEditContext,
	/// then invoke this Func, and then register a CursorModifier in the
	/// ITextEditorEditContext.
	/// </summary>
	public Func<Key<TextEditorCursor>, TextEditorCursor> GetCursorFunc { get; }
	
	/// <summary>
	/// The content to insert.
	///
	/// If a struct contains a reference type does that invalidate the
	/// optimization I'm trying to achieve with using a struct?
	///
	/// Does this even compile?
	/// </summary>
	public StringBuilder Content { get; }

	public Task Invoke(IEditContext editContext)
	{
		if (editContext is null)
			Console.WriteLine("editContext was null");
		else
			Console.WriteLine("editContext was NOT null");

		if (ResourceUri is null)
			Console.WriteLine("ResourceUri was null");
		else
			Console.WriteLine("ResourceUri was NOT null");

		var modelModifier = editContext.GetModelModifier(ResourceUri);

		var cursorModifier = editContext.GetCursorModifier(
			CursorKey,
			GetCursorFunc);

		var cursorModifierBag = new CursorModifierBagTextEditor(
			Key<TextEditorViewModel>.Empty,
			new List<TextEditorCursorModifier> { cursorModifier });

		modelModifier.Insert(
	        Content.ToString(),
			cursorModifierBag,
	        useLineEndKindPreference: false,
	        cancellationToken: default);

		return Task.CompletedTask;
	}
}
