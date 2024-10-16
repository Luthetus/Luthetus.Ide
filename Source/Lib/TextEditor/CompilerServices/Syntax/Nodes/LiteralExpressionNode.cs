using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class LiteralExpressionNode : IExpressionNode
{
    public LiteralExpressionNode(ISyntaxToken literalSyntaxToken, TypeClauseNode typeClauseNode)
    {
        LiteralSyntaxToken = literalSyntaxToken;
        ResultTypeClauseNode = typeClauseNode;

        SetChildList();
    }

    public ISyntaxToken LiteralSyntaxToken { get; }
    public TypeClauseNode ResultTypeClauseNode { get; }

    public ISyntax[] ChildList { get; private set; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.LiteralExpressionNode;
    
    public void SetChildList()
    {
    	var childCount = 2; // LiteralSyntaxToken, ResultTypeClauseNode,
            
        var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = LiteralSyntaxToken;
		childList[i++] = ResultTypeClauseNode;
            
        ChildList = childList;
    }
}
