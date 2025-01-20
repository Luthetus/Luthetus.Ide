using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

/// <summary>
/// This type takes up 100% of the width of a text editor row,
/// and therefore shifts the top position of any rows that follow.
/// </summary>
///
/// <param name="LineIndex">
/// When the text editor renders this line,
/// it will then render where the line is,
/// the corresponding widget.
///
/// So, the widget block-wise takes the UI row
/// that the line was occupying, and the line
/// content is moved down by the height of the widget.
///
/// An index greater than or equal to the count of lines
/// is permitted, to have the widget render at the bottom
/// of the text editor.
///
/// If the index is greater than the count of lines,
/// then the index is taken to be the count of lines instead.
/// </param>
public sealed record WidgetBlock(
    Key<WidgetBlock> Key,
    string Title,
    string HtmlElementId,
    int LineIndex,
    Type ComponentType,
    Dictionary<string, object?>? ComponentParameterMap);
