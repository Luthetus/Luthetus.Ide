using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Expression;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

public sealed record FunctionParameterEntryNodeTests
{
    public FunctionParameterEntryNode(
        IExpressionNode expressionNode,
        bool hasOutKeyword,
        bool hasInKeyword,
        bool hasRefKeyword)
    {
        ExpressionNode = expressionNode;
        HasOutKeyword = hasOutKeyword;
        HasInKeyword = hasInKeyword;
        HasRefKeyword = hasRefKeyword;

        var children = new List<ISyntax>
        {
            ExpressionNode
        };

        ChildBag = children.ToImmutableArray();
    }

    public IExpressionNode ExpressionNode { get; }
    public bool HasOutKeyword { get; }
    public bool HasInKeyword { get; }
    public bool HasRefKeyword { get; }

    public ImmutableArray<ISyntax> ChildBag { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.FunctionParameterEntryNode;
}