using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.TextEditor.RazorLib.Virtualizations.Models;
using Microsoft.AspNetCore.Components;
using System.Text;

namespace Luthetus.TextEditor.RazorLib.Virtualizations.Displays;

public partial class VirtualizationBoundaryDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public VirtualizationBoundary VirtualizationBoundary { get; set; } = null!;
    [Parameter, EditorRequired]
    public string VirtualizationBoundaryDisplayId { get; set; } = null!;

    private string GetStyleCssString()
    {
        var styleBuilder = new StringBuilder();

        // Width
        if (VirtualizationBoundary.WidthInPixels is null)
        {
            styleBuilder.Append(" width: 100%;");
        }
        else
        {
            var widthInPixelsInvariantCulture = VirtualizationBoundary.WidthInPixels.Value.ToCssValue();
            styleBuilder.Append($" width: {widthInPixelsInvariantCulture}px;");
        }

        // Height
        if (VirtualizationBoundary.HeightInPixels is null)
        {
            styleBuilder.Append(" height: 100%;");
        }
        else
        {
            var heightInPixelsInvariantCulture = VirtualizationBoundary.HeightInPixels.Value.ToCssValue();
            styleBuilder.Append($" height: {heightInPixelsInvariantCulture}px;");
        }

        // Left
        if (VirtualizationBoundary.LeftInPixels is null)
        {
            styleBuilder.Append(" left: 100%;");
        }
        else
        {
            var leftInPixelsInvariantCulture = VirtualizationBoundary.LeftInPixels.Value.ToCssValue();
            styleBuilder.Append($" left: {leftInPixelsInvariantCulture}px;");
        }

        // Top
        if (VirtualizationBoundary.TopInPixels is null)
        {
            styleBuilder.Append(" top: 100%;");
        }
        else
        {
            var topInPixelsInvariantCulture = VirtualizationBoundary.TopInPixels.Value.ToCssValue();
            styleBuilder.Append($" top: {topInPixelsInvariantCulture}px;");
        }

        return styleBuilder.ToString();
    }
}