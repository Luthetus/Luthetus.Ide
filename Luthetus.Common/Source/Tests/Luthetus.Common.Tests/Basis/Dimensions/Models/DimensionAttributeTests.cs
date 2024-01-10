using Luthetus.Common.RazorLib.Dimensions.Models;

namespace Luthetus.Common.Tests.Basis.Dimensions.Models;

/// <summary>
/// <see cref="DimensionAttribute"/>
/// </summary>
public class DimensionAttributeTests
{
    /// <summary>
    /// <see cref="DimensionAttribute.DimensionAttributeKind"/>
    /// <br/>----<br/>
    /// <see cref="DimensionAttribute.DimensionUnitList"/>
    /// <see cref="DimensionAttribute.StyleString"/>
    /// </summary>
    [Fact]
    public void DimensionAttributeKind()
    {
        // Single DimensionUnit
        {
            var dimensionAttribute = new DimensionAttribute
            {
                DimensionAttributeKind = RazorLib.Dimensions.Models.DimensionAttributeKind.Width,
            };

            dimensionAttribute.DimensionUnitList.Add(new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = 5
            });

            Assert.Equal("width: calc(5px);", dimensionAttribute.StyleString);
        }

        // Two DimensionUnit
        {
            var dimensionAttribute = new DimensionAttribute
            {
                DimensionAttributeKind = RazorLib.Dimensions.Models.DimensionAttributeKind.Width,
            };

            dimensionAttribute.DimensionUnitList.Add(new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.ViewportWidth,
                Value = 60
            });

            dimensionAttribute.DimensionUnitList.Add(new DimensionUnit
            {
                DimensionUnitKind = DimensionUnitKind.Pixels,
                Value = 33.333,
                DimensionOperatorKind = DimensionOperatorKind.Subtract,
            });

            Assert.Equal("width: calc(60vw - 33.333px);", dimensionAttribute.StyleString);
        }
    }
}