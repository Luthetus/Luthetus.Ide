using Luthetus.Common.RazorLib.Dimensions.Models;

namespace Luthetus.Common.RazorLib.PolymorphicUis.Models;

public interface IPolymorphicDialog : IPolymorphicUiRecord
{
	public Type DialogRendererType { get; }
	public Dictionary<string, object?>? DialogParameterMap { get; }
	public ElementDimensions DialogElementDimensions { get; }
    public bool DialogIsMinimized { get; set; }
    public bool DialogIsMaximized { get; set; }
    public bool DialogIsResizable { get; set; }
    public string DialogCssClassString { get; set; }
    public string DialogFocusPointHtmlElementId => $"luth_dialog-focus-point_{PolymorphicUiKey.Guid}";

	public bool IsDialog { get; set; }

    public ElementDimensions DialogConstructDefaultElementDimensions();
	
	public IPolymorphicDialog DialogSetIsMaximized(bool isMaximized);
}
