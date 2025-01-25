using System.Text;

namespace Luthetus.Common.RazorLib.Dimensions.Models;

public class ElementDimensions
{
    public ElementDimensions()
    {
        DimensionAttributeList.AddRange(new[]
        {
            new DimensionAttribute(DimensionAttributeKind.Width),
            new DimensionAttribute(DimensionAttributeKind.Height),
            new DimensionAttribute(DimensionAttributeKind.Left),
            new DimensionAttribute(DimensionAttributeKind.Right),
            new DimensionAttribute(DimensionAttributeKind.Top),
            new DimensionAttribute(DimensionAttributeKind.Bottom)
        });
    }

    public List<DimensionAttribute> DimensionAttributeList { get; } = new();
    public ElementPositionKind ElementPositionKind { get; set; } = ElementPositionKind.Static;
    public string StyleString => GetStyleString();

    private string GetStyleString()
    {
        var styleBuilder = new StringBuilder();

        styleBuilder.Append($"position: {ElementPositionKind.ToString().ToLower()}; ");

        foreach (var dimensionAttribute in DimensionAttributeList)
        {
            styleBuilder.Append(dimensionAttribute.StyleString);
        }

        return styleBuilder.ToString();
    }
}