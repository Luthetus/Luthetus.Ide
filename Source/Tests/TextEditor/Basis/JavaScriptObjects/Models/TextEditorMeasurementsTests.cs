using Luthetus.TextEditor.RazorLib.TextEditors.Models;

namespace Luthetus.TextEditor.Tests.Basis.JavaScriptObjects.Models;

/// <summary>
/// <see cref="TextEditorMeasurements"/>
/// </summary>
public class TextEditorMeasurementsTests
{
    /// <summary>
    /// <see cref="TextEditorMeasurements(double, double, double, double, double, double, double, CancellationToken)"/>
	/// <br/>----<br/>
    /// <see cref="TextEditorMeasurements.ScrollLeft"/>
    /// <see cref="TextEditorMeasurements.ScrollTop"/>
    /// <see cref="TextEditorMeasurements.ScrollWidth"/>
    /// <see cref="TextEditorMeasurements.ScrollHeight"/>
    /// <see cref="TextEditorMeasurements.MarginScrollHeight"/>
    /// <see cref="TextEditorMeasurements.Width"/>
    /// <see cref="TextEditorMeasurements.Height"/>
    /// <see cref="TextEditorMeasurements.MeasurementsExpiredCancellationToken"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
		var cancellationTokenSource = new CancellationTokenSource();

        var scrollLeft = 30;
		var scrollTop = 70;
		var scrollWidth = 1000;
		var scrollHeight = 2000;
		var marginScrollHeight = 20;
		var width = 3000;
		var height = 4000;
		var boundingClientRectLeft = 250;
		var boundingClientRectTop = 300;

        var textEditorDimensions = new TextEditorDimensions(
            width,
            height,
            boundingClientRectLeft,
            boundingClientRectTop);
		
		var scrollbarDimensions = new ScrollbarDimensions(
            scrollLeft,
            scrollTop,
            scrollWidth,
            scrollHeight,
            marginScrollHeight);

		Assert.Equal(scrollLeft, scrollbarDimensions.ScrollLeft);
		Assert.Equal(scrollTop, scrollbarDimensions.ScrollTop);
		Assert.Equal(scrollWidth, scrollbarDimensions.ScrollWidth);
		Assert.Equal(scrollHeight, scrollbarDimensions.ScrollHeight);
		Assert.Equal(marginScrollHeight, scrollbarDimensions.MarginScrollHeight);
		Assert.Equal(width, textEditorDimensions.Width);
		Assert.Equal(height, textEditorDimensions.Height);
		Assert.Equal(boundingClientRectLeft, textEditorDimensions.BoundingClientRectLeft);
		Assert.Equal(boundingClientRectTop, textEditorDimensions.BoundingClientRectTop);
	}
}