using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Statement;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed record PreprocessorLibraryReferenceStatementNode : IStatementNode
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

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.PreprocessorLibraryReferenceStatementNode;
}