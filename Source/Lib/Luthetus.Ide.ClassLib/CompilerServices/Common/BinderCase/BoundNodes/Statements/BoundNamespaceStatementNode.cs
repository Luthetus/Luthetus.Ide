using Luthetus.Ide.ClassLib.CompilerServices.Common.General;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax.SyntaxTokens;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;

/// <summary>
/// Many files in C# can have the same namespace.
/// Therefore, my thought process is that the <see cref="BoundNamespaceNode"/>
/// will be a CompilationUnit in a sense.
/// <br/><br/>
/// Then, the BoundNamespace will have a <see cref="BoundNamespaceEntryNode"/>
/// for each C# file which has a namespace declaration with a matching
/// namespace name. Be it a block namespace or file namespace declaration.
/// <br/><br/>
/// I feel as though I'm making everything a compilation unit so that is
/// weird.
/// <br/><br/>
/// I'd furthermore treat each <see cref="BoundNamespaceEntryNode"/> as a
/// CompilationUnit.
/// <br/><br/>
/// A scope could be created by iterating over all the using statements
/// and looking up into a dictionary the <see cref="BoundNamespaceNode"/>
/// and then creating one large compilation unit which is the aggregation
/// of all SyntaxNodes in each respective namespace.
/// </br><br/>
/// In short I don't know, but going to continue on trying to figure it out.
/// <br/><br/>
/// TODO: Delete this comment
/// </summary>
public class BoundNamespaceStatementNode : ISyntaxNode
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
