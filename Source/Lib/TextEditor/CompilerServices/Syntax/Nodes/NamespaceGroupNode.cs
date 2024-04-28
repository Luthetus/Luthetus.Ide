using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed record NamespaceGroupNode : ISyntaxNode
{
    public NamespaceGroupNode(
        string namespaceString,
        ImmutableArray<NamespaceStatementNode> namespaceStatementNodeList)
    {
        NamespaceString = namespaceString;
        NamespaceStatementNodeList = namespaceStatementNodeList;

        ChildList = namespaceStatementNodeList.Select(x => (ISyntax)x).ToImmutableArray();
    }

    public string NamespaceString { get; }
    public ImmutableArray<NamespaceStatementNode> NamespaceStatementNodeList { get; }

    public ImmutableArray<ISyntax> ChildList { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.NamespaceGroupNode;

    /// <summary>
    /// <see cref="GetTopLevelTypeDefinitionNodes"/> provides a collection
    /// which contains all top level type definitions of the namespace.
    /// <br/><br/>
    /// This is to say that, any type definitions which are nested, would not
    /// be in this collection.
    /// </summary>
    public ImmutableArray<TypeDefinitionNode> GetTopLevelTypeDefinitionNodes()
    {
        return NamespaceStatementNodeList
            .SelectMany(x => x.GetTopLevelTypeDefinitionNodes())
            .ToImmutableArray();
    }
}