namespace Luthetus.TextEditor.RazorLib.Virtualizations.Models;

public record VirtualizationEntry(
    int LineIndex,
    int HorizontalPositionIndexInclusiveStart,
    int HorizontalPositionIndexExclusiveEnd,
    double? WidthInPixels,
    double? HeightInPixels,
    double? LeftInPixels,
    double? TopInPixels);