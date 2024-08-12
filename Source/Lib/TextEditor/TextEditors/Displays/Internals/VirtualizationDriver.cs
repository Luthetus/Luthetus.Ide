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

	public VirtualizationDriver(
		TextEditorViewModelDisplay textEditorViewModelDisplay,
		bool useHorizontalVirtualization,
		bool useVerticalVirtualization)
	{
		_root = textEditorViewModelDisplay;
		UseHorizontalVirtualization = useHorizontalVirtualization;
		UseVerticalVirtualization = useVerticalVirtualization;
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

    public string GetStyleCssString(VirtualizationBoundary virtualizationBoundary)
    {
        var styleBuilder = new StringBuilder();

        // Width
        if (virtualizationBoundary.WidthInPixels is null)
        {
            styleBuilder.Append(" width: 100%;");
        }
        else
        {
            var widthInPixelsInvariantCulture = virtualizationBoundary.WidthInPixels.Value.ToCssValue();
            styleBuilder.Append($" width: {widthInPixelsInvariantCulture}px;");
        }

        // Height
        if (virtualizationBoundary.HeightInPixels is null)
        {
            styleBuilder.Append(" height: 100%;");
        }
        else
        {
            var heightInPixelsInvariantCulture = virtualizationBoundary.HeightInPixels.Value.ToCssValue();
            styleBuilder.Append($" height: {heightInPixelsInvariantCulture}px;");
        }

        // Left
        if (virtualizationBoundary.LeftInPixels is null)
        {
            styleBuilder.Append(" left: 100%;");
        }
        else
        {
            var leftInPixelsInvariantCulture = virtualizationBoundary.LeftInPixels.Value.ToCssValue();
            styleBuilder.Append($" left: {leftInPixelsInvariantCulture}px;");
        }

        // Top
        if (virtualizationBoundary.TopInPixels is null)
        {
            styleBuilder.Append(" top: 100%;");
        }
        else
        {
            var topInPixelsInvariantCulture = virtualizationBoundary.TopInPixels.Value.ToCssValue();
            styleBuilder.Append($" top: {topInPixelsInvariantCulture}px;");
        }

        return styleBuilder.ToString();
    }
}
