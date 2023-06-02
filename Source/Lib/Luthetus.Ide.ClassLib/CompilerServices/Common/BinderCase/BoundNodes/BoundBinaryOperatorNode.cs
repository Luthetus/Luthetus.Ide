using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes;

public sealed record BoundBinaryOperatorNode : ISyntaxNode
{
    public BoundBinaryOperatorNode(
        Type leftOperandType,
        ISyntaxToken operatorToken,
        Type rightOperandType,
        Type resultType)
    {
        LeftOperandType = leftOperandType;
        OperatorToken = operatorToken;
        RightOperandType = rightOperandType;
        ResultType = resultType;

        Children = new ISyntax[]
        {
            OperatorToken
        }
        .ToImmutableArray();
    }

    public Type LeftOperandType { get; init; }
    public ISyntaxToken OperatorToken { get; init; }
    public Type RightOperandType { get; init; }
    public Type ResultType { get; init; }

    public ImmutableArray<ISyntax> Children { get; init; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundBinaryOperatorNode;
}
