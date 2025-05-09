using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.Decorations.Models;

/// <summary>
/// In a concurrent safe manner, get the content from the TextEditorModel
/// such that no insertions or deletions can occur during the setting of this property.
/// </summary>
public record TextEditorPresentationModelCalculation(string ContentAtRequest)
{
    public List<TextEditorTextModification> TextModificationsSinceRequestList = new();
    public IReadOnlyList<TextEditorTextSpan> TextSpanList = Array.Empty<TextEditorTextSpan>();
}