namespace Luthetus.TextEditor.RazorLib.Virtualizations.Models;

/// <summary>
/// Use -1 to indicate '100%' as the CSS unit of measurement.
///
/// (I'm not even sure that anyone was using the null value?
//  (back when this was nullable))
/// </summary>
public record struct VirtualizationBoundary(
    double WidthInPixels,
    double HeightInPixels);