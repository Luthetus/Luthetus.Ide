using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class PreprocessorLibraryReferenceStatementNode : IStatementNode
{
    public PreprocessorLibraryReferenceStatementNode(
        ISyntaxToken includeDirectiveSyntaxToken,
        ISyntaxToken libraryReferenceSyntaxToken)
    {
        IncludeDirectiveSyntaxToken = includeDirectiveSyntaxToken;
        LibraryReferenceSyntaxToken = libraryReferenceSyntaxToken;

        ChildList = new ISyntax[]
        {
            IncludeDirectiveSyntaxToken,
            LibraryReferenceSyntaxToken,
        }.ToImmutableArray();
    }

    public ISyntaxToken IncludeDirectiveSyntaxToken { get; }
    public ISyntaxToken LibraryReferenceSyntaxToken { get; }

    public ImmutableArray<ISyntax> ChildList { get; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.PreprocessorLibraryReferenceStatementNode;
}