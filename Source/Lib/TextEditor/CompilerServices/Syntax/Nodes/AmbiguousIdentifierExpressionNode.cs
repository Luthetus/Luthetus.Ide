using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class AmbiguousIdentifierExpressionNode : IExpressionNode
{
    public AmbiguousIdentifierExpressionNode(
        ISyntaxToken token,
        GenericParametersListingNode? genericParametersListingNode,
        TypeClauseNode resultTypeClauseNode)
    {
        Token = token;
        GenericParametersListingNode = genericParametersListingNode;
        ResultTypeClauseNode = resultTypeClauseNode;

        SetChildList();
    }

    public ISyntaxToken Token { get; }
    public GenericParametersListingNode? GenericParametersListingNode { get; private set; }
    public TypeClauseNode ResultTypeClauseNode { get; }

    public ISyntax[] ChildList { get; private set; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.AmbiguousIdentifierExpressionNode;
    
    public AmbiguousIdentifierExpressionNode SetGenericParametersListingNode(GenericParametersListingNode? genericParametersListingNode)
    {
    	GenericParametersListingNode = genericParametersListingNode;
    	
    	SetChildList();
    	return this;
    }
    
    public void SetChildList()
    {
    	// TODO: This method.
    	ChildList = Array.Empty<ISyntax>();
    }
}

