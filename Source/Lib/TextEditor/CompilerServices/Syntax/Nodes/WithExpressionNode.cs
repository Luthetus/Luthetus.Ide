using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class WithExpressionNode : IExpressionNode
{
    public WithExpressionNode(
    	SyntaxToken variableIdentifierToken,
    	SyntaxToken openBraceToken,
    	SyntaxToken closeBraceToken,
    	TypeClauseNode resultTypeClauseNode)
    {
        IdentifierToken = variableIdentifierToken;
    	OpenBraceToken = openBraceToken;
    	CloseBraceToken = closeBraceToken;
    	ResultTypeClauseNode = resultTypeClauseNode;
    }

	private ISyntax[] _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

    public SyntaxToken IdentifierToken { get; }
    public SyntaxToken OpenBraceToken { get; }
    public SyntaxToken CloseBraceToken { get; }
    public TypeClauseNode ResultTypeClauseNode { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.WithExpressionNode;
    
    public ISyntax[] GetChildList()
    {
    	if (!_childListIsDirty)
    		return _childList;
    	
    	_childList = new ISyntax[]
        {
            IdentifierToken,
            OpenBraceToken,
            CloseBraceToken,
            ResultTypeClauseNode,
        };
        
    	_childListIsDirty = false;
    	return _childList;
    }
}
