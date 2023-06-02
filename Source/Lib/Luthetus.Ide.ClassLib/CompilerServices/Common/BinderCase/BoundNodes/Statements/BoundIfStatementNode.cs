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
        IBoundExpressionNode boundExpressionNode)
    {
        KeywordToken = keywordToken;
        BoundExpressionNode = boundExpressionNode;

        Children = new ISyntax[]
        {
            KeywordToken,
            BoundExpressionNode,
        }.ToImmutableArray();
    }

    public BoundIfStatementNode(
        KeywordToken keywordToken,
        IBoundExpressionNode boundExpressionNode,
        CompilationUnit ifStatementBodyCompilationUnit)
    {
        KeywordToken = keywordToken;
        BoundExpressionNode = boundExpressionNode;
        IfStatementBodyCompilationUnit = ifStatementBodyCompilationUnit;

        Children = new ISyntax[]
        {
            KeywordToken,
            BoundExpressionNode,
            IfStatementBodyCompilationUnit,
        }.ToImmutableArray();
    }

    public KeywordToken KeywordToken { get; }
    public IBoundExpressionNode BoundExpressionNode { get; }
    public CompilationUnit? IfStatementBodyCompilationUnit { get; }

    public ImmutableArray<ISyntax> Children { get; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundIfStatementNode;

    public BoundIfStatementNode WithIfStatementBody(
        CompilationUnit ifStatementBodyCompilationUnit)
    {
        return new BoundIfStatementNode(
            KeywordToken,
            BoundExpressionNode,
            ifStatementBodyCompilationUnit);
    }
}
