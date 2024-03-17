using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Dialogs.Models;

public record DialogViewModel : IDialog
{
	public DialogViewModel(
		Key<IDynamicViewModel> dynamicViewModelKey,
		string title,
		Type componentType,
		Dictionary<string, object?>? componentParameterMap,
		string cssClass,
		bool isResizable)
	{
		DynamicViewModelKey = dynamicViewModelKey;
		Title = title;
        ComponentType = componentType;
        ComponentParameterMap = componentParameterMap;
        CssClass = cssClass;
		DialogIsResizable = isResizable;
	}

	public Key<IDynamicViewModel> DynamicViewModelKey { get; }
	public bool DialogIsMinimized { get; set; }
    public bool DialogIsMaximized { get; set; }
    public bool DialogIsResizable { get; set; }
    public string DialogFocusPointHtmlElementId { get; set; }
    public Key<IDialog> DialogKey { get; }
    public ElementDimensions DialogElementDimensions { get; set; } = new();
	public string Title { get; set; }
    public Type ComponentType { get; }
    public Dictionary<string, object?>? ComponentParameterMap { get; init; }
    public string CssClass { get; set; }
    public string? CssStyle { get; set; }

    public virtual string FocusPointHtmlElementId => $"luth_dialog-focus-point_{DynamicViewModelKey.Guid}";

	public IDialog SetParameterMap(Dictionary<string, object?>? componentParameterMap)
	{
		return this with { ComponentParameterMap = componentParameterMap };
	}

	public IDialog SetTitle(string title)
	{
		return this with { Title = title };
	}

	public IDialog SetIsMinimized(bool isMinimized)
	{
		return this with { DialogIsMinimized = isMinimized };
	}

	public IDialog SetIsMaximized(bool isMaximized)
	{
		return this with { DialogIsMaximized = isMaximized };
	}

	public IDialog SetIsResizable(bool isResizable)
	{
		return this with { DialogIsResizable = isResizable };
	}

	public IDialog SetCssClassString(string cssClassString)
	{
		return this with { CssClass = cssClassString };
	}
}
