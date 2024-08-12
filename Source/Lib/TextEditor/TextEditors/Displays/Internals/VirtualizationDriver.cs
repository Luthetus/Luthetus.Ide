using System.Text;
using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.Characters.Models;
using Luthetus.TextEditor.RazorLib.Cursors.Models;
using Luthetus.TextEditor.RazorLib.Virtualizations.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

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
	
	public bool UseHorizontalVirtualization { get; init; }
	public bool UseVerticalVirtualization { get; init; }
}
