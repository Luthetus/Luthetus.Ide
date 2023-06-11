using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;

public sealed record BoundFunctionInvocationNode : ISyntaxNode
{
    public BoundFunctionInvocationNode(
        ISyntaxToken identifierToken,
        BoundFunctionParametersNode boundFunctionParametersNode,
        BoundGenericArgumentsNode? boundGenericArgumentsNode)
    {
        IdentifierToken = identifierToken;
        BoundFunctionParametersNode = boundFunctionParametersNode;
        BoundGenericArgumentsNode = boundGenericArgumentsNode;

        var childrenList = new List<ISyntax>
        {
            IdentifierToken,
            BoundFunctionParametersNode
        };

        if (BoundGenericArgumentsNode is not null)
            childrenList.Add(BoundGenericArgumentsNode);

        Children = childrenList.ToImmutableArray();
    }

    public ISyntaxToken IdentifierToken { get; init; }
    public BoundFunctionParametersNode BoundFunctionParametersNode { get; init; }
    public BoundGenericArgumentsNode? BoundGenericArgumentsNode { get; init; }

    public ImmutableArray<ISyntax> Children { get; init; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundFunctionInvocationNode;
}