using Luthetus.TextEditor.RazorLib.Lexing;

namespace Luthetus.Ide.ClassLib.CodeAnalysis.C.Syntax.SyntaxTokens;

public interface ISyntaxToken : ISyntax
{
    public TextEditorTextSpan TextEditorTextSpan { get; }
}