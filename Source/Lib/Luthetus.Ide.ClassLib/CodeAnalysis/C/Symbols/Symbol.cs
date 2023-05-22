using Luthetus.TextEditor.RazorLib.Lexing;
using Luthetus.Ide.ClassLib.CodeAnalysis.C.Syntax;

namespace Luthetus.Ide.ClassLib.CodeAnalysis.C.Symbols;

public interface ISymbol
{
    public TextEditorTextSpan TextSpan { get; }
    public SyntaxKind SyntaxKind { get; }
}

