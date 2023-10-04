using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Dialogs.Models;

public record DialogRecord(
    Key<DialogRecord> Key,
    string Title,
    Type RendererType,
    Dictionary<string, object?>? Parameters,
    string? CssClassString)
{
    public ElementDimensions ElementDimensions { get; init; } = ConstructDefaultDialogDimensions();
    public bool IsMinimized { get; set; }
    public bool IsMaximized { get; set; }
    public bool IsResizable { get; set; }

    public static ElementDimensions ConstructDefaultDialogDimensions()
    {
        var elementDimensions = new ElementDimensions
        {
            ElementPositionKind = ElementPositionKind.Fixed
        };

        // Width
        {
            var width = elementDimensions.DimensionAttributeBag.Single(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Width);

            width.DimensionUnitBag.Add(new DimensionUnit
            {
                Value = 60,
                DimensionUnitKind = DimensionUnitKind.ViewportWidth
            });
        }

        // Height
        {
            var height = elementDimensions.DimensionAttributeBag.Single(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Height);

            height.DimensionUnitBag.Add(new DimensionUnit
            {
                Value = 60,
                DimensionUnitKind = DimensionUnitKind.ViewportHeight
            });
        }

        // Left
        {
            var left = elementDimensions.DimensionAttributeBag.Single(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Left);

            left.DimensionUnitBag.Add(new DimensionUnit
            {
                Value = 20,
                DimensionUnitKind = DimensionUnitKind.ViewportWidth
            });
        }

        // Top
        {
            var top = elementDimensions.DimensionAttributeBag.Single(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Top);

            top.DimensionUnitBag.Add(new DimensionUnit
            {
                Value = 20,
                DimensionUnitKind = DimensionUnitKind.ViewportHeight
            });
        }

        return elementDimensions;
    }
}