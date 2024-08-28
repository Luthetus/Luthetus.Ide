using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Widgets.Models;

public class WidgetModel
{
	public WidgetModel(
		Type componentType,
		Dictionary<string, object?>? componentParameterMap,
		string cssClass,
		string cssStyle)
	{
		ComponentType = componentType;
		ComponentParameterMap = componentParameterMap;
		CssClass = cssClass;
		CssStyle = cssStyle;
	}

	public Type ComponentType { get; }
    public Dictionary<string, object?>? ComponentParameterMap { get; }
    public string CssClass { get; }
    public string CssStyle { get; }
    public Key<WidgetModel> Key { get; init; } = Key<WidgetModel>.NewKey();
}
