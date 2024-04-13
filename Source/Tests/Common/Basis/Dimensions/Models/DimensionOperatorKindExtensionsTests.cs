using Luthetus.Common.RazorLib.Dimensions.Models;

namespace Luthetus.Common.Tests.Basis.Dimensions.Models;

/// <summary>
/// <see cref="DimensionOperatorKindExtensions"/>
/// </summary>
public class DimensionOperatorKindExtensionsTests
{
    /// <summary>
    /// <see cref="DimensionOperatorKindExtensions.GetStyleString(DimensionOperatorKind)"/>
    /// </summary>
    [Fact]
    public void GetStyleString()
    {
        Assert.Equal("+", DimensionOperatorKind.Add.GetStyleString());
        Assert.Equal("-", DimensionOperatorKind.Subtract.GetStyleString());
        Assert.Equal("*", DimensionOperatorKind.Multiply.GetStyleString());
        Assert.Equal("/", DimensionOperatorKind.Divide.GetStyleString());
    }
}