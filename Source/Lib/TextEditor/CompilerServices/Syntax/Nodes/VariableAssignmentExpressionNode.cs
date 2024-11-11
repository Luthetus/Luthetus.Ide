using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class VariableAssignmentExpressionNode : IExpressionNode
{
    public VariableAssignmentExpressionNode(
        IdentifierToken variableIdentifierToken,
        EqualsToken equalsToken,
        IExpressionNode expressionNode)
    {
        VariableIdentifierToken = variableIdentifierToken;
        EqualsToken = equalsToken;
        ExpressionNode = expressionNode;
    }

	private ISyntax[] _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

    public IdentifierToken VariableIdentifierToken { get; }
    public EqualsToken EqualsToken { get; }
    public IExpressionNode ExpressionNode { get; private set; }
    public TypeClauseNode ResultTypeClauseNode => ExpressionNode.ResultTypeClauseNode;

    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.VariableAssignmentExpressionNode;
    
    public VariableAssignmentExpressionNode SetExpressionNode(IExpressionNode expressionNode)
    {
    	ExpressionNode = expressionNode;
    	_childListIsDirty = true;
    	return this;
    }
    
    public ISyntax[] GetChildList()
    {
    	if (!_childListIsDirty)
    		return _childList;
    	
    	_childList = new ISyntax[]
        {
            VariableIdentifierToken,
            EqualsToken,
            ExpressionNode,
        };
        
    	_childListIsDirty = false;
    	return _childList;
    }
}
