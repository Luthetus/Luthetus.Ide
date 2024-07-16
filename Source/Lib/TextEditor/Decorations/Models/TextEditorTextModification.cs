using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.Decorations.Models;

public record TextEditorTextModification(bool WasInsertion, TextEditorTextSpan TextEditorTextSpan);