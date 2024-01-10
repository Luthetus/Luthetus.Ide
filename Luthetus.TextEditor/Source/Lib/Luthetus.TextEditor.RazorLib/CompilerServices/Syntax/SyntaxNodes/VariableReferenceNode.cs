using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Expression;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;

public sealed record VariableReferenceNode : IExpressionNode
{
    public VariableReferenceNode(
        IdentifierToken variableIdentifierToken,
        VariableDeclarationNode variableDeclarationNode)
    {
        VariableIdentifierToken = variableIdentifierToken;
        VariableDeclarationNode = variableDeclarationNode;

        ChildList = new ISyntax[]
        {
            VariableIdentifierToken,
            VariableDeclarationNode,
        }.ToImmutableArray();
    }

    public IdentifierToken VariableIdentifierToken { get; }
    /// <summary>
    /// The <see cref="VariableDeclarationNode"/> is null when the variable is undeclared
    /// </summary>
    public VariableDeclarationNode VariableDeclarationNode { get; }
    public TypeClauseNode ResultTypeClauseNode => VariableDeclarationNode.TypeClauseNode;

    public ImmutableArray<ISyntax> ChildList { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.VariableReferenceNode;
}
