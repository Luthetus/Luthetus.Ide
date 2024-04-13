using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.Decorations.Models;

/// <summary>
/// In a concurrent safe manner, get the content from the TextEditorModel
/// such that no insertions or deletions can occur during the setting of this property.
/// </summary>
public record TextEditorPresentationModelCalculation(string ContentAtRequest)
{
    public List<TextEditorTextModification> TextModificationsSinceRequestList { get; set; } = new();
    public ImmutableArray<TextEditorTextSpan> TextSpanList { get; set; } = ImmutableArray<TextEditorTextSpan>.Empty;
}