﻿using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax.SyntaxNodes.Statement;

public sealed record PreprocessorLibraryReferenceStatement : IStatementNode
{
    public PreprocessorLibraryReferenceStatement(
        ISyntaxToken includeDirectiveSyntaxToken,
        ISyntaxToken libraryReferenceSyntaxToken)
    {
        IncludeDirectiveSyntaxToken = includeDirectiveSyntaxToken;
        LibraryReferenceSyntaxToken = libraryReferenceSyntaxToken;

        Children = new ISyntax[]
        {
            IncludeDirectiveSyntaxToken,
            LibraryReferenceSyntaxToken,
        }.ToImmutableArray();
    }

    public ISyntaxToken IncludeDirectiveSyntaxToken { get; init; }
    public ISyntaxToken LibraryReferenceSyntaxToken { get; init; }

    public ImmutableArray<ISyntax> Children { get; init; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.PreprocessorLibraryReferenceStatementNode;
}