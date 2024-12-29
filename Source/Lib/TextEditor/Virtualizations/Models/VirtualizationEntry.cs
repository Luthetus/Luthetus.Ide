namespace Luthetus.TextEditor.RazorLib.Virtualizations.Models;

/// <summary>
/// This type is intended to represent a line within a flat list.
/// The 'LineIndex' is just a marker for the offset within the flat list, not actually multi-dimensional list.
/// </summary>
public record VirtualizationEntry(
    int LineIndex,
    int PositionIndexInclusiveStart,
    int PositionIndexExclusiveEnd,
    double? WidthInPixels,
    double? HeightInPixels,
    double? LeftInPixels,
    double? TopInPixels);