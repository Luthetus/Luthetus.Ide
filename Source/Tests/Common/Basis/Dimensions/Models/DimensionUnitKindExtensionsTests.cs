using Luthetus.Common.RazorLib.Dimensions.Models;

namespace Luthetus.Common.Tests.Basis.Dimensions.Models;

/// <summary>
/// <see cref="DimensionUnitKindExtensions"/>
/// </summary>
public class DimensionUnitKindExtensionsTests
{
    /// <summary>
    /// <see cref="DimensionUnitKindExtensions.GetStyleString(DimensionUnitKind)"/>
    /// </summary>
    [Fact]
    public void GetStyleString()
    {
        Assert.Equal("px", DimensionUnitKind.Pixels.GetStyleString());
        Assert.Equal("vw", DimensionUnitKind.ViewportWidth.GetStyleString());
        Assert.Equal("vh", DimensionUnitKind.ViewportHeight.GetStyleString());
        Assert.Equal("%", DimensionUnitKind.Percentage.GetStyleString());
        Assert.Equal("rch", DimensionUnitKind.RootCharacterWidth.GetStyleString());
        Assert.Equal("rem", DimensionUnitKind.RootCharacterHeight.GetStyleString());
        Assert.Equal("ch", DimensionUnitKind.CharacterWidth.GetStyleString());
        Assert.Equal("em", DimensionUnitKind.CharacterHeight.GetStyleString());
    }
}