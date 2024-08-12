namespace Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals;

public class VirtualizationDriver
{
	public readonly TextEditorViewModelDisplay _root;

	public VirtualizationDriver(TextEditorViewModelDisplay textEditorViewModelDisplay)
	{
		_root = textEditorViewModelDisplay;
	}

	// Odd public but am middle of thinking
	public TextEditorRenderBatchValidated _renderBatch;

	public RenderFragment GetRenderFragment(TextEditorRenderBatchValidated renderBatch)
	{
		// Dangerous state can change mid run possible?
		_renderBatch = renderBatch;
		return VirtualizationStaticRenderFragments.GetRenderFragment(this);
	}
}
