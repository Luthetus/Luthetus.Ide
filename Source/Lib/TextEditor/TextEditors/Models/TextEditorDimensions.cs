namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

/// <summary>
/// The unit of measurement is Pixels (px)
/// JavaScript/Html controls the text editor dimensions
/// </summary>
/// <param name="Width">The unit of measurement is Pixels (px)</param>
/// <param name="Height">The unit of measurement is Pixels (px)</param>
/// <param name="BoundingClientRectLeft">The unit of measurement is Pixels (px)</param>
/// <param name="BoundingClientRectTop">The unit of measurement is Pixels (px)</param>
public struct TextEditorDimensions
{
	public TextEditorDimensions(
		double width,
	    double height,
		double boundingClientRectLeft,
		double boundingClientRectTop)
	{
		Width = width;
	    Height = height;
		BoundingClientRectLeft = boundingClientRectLeft;
		BoundingClientRectTop = boundingClientRectTop;
	}

	public double Width;
    public double Height;
	public double BoundingClientRectLeft;
	public double BoundingClientRectTop;
}

public struct TextEditorDimensionsDto
{
	public TextEditorDimensionsDto(
		double width,
	    double height,
		double boundingClientRectLeft,
		double boundingClientRectTop)
	{
		Width = width;
	    Height = height;
		BoundingClientRectLeft = boundingClientRectLeft;
		BoundingClientRectTop = boundingClientRectTop;
	}

	public double Width { get; set; }
    public double Height { get; set; }
	public double BoundingClientRectLeft { get; set; }
	public double BoundingClientRectTop { get; set; }
}
