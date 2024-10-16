using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class ReturnStatementNode : ISyntaxNode
{
    public ReturnStatementNode(KeywordToken keywordToken, IExpressionNode expressionNode)
    {
        KeywordToken = keywordToken;
        ExpressionNode = expressionNode;

        SetChildList();
    }

    public KeywordToken KeywordToken { get; }
    public IExpressionNode ExpressionNode { get; }

    public ISyntax[] ChildList { get; private set; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.ReturnStatementNode;
    
    public void SetChildList()
    {
    	var childCount = 2; // KeywordToken, ExpressionNode
            
        var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = KeywordToken;
		childList[i++] = ExpressionNode;
            
        ChildList = childList;
    }
}