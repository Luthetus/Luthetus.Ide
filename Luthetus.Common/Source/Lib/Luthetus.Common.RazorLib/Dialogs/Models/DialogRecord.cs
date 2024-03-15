using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.PolymorphicUis.Models;

namespace Luthetus.Common.RazorLib.Dialogs.Models;

public record DialogRecord : IPolymorphicDialog
{
	public DialogRecord(
        Key<IPolymorphicUiRecord> polymorphicUiKey,
        string title,
        Type dialogRendererType,
        Dictionary<string, object?>? dialogParameterMap,
        string? cssClassString,
		bool isResizable)
    {
		PolymorphicUiKey = polymorphicUiKey;
		Title = title;
		DialogRendererType = dialogRendererType;
		DialogParameterMap = dialogParameterMap;
		CssClass = cssClassString;
		DialogCssClassString = cssClassString;
		DialogIsResizable = isResizable;
		DialogElementDimensions = DialogConstructDefaultElementDimensions();
	}

    public string Title { get; init; }
    public Type DialogRendererType { get; init; }
    public Dictionary<string, object?>? DialogParameterMap { get; init; }
    public string? DialogCssClassString { get; set; }

	public Key<IPolymorphicUiRecord> PolymorphicUiKey { get; }
	public string? CssClass { get; }
	public string? CssStyle { get; }
	public Type RendererType { get; }

    public ElementDimensions DialogElementDimensions { get; init; }
    public bool DialogIsMinimized { get; set; }
    public bool DialogIsMaximized { get; set; }
    public bool DialogIsResizable { get; set; }
    public string DialogFocusPointHtmlElementId => $"luth_dialog-focus-point_{PolymorphicUiKey.Guid}";

    public ElementDimensions DialogConstructDefaultElementDimensions()
    {
        var elementDimensions = new ElementDimensions
        {
            ElementPositionKind = ElementPositionKind.Fixed
        };

        // Width
        {
            var width = elementDimensions.DimensionAttributeList.Single(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Width);

            width.DimensionUnitList.Add(new DimensionUnit
            {
                Value = 60,
                DimensionUnitKind = DimensionUnitKind.ViewportWidth
            });
        }

        // Height
        {
            var height = elementDimensions.DimensionAttributeList.Single(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Height);

            height.DimensionUnitList.Add(new DimensionUnit
            {
                Value = 60,
                DimensionUnitKind = DimensionUnitKind.ViewportHeight
            });
        }

        // Left
        {
            var left = elementDimensions.DimensionAttributeList.Single(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Left);

            left.DimensionUnitList.Add(new DimensionUnit
            {
                Value = 20,
                DimensionUnitKind = DimensionUnitKind.ViewportWidth
            });
        }

        // Top
        {
            var top = elementDimensions.DimensionAttributeList.Single(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Top);

            top.DimensionUnitList.Add(new DimensionUnit
            {
                Value = 20,
                DimensionUnitKind = DimensionUnitKind.ViewportHeight
            });
        }

        return elementDimensions;
    }

	public IPolymorphicDialog DialogSetIsMaximized(bool isMaximized)
	{
		return this with
		{
			DialogIsMaximized = isMaximized
		};
	}
}