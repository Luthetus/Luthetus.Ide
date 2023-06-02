using Luthetus.Ide.ClassLib.CompilerServices.Common.General;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax.SyntaxTokens;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;

public sealed record BoundNamespaceStatementNode : ISyntaxNode
{
    public BoundNamespaceStatementNode(
        KeywordToken keywordToken,
        IdentifierToken identifierToken,
        ImmutableArray<CompilationUnit> children)
    {
        Children = children
            .Select(x => (ISyntax)x)
            .ToImmutableArray();
        KeywordToken = keywordToken;
        IdentifierToken = identifierToken;
    }

    public KeywordToken KeywordToken { get; }
    public IdentifierToken IdentifierToken { get; }

    public ImmutableArray<ISyntax> Children { get; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundNamespaceStatementNode;
}
