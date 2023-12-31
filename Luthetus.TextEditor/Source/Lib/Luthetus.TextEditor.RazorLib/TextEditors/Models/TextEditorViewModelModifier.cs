namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public class TextEditorViewModelModifier
{
    public TextEditorViewModelModifier(TextEditorViewModel viewModel)
    {
        ViewModel = viewModel;
    }

    /// <summary>
    /// TODO: Make this private
    /// </summary>
    public TextEditorViewModel ViewModel { get; set; }
    public bool WasModified { get; internal set; }
}