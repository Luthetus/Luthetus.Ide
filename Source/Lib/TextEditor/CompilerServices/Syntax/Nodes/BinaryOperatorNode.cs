using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class BinaryOperatorNode : ISyntaxNode
{
    public BinaryOperatorNode(
        TypeClauseNode leftOperandTypeClauseNode,
        SyntaxToken operatorToken,
        TypeClauseNode rightOperandTypeClauseNode,
        TypeClauseNode resultTypeClauseNode)
    {
        LeftOperandTypeClauseNode = leftOperandTypeClauseNode;
        OperatorToken = operatorToken;
        RightOperandTypeClauseNode = rightOperandTypeClauseNode;
        ResultTypeClauseNode = resultTypeClauseNode;
    }

	private ISyntax[] _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

    public TypeClauseNode LeftOperandTypeClauseNode { get; }
    public SyntaxToken OperatorToken { get; }
    public TypeClauseNode RightOperandTypeClauseNode { get; }
	public TypeClauseNode ResultTypeClauseNode { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BinaryOperatorNode;
    
    public ISyntax[] GetChildList()
    {
    	if (!_childListIsDirty)
    		return _childList;
    	
    	var childCount = 1; // OperatorToken,
            
        var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = OperatorToken;
            
        _childList = childList;
        
    	_childListIsDirty = false;
    	return _childList;
    }
}