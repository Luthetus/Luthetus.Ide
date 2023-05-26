using Luthetus.Ide.ClassLib.CompilerServices.Common.General;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;

/// <summary><see cref="BoundNamespaceEntryNode"/> composes <see cref="CompilationUnit"/> to extend the base <see cref="CompilationUnit"/> functionality. The extended functionality that is gained here is the <see cref="ResourceUri"/> so one can track which file was parsed to get the <see cref="CompilationUnit"/></summary>
public class BoundNamespaceEntryNode : ISyntaxNode
{
    public BoundNamespaceEntryNode(
        string resourceUri,
        CompilationUnit compilationUnit)
    {
        ResourceUri = resourceUri;
        CompilationUnit = compilationUnit;
    }

    /// <summary>This might be used to refer to the absolute file path of the file on one's computer which was parsed to make this <see cref="BoundNamespaceEntryNode"/>.</summary>
    public string ResourceUri { get; }
    public CompilationUnit CompilationUnit { get; }

    public ImmutableArray<ISyntax> Children { get; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundNamespaceEntryNode;
}
