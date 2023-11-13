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
    /// <see cref="ElementDimensions.DimensionAttributeBag"/>
    /// <see cref="ElementDimensions.ElementPositionKind"/>
    /// <see cref="ElementDimensions.StyleString"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        var elementDimensions = new ElementDimensions();

        Assert.Single(elementDimensions.DimensionAttributeBag, x => x.DimensionAttributeKind == DimensionAttributeKind.Width);
        Assert.Single(elementDimensions.DimensionAttributeBag, x => x.DimensionAttributeKind == DimensionAttributeKind.Height);
        Assert.Single(elementDimensions.DimensionAttributeBag, x => x.DimensionAttributeKind == DimensionAttributeKind.Left);
        Assert.Single(elementDimensions.DimensionAttributeBag, x => x.DimensionAttributeKind == DimensionAttributeKind.Right);
        Assert.Single(elementDimensions.DimensionAttributeBag, x => x.DimensionAttributeKind == DimensionAttributeKind.Top);
        Assert.Single(elementDimensions.DimensionAttributeBag, x => x.DimensionAttributeKind == DimensionAttributeKind.Bottom);

        Assert.Equal(ElementPositionKind.Static, elementDimensions.ElementPositionKind);
        Assert.Equal("position: static; ", elementDimensions.StyleString);

        // Test setter
        {
            elementDimensions.ElementPositionKind = ElementPositionKind.Fixed;

            Assert.Equal(ElementPositionKind.Fixed, elementDimensions.ElementPositionKind);
            Assert.Equal("position: fixed; ", elementDimensions.StyleString);
        }

        var widthDimensionAttribute = elementDimensions.DimensionAttributeBag.Single(x =>
            x.DimensionAttributeKind == DimensionAttributeKind.Width);

        widthDimensionAttribute.DimensionUnitBag.Add(new DimensionUnit
        {
            Value = 60,
            DimensionUnitKind = DimensionUnitKind.ViewportWidth,
        });

        var heightDimensionAttribute = elementDimensions.DimensionAttributeBag.Single(x =>
            x.DimensionAttributeKind == DimensionAttributeKind.Height);

        widthDimensionAttribute.DimensionUnitBag.Add(new DimensionUnit
        {
            Value = 60,
            DimensionUnitKind = DimensionUnitKind.ViewportHeight,
        });

        Assert.Equal("position: fixed; width: calc(60vw + 60vh);", elementDimensions.StyleString);
    }
}