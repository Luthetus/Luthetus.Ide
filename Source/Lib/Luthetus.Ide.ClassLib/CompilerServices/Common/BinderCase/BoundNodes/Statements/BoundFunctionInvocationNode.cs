using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;

public sealed record BoundFunctionInvocationNode : ISyntaxNode
{
    public BoundFunctionInvocationNode(
        ISyntaxToken identifierToken)
    {
        IdentifierToken = identifierToken;

        Children = new ISyntax[]
        {
            IdentifierToken
        }.ToImmutableArray();
    }

    public ISyntaxToken IdentifierToken { get; }

    public ImmutableArray<ISyntax> Children { get; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundFunctionInvocationNode;
}