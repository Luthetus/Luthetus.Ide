using Luthetus.Ide.ClassLib.CodeAnalysis.C.Syntax;
using Luthetus.TextEditor.RazorLib.Lexing;

namespace Luthetus.Ide.ClassLib.CodeAnalysis.CSharp.Syntax.SyntaxTokens;

public class NumericLiteralToken : ISyntaxToken
{
    public NumericLiteralToken(TextEditorTextSpan textSpan)
    {
        TextSpan = textSpan;
    }

    public TextEditorTextSpan TextSpan { get; }

    public SyntaxKind SyntaxKind => SyntaxKind.NumericLiteralToken;
}
