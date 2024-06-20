namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public class TextEditorViewModelModifier
{
    public TextEditorViewModelModifier(TextEditorViewModel viewModel)
    {
        ViewModel = viewModel;
    }

    public TextEditorViewModel ViewModel { get; set; }
    public bool WasModified { get; internal set; }
    public bool ScrollWasModified { get; internal set; }

	/// <summary>
	/// This property decides whether or not to re-calculate the virtualization result that gets displayed on the UI.
	/// </summary>
    public bool ShouldReloadVirtualizationResult { get; set; }
}