namespace Luthetus.TextEditor.RazorLib.PolymorphicUis.Models;

public partial record TextEditorViewModelPolymorphicUi : IPolymorphicDialog
{
	private Key<DialogRecord> _dialogKey = Key<DialogRecord>.NewKey();

	public ElementDimensions DialogElementDimensions { get; }
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
}
