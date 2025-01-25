using System.Text;

namespace Luthetus.Common.RazorLib.Dimensions.Models;

public class ElementDimensions
{
    public DimensionAttribute WidthDimensionAttribute { get; set; } = new(DimensionAttributeKind.Width);
    public DimensionAttribute HeightDimensionAttribute { get; set; } = new(DimensionAttributeKind.Height);
    public DimensionAttribute LeftDimensionAttribute { get; set; } = new(DimensionAttributeKind.Left);
    public DimensionAttribute RightDimensionAttribute { get; set; } = new(DimensionAttributeKind.Right);
    public DimensionAttribute TopDimensionAttribute { get; set; } = new(DimensionAttributeKind.Top);
    public DimensionAttribute BottomDimensionAttribute { get; set; } = new(DimensionAttributeKind.Bottom);
    
    public ElementPositionKind ElementPositionKind { get; set; } = ElementPositionKind.Static;
    public string StyleString => GetStyleString();

    private string GetStyleString()
    {
        var styleBuilder = new StringBuilder();

        styleBuilder.Append($"position: {ElementPositionKind.ToString().ToLower()}; ");

        styleBuilder.Append(WidthDimensionAttribute.StyleString);
        styleBuilder.Append(HeightDimensionAttribute.StyleString);
        styleBuilder.Append(LeftDimensionAttribute.StyleString);
        styleBuilder.Append(RightDimensionAttribute.StyleString);
        styleBuilder.Append(TopDimensionAttribute.StyleString);
        styleBuilder.Append(BottomDimensionAttribute.StyleString);

        return styleBuilder.ToString();
    }
}