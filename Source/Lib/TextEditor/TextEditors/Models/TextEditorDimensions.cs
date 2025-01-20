namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

/// <summary>
/// The unit of measurement is Pixels (px)
/// JavaScript/Html controls the text editor dimensions
/// </summary>
/// <param name="Width">The unit of measurement is Pixels (px)</param>
/// <param name="Height">The unit of measurement is Pixels (px)</param>
/// <param name="BoundingClientRectLeft">The unit of measurement is Pixels (px)</param>
/// <param name="BoundingClientRectTop">The unit of measurement is Pixels (px)</param>
public sealed record TextEditorDimensions(
    double Width,
    double Height,
	double BoundingClientRectLeft,
	double BoundingClientRectTop);
