using Luthetus.Common.RazorLib.Dimensions.Models;

namespace Luthetus.Common.Tests.Basis.Dimensions.Models;

/// <summary>
/// <see cref="ElementDimensions"/>
/// </summary>
public class ElementDimensionsTests
{
    /// <summary>
    /// <see cref="ElementDimensions()"/>
    /// <br/>----<br/>
    /// <see cref="ElementDimensions.DimensionAttributeList"/>
    /// <see cref="ElementDimensions.ElementPositionKind"/>
    /// <see cref="ElementDimensions.StyleString"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var elementDimensions = new ElementDimensions();

        Assert.Single(elementDimensions.DimensionAttributeList, x => x.DimensionAttributeKind == DimensionAttributeKind.Width);
        Assert.Single(elementDimensions.DimensionAttributeList, x => x.DimensionAttributeKind == DimensionAttributeKind.Height);
        Assert.Single(elementDimensions.DimensionAttributeList, x => x.DimensionAttributeKind == DimensionAttributeKind.Left);
        Assert.Single(elementDimensions.DimensionAttributeList, x => x.DimensionAttributeKind == DimensionAttributeKind.Right);
        Assert.Single(elementDimensions.DimensionAttributeList, x => x.DimensionAttributeKind == DimensionAttributeKind.Top);
        Assert.Single(elementDimensions.DimensionAttributeList, x => x.DimensionAttributeKind == DimensionAttributeKind.Bottom);

        Assert.Equal(ElementPositionKind.Static, elementDimensions.ElementPositionKind);
        Assert.Equal("position: static; ", elementDimensions.StyleString);

        // Test setter
        {
            elementDimensions.ElementPositionKind = ElementPositionKind.Fixed;

            Assert.Equal(ElementPositionKind.Fixed, elementDimensions.ElementPositionKind);
            Assert.Equal("position: fixed; ", elementDimensions.StyleString);
        }

        var widthDimensionAttribute = elementDimensions.DimensionAttributeList.Single(x =>
            x.DimensionAttributeKind == DimensionAttributeKind.Width);

        widthDimensionAttribute.DimensionUnitList.Add(new DimensionUnit
        {
            Value = 60,
            DimensionUnitKind = DimensionUnitKind.ViewportWidth,
        });

        var heightDimensionAttribute = elementDimensions.DimensionAttributeList.Single(x =>
            x.DimensionAttributeKind == DimensionAttributeKind.Height);

        widthDimensionAttribute.DimensionUnitList.Add(new DimensionUnit
        {
            Value = 60,
            DimensionUnitKind = DimensionUnitKind.ViewportHeight,
        });

        Assert.Equal("position: fixed; width: calc(60vw + 60vh);", elementDimensions.StyleString);
    }
}