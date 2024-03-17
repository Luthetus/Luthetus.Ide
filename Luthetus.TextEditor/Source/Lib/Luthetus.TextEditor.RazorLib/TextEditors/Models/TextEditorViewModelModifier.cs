namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public class TextEditorViewModelModifier
{
    public TextEditorViewModelModifier(TextEditorViewModel viewModel)
    {
        ViewModel = viewModel;
    }

    public TextEditorViewModel ViewModel { get; set; }
    public bool WasModified { get; internal set; }
}