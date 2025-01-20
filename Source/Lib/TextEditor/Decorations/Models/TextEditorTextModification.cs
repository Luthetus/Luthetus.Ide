using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.Decorations.Models;

public record struct TextEditorTextModification(bool WasInsertion, TextEditorTextSpan TextEditorTextSpan);