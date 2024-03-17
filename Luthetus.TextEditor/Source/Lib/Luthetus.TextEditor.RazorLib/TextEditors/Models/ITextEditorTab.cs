namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public interface ITextEditorTab : ITab
{
	public Key<TextEditorViewModel> ViewModelKey { get; }
}
