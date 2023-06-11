using Luthetus.Ide.ClassLib.CompilerServices.Common.General;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.TextEditor.RazorLib.Lexing;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;

public sealed record BoundNamespaceEntryNode : ISyntaxNode
{
    public BoundNamespaceEntryNode(
        ResourceUri resourceUri,
        CompilationUnit compilationUnit)
    {
        ResourceUri = resourceUri;
        CompilationUnit = compilationUnit;

        Children = new ISyntax[] 
        {
            CompilationUnit
        }.ToImmutableArray();
    }

    public ResourceUri ResourceUri { get; }
    public CompilationUnit CompilationUnit { get; init; }

    public ImmutableArray<ISyntax> Children { get; init; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundNamespaceEntryNode;
}
