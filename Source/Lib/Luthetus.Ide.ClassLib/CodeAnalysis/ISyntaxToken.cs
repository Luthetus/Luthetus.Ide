using Luthetus.TextEditor.RazorLib.Lexing;

namespace Luthetus.Ide.ClassLib.CodeAnalysis;

public interface ISyntaxToken : ISyntax
{
    public TextEditorTextSpan TextSpan { get; }
}
