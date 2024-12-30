namespace Luthetus.TextEditor.RazorLib.Virtualizations.Models;

public record struct VirtualizationBoundary(
    double? WidthInPixels,
    double? HeightInPixels,
    double? LeftInPixels,
    double? TopInPixels);