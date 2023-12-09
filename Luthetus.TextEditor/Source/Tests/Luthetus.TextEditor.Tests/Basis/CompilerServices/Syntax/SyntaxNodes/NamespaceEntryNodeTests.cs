using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;

/// <summary>
/// TODO: Rewrite 'namespace node' logic (2023-11-27)
/// </summary>
public sealed record NamespaceEntryNodeTests
{
    public NamespaceEntryNode(ResourceUri resourceUri, CodeBlockNode codeBlockNode)
    {
        ResourceUri = resourceUri;
        CodeBlockNode = codeBlockNode;

        ChildBag = (new ISyntax[]
        {
            CodeBlockNode
        }).ToImmutableArray();
    }

    public ResourceUri ResourceUri { get; }
    public CodeBlockNode CodeBlockNode { get; }

    public ImmutableArray<ISyntax> ChildBag { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.NamespaceEntryNode;
}