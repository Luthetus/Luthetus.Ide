namespace Luthetus.TextEditor.Tests.Adhoc.Rewrite;

public class TextEditorWorkDeletion
{
	public TextEditorWorkKind TextEditorWorkKind => TextEditorWorkKind.Deletion;

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
	/// Track where the deletion should start.
	/// </summary>
	Key<TextEditorCursor> CursorKey;

	/// <summary>
	/// How many user-characters should be deleted from the starting position.
	/// For example: "\r\n" is 1 user-character, yet 2 chars.
	/// </summary>
	int Count;

	public Task Invoke(ITextEditorEditContext editContext)
	{
		return Task.CompletedTask;
	}
}
