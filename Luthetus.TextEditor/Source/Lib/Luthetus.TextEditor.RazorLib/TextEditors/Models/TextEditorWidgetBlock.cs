using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

/// <summary>
/// This type takes up 100% of the width of a text editor row,
/// and therefore shifts the top position of any rows that follow.
/// </summary>
public record TextEditorWidgetBlock(
    Key<TextEditorWidgetBlock> Key,
    string Title,
    string HtmlElementId,
    Type ComponentType,
    Dictionary<string, object?>? ComponentParameterMap);
