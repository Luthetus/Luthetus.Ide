using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Symbols;

public record struct LambdaSymbol : ISymbol
{
    public LambdaSymbol(int symbolId, TextEditorTextSpan textSpan, LambdaExpressionNode lambdaExpressionNode)
    {
    	SymbolId = symbolId;
        TextSpan = textSpan;
        LambdaExpressionNode = lambdaExpressionNode;
    }

    public LambdaExpressionNode LambdaExpressionNode { get; }
    
    public int SymbolId { get; }
    public TextEditorTextSpan TextSpan { get; }
    public string SymbolKindString => SyntaxKind.ToString();

    public SyntaxKind SyntaxKind => SyntaxKind.LambdaSymbol;
}
