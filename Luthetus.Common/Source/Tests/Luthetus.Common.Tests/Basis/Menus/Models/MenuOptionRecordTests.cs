namespace Luthetus.Common.RazorLib.Menus.Models;

public record MenuOptionRecordTests(
    string DisplayName,
    MenuOptionKind MenuOptionKind,
    Action? OnClick = null,
    MenuRecord? SubMenu = null,
    Type? WidgetRendererType = null,
    Dictionary<string, object?>? WidgetParameterMap = null);