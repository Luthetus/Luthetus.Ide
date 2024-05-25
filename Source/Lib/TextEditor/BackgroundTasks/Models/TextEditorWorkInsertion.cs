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
		Func<IEditContext, Key<TextEditorCursor>, TextEditorCursor> getCursorFunc,
		string content,
		Key<TextEditorViewModel> viewModelKey)
	{
		ResourceUri = resourceUri;
		CursorKey = cursorKey;
		GetCursorFunc = getCursorFunc;
		_content = content;
		ViewModelKey = viewModelKey;
	}
	
	/// <summary>
	/// The string that was provided to the constructor.
	/// If no batching occurs, then this field is the Content, otherwise a StringBuilder
	/// will be lazily constructed, and then used as the Content from then on.
	/// </summary>
	private string _content;
	private StringBuilder? _contentBuilder;

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
	/// The content to insert.
	/// </summary>
	public string Content => _contentBuilder?.ToString() ?? _content;

	public StringBuilder ContentBuilder => _contentBuilder ??= new(Content);

	public ITextEditorWork? BatchEnqueue(
		ITextEditorWork precedentWork)
	{
		if (precedentWork.TextEditorWorkKind == TextEditorWorkKind.Insertion &&
			precedentWork.CursorKey == CursorKey &&
			precedentWork.ViewModelKey == ViewModelKey)
		{
			((TextEditorWorkInsertion)precedentWork).ContentBuilder.Append(Content);
			return precedentWork;
		}
		
		return null;
	}

	public ITextEditorWork? BatchDequeue(
		IEditContext editContext,
		ITextEditorWork precedentWork)
	{
		if (precedentWork.TextEditorWorkKind == TextEditorWorkKind.Insertion &&
			precedentWork.CursorKey == CursorKey &&
			precedentWork.ViewModelKey == ViewModelKey)
		{
			((TextEditorWorkInsertion)precedentWork).ContentBuilder.Append(Content);
			return precedentWork;
		}
		
		return null;
	}

	public Task Invoke(IEditContext editContext)
	{
		var modelModifier = editContext.GetModelModifier(ResourceUri);

		var (cursorModifier, cursorModifierBag) = ITextEditorWork.GetCursorModifierAndBagTuple(
			editContext,
			ViewModelKey,
			CursorKey,
			GetCursorFunc);

		modelModifier.Insert(
	        Content.ToString(),
			cursorModifierBag,
	        useLineEndKindPreference: false,
	        cancellationToken: default);

		return Task.CompletedTask;
	}
}
