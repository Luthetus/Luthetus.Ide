namespace Luthetus.TextEditor.RazorLib.Virtualizations.Models;

/// <summary>
/// Use -1 to indicate '100%' as the CSS unit of measurement.
/// </summary>
public record struct VirtualizationBoundary(
    double WidthInPixels,
    double HeightInPixels,
    double LeftInPixels,
    double TopInPixels);