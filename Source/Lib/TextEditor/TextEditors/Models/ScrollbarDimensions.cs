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
public record ScrollbarDimensions(
	double ScrollLeft,
    double ScrollTop,
    double ScrollWidth,
    double ScrollHeight,
    double MarginScrollHeight)
{
	public ScrollbarDimensions MutateScrollLeft(int pixels, TextEditorDimensions textEditorDimensions)
	{
		var scrollLeftResult = ScrollLeft + pixels;
		var maxScrollLeft = ScrollWidth - textEditorDimensions.Width;

		if (scrollLeftResult < 0)
		{
			scrollLeftResult = 0;
		}
		else if (scrollLeftResult > maxScrollLeft)
		{
			scrollLeftResult = maxScrollLeft;
		}

		return this with
		{
			ScrollLeft = scrollLeftResult
		};
	}

	public ScrollbarDimensions SetScrollLeft(int pixels, TextEditorDimensions textEditorDimensions)
	{
		var scrollLeftResult = pixels;
		var maxScrollLeft = (int)Math.Ceiling(ScrollWidth - textEditorDimensions.Width);

		if (scrollLeftResult < 0)
		{
			scrollLeftResult = 0;
		}
		else if (scrollLeftResult > maxScrollLeft)
		{
			scrollLeftResult = maxScrollLeft;
		}

		return this with
		{
			ScrollLeft = scrollLeftResult
		};
	}

	public ScrollbarDimensions MutateScrollTop(int pixels, TextEditorDimensions textEditorDimensions)
	{
		var scrollTopResult = ScrollTop + pixels;
		var maxScrollTop = ScrollHeight - textEditorDimensions.Height;

		if (scrollTopResult < 0)
		{
			scrollTopResult = 0;
		}
		else if (scrollTopResult > maxScrollTop)
		{
			scrollTopResult = maxScrollTop;
		}

		return this with
		{
			ScrollTop = scrollTopResult
		};
	}

	public ScrollbarDimensions SetScrollTop(int pixels, TextEditorDimensions textEditorDimensions)
	{
		var scrollTopResult = pixels;
		var maxScrollTop = (int)Math.Ceiling(ScrollHeight - textEditorDimensions.Height);

		if (scrollTopResult < 0)
		{
			scrollTopResult = 0;
		}
		else if (scrollTopResult > maxScrollTop)
		{
			scrollTopResult = maxScrollTop;
		}

		return this with
		{
			ScrollTop = scrollTopResult
		};
	}
}
