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
public sealed record ScrollbarDimensions(
	double ScrollLeft,
    double ScrollTop,
    double ScrollWidth,
    double ScrollHeight,
    double MarginScrollHeight)
{
	public ScrollbarDimensions WithMutateScrollLeft(int pixels, TextEditorDimensions textEditorDimensions) =>
		WithSetScrollLeft((int)Math.Ceiling(ScrollLeft + pixels), textEditorDimensions);

	public ScrollbarDimensions WithSetScrollLeft(int pixels, TextEditorDimensions textEditorDimensions)
	{
		var resultScrollLeft = Math.Max(0, pixels);
		var maxScrollLeft = (int)Math.Max(0, ScrollWidth - textEditorDimensions.Width);

		if (resultScrollLeft > maxScrollLeft)
			resultScrollLeft = maxScrollLeft;

		return this with { ScrollLeft = resultScrollLeft };
	}

	public ScrollbarDimensions WithMutateScrollTop(int pixels, TextEditorDimensions textEditorDimensions) =>
		WithSetScrollTop((int)Math.Ceiling(ScrollTop + pixels), textEditorDimensions);

	public ScrollbarDimensions WithSetScrollTop(int pixels, TextEditorDimensions textEditorDimensions)
	{
		var resultScrollTop = Math.Max(0, pixels);
		var maxScrollTop = (int)Math.Max(0, ScrollHeight - textEditorDimensions.Height);

		if (resultScrollTop > maxScrollTop)
			resultScrollTop = maxScrollTop;

		return this with { ScrollTop = resultScrollTop };
	}
}
