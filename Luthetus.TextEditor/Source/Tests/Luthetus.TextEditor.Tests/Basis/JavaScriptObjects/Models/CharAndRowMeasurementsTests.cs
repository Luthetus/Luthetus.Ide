using Xunit;
using Luthetus.TextEditor.RazorLib.JavaScriptObjects.Models;

namespace Luthetus.TextEditor.Tests.Basis.JavaScriptObjects.Models;

/// <summary>
/// <see cref="CharAndRowMeasurements"/>
/// </summary>
public class CharAndRowMeasurementsTests
{
    /// <summary>
    /// <see cref="CharAndRowMeasurements(double, double)"/>
	/// <br/>----<br/>
    /// <see cref="CharAndRowMeasurements.CharacterWidth"/>
    /// <see cref="CharAndRowMeasurements.RowHeight"/>
    /// </summary>
    [Fact]
	public void Constructor()
	{
		var characterWidth = 10.14;
		var rowHeight = 33.77;

        var charAndRowMeasurements = new CharAndRowMeasurements(
            characterWidth,
            rowHeight);

		Assert.Equal(characterWidth, charAndRowMeasurements.CharacterWidth);
		Assert.Equal(rowHeight, charAndRowMeasurements.RowHeight);
	}
}