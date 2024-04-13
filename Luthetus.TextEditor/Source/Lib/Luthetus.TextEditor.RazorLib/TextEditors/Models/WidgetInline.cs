using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

/// <summary>
/// This type takes up some amount of width of a text editor row,
/// and therefore shifts any the left position of any columns that follow.
/// </summary>
public record WidgetInline(
    Key<WidgetInline> Key,
    string Title,
    string HtmlElementId,
    Type ComponentType,
    Dictionary<string, object?>? ComponentParameterMap);