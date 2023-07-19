using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Expression;
using Luthetus.Ide.ClassLib.CompilerServices.Common.General;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax.SyntaxTokens;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;

public sealed record BoundIfStatementNode : ISyntaxNode
{
    public BoundIfStatementNode(
        KeywordToken keywordToken,
        IBoundExpressionNode boundExpressionNode,
        CodeBlockNode? ifStatementBodyCodeBlockNode)
    {
        KeywordToken = keywordToken;
        BoundExpressionNode = boundExpressionNode;
        IfStatementBodyCodeBlockNode = ifStatementBodyCodeBlockNode;

        var childrenList = new List<ISyntax>
        {
            KeywordToken,
            BoundExpressionNode,
        };

        if (IfStatementBodyCodeBlockNode is not null)
            childrenList.Add(IfStatementBodyCodeBlockNode);

        Children = childrenList.ToImmutableArray();
    }

    public KeywordToken KeywordToken { get; init; }
    public IBoundExpressionNode BoundExpressionNode { get; init; }
    public CodeBlockNode? IfStatementBodyCodeBlockNode { get; init; }

    public ImmutableArray<ISyntax> Children { get; init; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundIfStatementNode;
}
