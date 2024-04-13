using Luthetus.Common.RazorLib.Dimensions.Models;

namespace Luthetus.Common.Tests.Basis.Dimensions.Models;

/// <summary>
/// <see cref="DimensionUnit"/>
/// </summary>
public class DimensionUnitTests
{
    /// <summary>
    /// <see cref="DimensionUnit.Value"/>
    /// <br/>----<br/>
    /// <see cref="DimensionUnit.DimensionUnitKind"/>
    /// <see cref="DimensionUnit.DimensionOperatorKind"/>
    /// <see cref="DimensionUnit.Purpose"/>
    /// </summary>
    [Fact]
    public void Value()
    {
        var value = 5;
        var dimensionOperatorKind = DimensionOperatorKind.Add;
        var purpose = "displacement";
        var dimensionUnitKind = DimensionUnitKind.Pixels;

        var dimensionUnit = new DimensionUnit
        {
            Value = value,
            DimensionOperatorKind = dimensionOperatorKind,
            Purpose = purpose,
            DimensionUnitKind = dimensionUnitKind,
        };

        Assert.Equal(value, dimensionUnit.Value);
        Assert.Equal(dimensionOperatorKind, dimensionUnit.DimensionOperatorKind);
        Assert.Equal(purpose, dimensionUnit.Purpose);
        Assert.Equal(dimensionUnitKind, dimensionUnit.DimensionUnitKind);
    }
}