namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

/// <summary>
/// The unit of measurement is Pixels (px)
/// C# controls the scrollbar dimensions
/// </summary>
/// <param name="ScrollLeft">The unit of measurement is Pixels (px)</param>
/// <param name="ScrollTop">The unit of measurement is Pixels (px)</param>
/// <param name="ScrollWidth">The unit of measurement is Pixels (px)</param>
/// <param name="ScrollHeight">The unit of measurement is Pixels (px)</param>
/// <param name="MarginScrollHeight">The unit of measurement is Pixels (px)</param>
public record struct ScrollbarDimensions(
	double ScrollLeft,
    double ScrollTop,
    double ScrollWidth,
    double ScrollHeight,
    double MarginScrollHeight);
