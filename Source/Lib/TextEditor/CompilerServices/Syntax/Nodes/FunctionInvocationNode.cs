using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class FunctionInvocationNode : IExpressionNode
{
    public FunctionInvocationNode(
        IdentifierToken functionInvocationIdentifierToken,
        FunctionDefinitionNode? functionDefinitionNode,
        GenericParametersListingNode? genericParametersListingNode,
        FunctionParametersListingNode functionParametersListingNode,
        TypeClauseNode resultTypeClauseNode)
    {
        FunctionInvocationIdentifierToken = functionInvocationIdentifierToken;
        FunctionDefinitionNode = functionDefinitionNode;
        GenericParametersListingNode = genericParametersListingNode;
        FunctionParametersListingNode = functionParametersListingNode;
        ResultTypeClauseNode = resultTypeClauseNode;
        
        SetChildList();
    }

    public IdentifierToken FunctionInvocationIdentifierToken { get; }
    public FunctionDefinitionNode? FunctionDefinitionNode { get; }
    public GenericParametersListingNode? GenericParametersListingNode { get; }
    public FunctionParametersListingNode FunctionParametersListingNode { get; }
    public TypeClauseNode ResultTypeClauseNode { get; }

    public ISyntax[] ChildList { get; private set; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.FunctionInvocationNode;

    public void SetChildList()
    {
    	var children = new List<ISyntax>
        {
            FunctionInvocationIdentifierToken
        };

        if (FunctionDefinitionNode is not null)
            children.Add(FunctionDefinitionNode);

        if (GenericParametersListingNode is not null)
            children.Add(GenericParametersListingNode);

        children.Add(FunctionParametersListingNode);

        children.Add(ResultTypeClauseNode);

        ChildList = children.ToImmutableArray();
    	throw new NotImplementedException();
    }
}