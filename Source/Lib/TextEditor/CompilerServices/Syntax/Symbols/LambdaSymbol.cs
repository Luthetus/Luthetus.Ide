using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Symbols;

public record struct LambdaSymbol : ISymbol
{
    public LambdaSymbol(TextEditorTextSpan textSpan, LambdaExpressionNode lambdaExpressionNode)
    {
        TextSpan = textSpan;
        LambdaExpressionNode = lambdaExpressionNode;
    }

    public LambdaExpressionNode LambdaExpressionNode { get; }
    
    public TextEditorTextSpan TextSpan { get; }
    public string SymbolKindString => SyntaxKind.ToString();

    public SyntaxKind SyntaxKind => SyntaxKind.LambdaSymbol;
}
