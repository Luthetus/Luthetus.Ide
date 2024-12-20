using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class WithExpressionNode : IExpressionNode
{
    public WithExpressionNode(
    	IdentifierToken variableIdentifierToken,
    	OpenBraceToken openBraceToken,
    	CloseBraceToken closeBraceToken,
    	TypeClauseNode resultTypeClauseNode)
    {
        IdentifierToken = variableIdentifierToken;
    	OpenBraceToken = openBraceToken;
    	CloseBraceToken = closeBraceToken;
    	ResultTypeClauseNode = resultTypeClauseNode;
    }

	private ISyntax[] _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

    public IdentifierToken IdentifierToken { get; }
    public OpenBraceToken OpenBraceToken { get; }
    public CloseBraceToken CloseBraceToken { get; }
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
