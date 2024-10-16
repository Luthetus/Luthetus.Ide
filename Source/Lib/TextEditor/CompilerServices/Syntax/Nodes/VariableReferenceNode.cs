using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class VariableReferenceNode : IExpressionNode
{
    public VariableReferenceNode(
        IdentifierToken variableIdentifierToken,
        IVariableDeclarationNode variableDeclarationNode)
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
    public IVariableDeclarationNode VariableDeclarationNode { get; }
    public TypeClauseNode ResultTypeClauseNode => VariableDeclarationNode.TypeClauseNode;

    public ImmutableArray<ISyntax> ChildList { get; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.VariableReferenceNode;
}
