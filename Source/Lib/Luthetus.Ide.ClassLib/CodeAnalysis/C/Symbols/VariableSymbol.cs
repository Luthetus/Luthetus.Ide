using Luthetus.TextEditor.RazorLib.Lexing;
using Luthetus.Ide.ClassLib.CodeAnalysis.C.Syntax;

namespace Luthetus.Ide.ClassLib.CodeAnalysis.C.Symbols;

public class VariableSymbol : ISymbol
{
    public VariableSymbol(
        TextEditorTextSpan textSpan)
    {
        TextSpan = textSpan;
    }

    public TextEditorTextSpan TextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.VariableSymbol;
}

