namespace Luthetus.TextEditor.RazorLib.JavaScriptObjects.Models;

/// <summary>
/// The unit of measurement is Pixels (px)
/// </summary>
/// <param name="ScrollLeft">The unit of measurement is Pixels (px)</param>
/// <param name="ScrollTop">The unit of measurement is Pixels (px)</param>
/// <param name="ScrollWidth">The unit of measurement is Pixels (px)</param>
/// <param name="ScrollHeight">The unit of measurement is Pixels (px)</param>
/// <param name="MarginScrollHeight">The unit of measurement is Pixels (px)</param>
/// <param name="Width">The unit of measurement is Pixels (px)</param>
/// <param name="Height">The unit of measurement is Pixels (px)</param>
public record TextEditorMeasurements(
    double ScrollLeft,
    double ScrollTop,
    double ScrollWidth,
    double ScrollHeight,
    double MarginScrollHeight,
    double Width,
    double Height,
    CancellationToken MeasurementsExpiredCancellationToken);