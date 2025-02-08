namespace Luthetus.Common.RazorLib.Menus.Models;

public record MenuOptionRecord
{
    public MenuOptionRecord(
    	string displayName,
	    MenuOptionKind menuOptionKind,
	    Func<Task>? onClickFunc = null,
	    MenuRecord? subMenu = null,
	    Type? widgetRendererType = null,
	    Dictionary<string, object?>? widgetParameterMap = null)
    {
    	DisplayName = displayName;
	    MenuOptionKind = menuOptionKind;
	    OnClickFunc = onClickFunc;
	    SubMenu = subMenu;
	    WidgetRendererType = widgetRendererType;
	    WidgetParameterMap = widgetParameterMap;
    }
    
    public string DisplayName { get; init; }
    public MenuOptionKind MenuOptionKind { get; init; }
    public Func<Task>? OnClickFunc{ get; init; }
    public MenuRecord? SubMenu { get; set; }
    public Type? WidgetRendererType { get; init; }
    public Dictionary<string, object?>? WidgetParameterMap { get; init; }
}