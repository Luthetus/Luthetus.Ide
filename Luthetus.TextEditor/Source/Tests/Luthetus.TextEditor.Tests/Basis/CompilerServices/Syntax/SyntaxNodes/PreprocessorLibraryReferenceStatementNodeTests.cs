using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes.Statement;

namespace Luthetus.TextEditor.Tests.Basis.CompilerServices.Syntax.SyntaxNodes;

public sealed record PreprocessorLibraryReferenceStatementNodeTests
{
    public PreprocessorLibraryReferenceStatementNode(
        ISyntaxToken includeDirectiveSyntaxToken,
        ISyntaxToken libraryReferenceSyntaxToken)
    {
        IncludeDirectiveSyntaxToken = includeDirectiveSyntaxToken;
        LibraryReferenceSyntaxToken = libraryReferenceSyntaxToken;

        ChildBag = new ISyntax[]
        {
            IncludeDirectiveSyntaxToken,
            LibraryReferenceSyntaxToken,
        }.ToImmutableArray();
    }

    public ISyntaxToken IncludeDirectiveSyntaxToken { get; }
    public ISyntaxToken LibraryReferenceSyntaxToken { get; }

    public ImmutableArray<ISyntax> ChildBag { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.PreprocessorLibraryReferenceStatementNode;
}