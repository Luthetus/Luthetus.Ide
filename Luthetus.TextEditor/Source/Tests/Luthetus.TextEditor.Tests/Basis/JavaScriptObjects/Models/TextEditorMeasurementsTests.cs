using Xunit;
using Luthetus.TextEditor.RazorLib.JavaScriptObjects.Models;

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
		var measurementsExpiredCancellationToken = cancellationTokenSource.Token;

        var textEditorMeasurements = new TextEditorMeasurements(
            scrollLeft,
            scrollTop,
            scrollWidth,
            scrollHeight,
            marginScrollHeight,
            width,
            height,
            measurementsExpiredCancellationToken);

		Assert.Equal(scrollLeft, textEditorMeasurements.ScrollLeft);
		Assert.Equal(scrollTop, textEditorMeasurements.ScrollTop);
		Assert.Equal(scrollWidth, textEditorMeasurements.ScrollWidth);
		Assert.Equal(scrollHeight, textEditorMeasurements.ScrollHeight);
		Assert.Equal(marginScrollHeight, textEditorMeasurements.MarginScrollHeight);
		Assert.Equal(width, textEditorMeasurements.Width);
		Assert.Equal(height, textEditorMeasurements.Height);
		Assert.Equal(measurementsExpiredCancellationToken, textEditorMeasurements.MeasurementsExpiredCancellationToken);
	}
}