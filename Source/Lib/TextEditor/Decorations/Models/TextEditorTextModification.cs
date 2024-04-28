using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.RazorLib.Decorations.Models;

public record TextEditorTextModification(bool WasInsertion, TextEditorTextSpan TextEditorTextSpan);