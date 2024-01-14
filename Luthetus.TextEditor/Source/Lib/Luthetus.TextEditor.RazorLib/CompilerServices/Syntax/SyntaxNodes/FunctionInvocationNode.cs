using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;

public sealed record FunctionInvocationNode : ISyntaxNode
{
    public FunctionInvocationNode(
        IdentifierToken functionInvocationIdentifierToken,
        FunctionDefinitionNode? functionDefinitionNode,
        GenericParametersListingNode? genericParametersListingNode,
        FunctionParametersListingNode functionParametersListingNode)
    {
        FunctionInvocationIdentifierToken = functionInvocationIdentifierToken;
        FunctionDefinitionNode = functionDefinitionNode;
        GenericParametersListingNode = genericParametersListingNode;
        FunctionParametersListingNode = functionParametersListingNode;

        var children = new List<ISyntax>
        {
            FunctionInvocationIdentifierToken
        };

        if (FunctionDefinitionNode is not null)
            children.Add(FunctionDefinitionNode);

        if (GenericParametersListingNode is not null)
            children.Add(GenericParametersListingNode);

        children.Add(FunctionParametersListingNode);

        ChildList = children.ToImmutableArray();
    }

    public IdentifierToken FunctionInvocationIdentifierToken { get; }
    public FunctionDefinitionNode? FunctionDefinitionNode { get; }
    public GenericParametersListingNode? GenericParametersListingNode { get; }
    public FunctionParametersListingNode FunctionParametersListingNode { get; }

    public ImmutableArray<ISyntax> ChildList { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.FunctionInvocationNode;
}