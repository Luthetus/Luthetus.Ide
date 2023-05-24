using Luthetus.TextEditor.RazorLib.Lexing;

namespace Luthetus.Ide.ClassLib.CodeAnalysis.Symbols;

public interface ISymbol
{
    public TextEditorTextSpan TextSpan { get; }
    public SyntaxKind SyntaxKind { get; }
}

