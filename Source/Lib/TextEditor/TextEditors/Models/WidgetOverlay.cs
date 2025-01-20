using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

/// <summary>
/// This type is rendered 'on-top' of with respect to "z-index" the content.
/// </summary>
public sealed record WidgetOverlay(
    Key<WidgetOverlay> Key,
    string Title,
    string HtmlElementId,
    Type ComponentType,
    Dictionary<string, object?>? ComponentParameterMap);
