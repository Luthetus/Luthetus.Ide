using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Expression;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed record ObjectInitializationParameterEntryNode : ISyntaxNode
{
    public ObjectInitializationParameterEntryNode(
        IdentifierToken propertyIdentifierToken,
        EqualsToken equalsToken,
        IExpressionNode expressionNode)
    {
        PropertyIdentifierToken = propertyIdentifierToken;
        EqualsToken = equalsToken;
        ExpressionNode = expressionNode;

        var children = new List<ISyntax>
        {
            PropertyIdentifierToken,
            ExpressionNode,
        };

        ChildList = children.ToImmutableArray();
    }

    public IdentifierToken PropertyIdentifierToken { get; }
    public EqualsToken EqualsToken { get; }
    public IExpressionNode ExpressionNode { get; }

    public ImmutableArray<ISyntax> ChildList { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.ObjectInitializationParameterEntryNode;
}