using Luthetus.TextEditor.RazorLib.JavaScriptObjects.Models;

namespace Luthetus.TextEditor.Tests.Basis.JavaScriptObjects.Models;

/// <summary>
/// <see cref="CharAndLineMeasurements"/>
/// </summary>
public class CharAndRowMeasurementsTests
{
    /// <summary>
    /// <see cref="CharAndLineMeasurements(double, double)"/>
	/// <br/>----<br/>
    /// <see cref="CharAndLineMeasurements.CharacterWidth"/>
    /// <see cref="CharAndLineMeasurements.LineHeight"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
		var characterWidth = 10.14;
		var rowHeight = 33.77;

        var charAndRowMeasurements = new CharAndLineMeasurements(
            characterWidth,
            rowHeight);

		Assert.Equal(characterWidth, charAndRowMeasurements.CharacterWidth);
		Assert.Equal(rowHeight, charAndRowMeasurements.LineHeight);
	}
}