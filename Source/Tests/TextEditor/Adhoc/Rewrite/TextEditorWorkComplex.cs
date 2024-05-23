namespace Luthetus.TextEditor.Tests.Adhoc.Rewrite;

public class TextEditorWorkComplex
{
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

	public Task Invoke(ITextEditorEditContext editContext)
	{
		return Task.CompletedTask;
	}
}
