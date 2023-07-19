using Luthetus.Ide.ClassLib.CompilerServices.Common.General;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.TextEditor.RazorLib.Lexing;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.BinderCase.BoundNodes.Statements;

public sealed record BoundNamespaceEntryNode : ISyntaxNode
{
    public BoundNamespaceEntryNode(
        ResourceUri resourceUri,
        CodeBlockNode codeBlockNode)
    {
        ResourceUri = resourceUri;
        CodeBlockNode = codeBlockNode;

        Children = (new ISyntax[] 
        {
            CodeBlockNode
        }).ToImmutableArray();
    }

    public ResourceUri ResourceUri { get; }
    public CodeBlockNode CodeBlockNode { get; init; }

    public ImmutableArray<ISyntax> Children { get; init; }
    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.BoundNamespaceEntryNode;
}
