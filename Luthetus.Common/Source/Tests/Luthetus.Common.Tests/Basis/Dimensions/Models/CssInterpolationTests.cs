using Luthetus.Common.RazorLib.Dimensions.Models;

namespace Luthetus.Common.Tests.Basis.Dimensions.Models;

/// <summary>
/// <see cref="CssInterpolation"/>
/// </summary>
public class CssInterpolationTests
{
    /// <summary>
    /// <see cref="CssInterpolation.ToCssValue(double)"/>
    /// </summary>
    [Fact]
    public void ToCssValueDouble()
    {
        var originalNumber = (double)1.257;
        var cssValue = originalNumber.ToCssValue();

        Assert.Equal(originalNumber.ToString(), cssValue);
    }

    /// <summary>
    /// <see cref="CssInterpolation.ToCssValue(decimal)"/>
    /// </summary>
    [Fact]
    public void ToCssValueDecimal()
    {
        var originalNumber = (decimal)1.257;
        var cssValue = originalNumber.ToCssValue();

        Assert.Equal(originalNumber.ToString(), cssValue);
    }

    /// <summary>
    /// <see cref="CssInterpolation.ToCssValue(float)"/>
    /// </summary>
    [Fact]
    public void ToCssValueFloat()
    {
        var originalNumber = (float)1.257;
        var cssValue = originalNumber.ToCssValue();

        Assert.Equal(originalNumber.ToString(), cssValue);
    }

    /// <summary>
    /// <see cref="CssInterpolation.ToCssValue(int)"/>
    /// </summary>
    [Fact]
    public void ToCssValueInt()
    {
        var originalNumber = 1;
        var cssValue = originalNumber.ToCssValue();

        Assert.Equal(originalNumber.ToString(), cssValue);
    }
}