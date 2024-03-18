using Luthetus.Common.RazorLib.Dimensions.Models;

namespace Luthetus.Common.RazorLib.Dynamics.Models;

public interface IDialog : IDynamicViewModel
{
    public bool DialogIsMinimized { get; set; }
    public bool DialogIsMaximized { get; set; }
    public bool DialogIsResizable { get; set; }
    public string DialogFocusPointHtmlElementId { get; set; }
	public ElementDimensions DialogElementDimensions { get; set; }
	public string? DialogCssClass { get; set; }
	public string? DialogCssStyle { get; set; }

	public IDialog SetDialogIsMaximized(bool isMaximized);
}
