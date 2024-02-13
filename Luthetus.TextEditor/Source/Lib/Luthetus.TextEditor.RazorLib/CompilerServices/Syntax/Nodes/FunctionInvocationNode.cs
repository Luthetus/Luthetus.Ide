using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Expression;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed record FunctionInvocationNode : IExpressionNode
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
    }

    public IdentifierToken FunctionInvocationIdentifierToken { get; }
    public FunctionDefinitionNode? FunctionDefinitionNode { get; }
    public GenericParametersListingNode? GenericParametersListingNode { get; }
    public FunctionParametersListingNode FunctionParametersListingNode { get; }
    public TypeClauseNode ResultTypeClauseNode { get; }

    public ImmutableArray<ISyntax> ChildList { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.FunctionInvocationNode;

}