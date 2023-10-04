using System.Text;

namespace Luthetus.Common.RazorLib.Dimensions.Models;

public class ElementDimensions
{
    public ElementDimensions()
    {
        DimensionAttributeBag.AddRange(new[]
        {
            new DimensionAttribute { DimensionAttributeKind = DimensionAttributeKind.Width },
            new DimensionAttribute { DimensionAttributeKind = DimensionAttributeKind.Height },
            new DimensionAttribute { DimensionAttributeKind = DimensionAttributeKind.Left },
            new DimensionAttribute { DimensionAttributeKind = DimensionAttributeKind.Right },
            new DimensionAttribute { DimensionAttributeKind = DimensionAttributeKind.Top },
            new DimensionAttribute { DimensionAttributeKind = DimensionAttributeKind.Bottom }
        });
    }

    public List<DimensionAttribute> DimensionAttributeBag { get; } = new();
    public ElementPositionKind ElementPositionKind { get; set; } = ElementPositionKind.Static;
    public string StyleString => GetStyleString();

    private string GetStyleString()
    {
        var styleBuilder = new StringBuilder();

        styleBuilder.Append($"position: {ElementPositionKind.ToString().ToLower()}; ");

        foreach (var dimensionAttribute in DimensionAttributeBag)
        {
            styleBuilder.Append(dimensionAttribute.StyleString);
        }

        return styleBuilder.ToString();
    }
}