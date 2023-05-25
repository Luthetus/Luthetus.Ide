using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax.SyntaxTokens;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;

public class BoundNamespaceStatementNode : ISyntaxNode
{
    public BoundNamespaceStatementNode(
        KeywordToken keywordToken)
    {
        KeywordToken = keywordToken;

        Children = new ISyntax[]
        {
            KeywordToken,
        }.ToImmutableArray();
    }

    public KeywordToken KeywordToken { get; }

    public ImmutableArray<ISyntax> Children { get; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundNamespaceStatementNode;
}
