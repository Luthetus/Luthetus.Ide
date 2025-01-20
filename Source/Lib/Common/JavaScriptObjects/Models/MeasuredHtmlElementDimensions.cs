namespace Luthetus.Common.RazorLib.JavaScriptObjects.Models;

public record struct MeasuredHtmlElementDimensions(
    double WidthInPixels,
    double HeightInPixels,
    double LeftInPixels,
    double TopInPixels,
    double ZIndex);