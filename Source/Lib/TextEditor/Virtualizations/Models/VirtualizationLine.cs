namespace Luthetus.TextEditor.RazorLib.Virtualizations.Models;

/// <summary>
/// This type is intended to represent a line within a flat list.
/// The 'LineIndex' is just a marker for the offset within the flat list, not actually multi-dimensional list.
/// </summary>
public record struct VirtualizationLine(
    int LineIndex,
    int PositionIndexInclusiveStart,
    int PositionIndexExclusiveEnd,
    List<VirtualizationSpan>? VirtualizationSpanList,
    double? WidthInPixels,
    double? HeightInPixels,
    double? LeftInPixels,
    double? TopInPixels);