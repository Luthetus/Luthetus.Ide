using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class ObjectInitializationParameterEntryNode : ISyntaxNode
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
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.ObjectInitializationParameterEntryNode;
}