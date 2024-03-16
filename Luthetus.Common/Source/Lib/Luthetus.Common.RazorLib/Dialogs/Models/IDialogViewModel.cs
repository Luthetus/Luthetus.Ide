using Luthetus.Common.RazorLib.PolymorphicViewModels.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Dialogs.Models;

public interface IDialogViewModel
{
	public IPolymorphicViewModel? PolymorphicViewModel { get; }
	public Key<IDialogViewModel> Key { get; }
	public Type RendererType { get; }
	public Dictionary<string, object?>? ParameterMap { get; }
	public ElementDimensions ElementDimensions { get; }
	public string Title { get; }
	public bool IsMinimized { get; }
    public bool IsMaximized { get; }
    public bool IsResizable { get; }
    public string CssClassString { get; }
    public string FocusPointHtmlElementId { get; }

	public IDialogViewModel SetParameterMap(Dictionary<string, object?>? parameterMap);
	public IDialogViewModel SetTitle(string title);    
	public IDialogViewModel SetIsMinimized(bool isMinimized);
	public IDialogViewModel SetIsMaximized(bool isMaximized);
	public IDialogViewModel SetIsResizable(bool isResizable);
	public IDialogViewModel SetCssClassString(string cssClassString);

	public static ElementDimensions ConstructDefaultElementDimensions()
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

