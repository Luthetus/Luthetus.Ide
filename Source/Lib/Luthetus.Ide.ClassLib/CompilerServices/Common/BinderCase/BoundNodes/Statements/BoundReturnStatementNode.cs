using Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Expression;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax.SyntaxTokens;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;

public sealed record BoundReturnStatementNode : ISyntaxNode
{
    public BoundReturnStatementNode(
        KeywordToken keywordToken,
        IBoundExpressionNode boundExpressionNode)
    {
        KeywordToken = keywordToken;
        BoundExpressionNode = boundExpressionNode;

        Children = new ISyntax[]
        {
            KeywordToken,
            BoundExpressionNode
        }.ToImmutableArray();
    }

    public KeywordToken KeywordToken { get; init; }
    public IBoundExpressionNode BoundExpressionNode { get; init; }

    public ImmutableArray<ISyntax> Children { get; init; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundReturnStatementNode;
}
