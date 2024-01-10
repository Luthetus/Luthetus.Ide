using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxNodes;

public sealed record NamespaceStatementNode : ISyntaxNode
{
    public NamespaceStatementNode(
        KeywordToken keywordToken,
        IdentifierToken identifierToken,
        ImmutableArray<NamespaceEntryNode> namespaceEntryNodes)
    {
        KeywordToken = keywordToken;
        IdentifierToken = identifierToken;
        NamespaceEntryNodeList = namespaceEntryNodes;

        var children = new List<ISyntax>
        {
            keywordToken,
            identifierToken,
        };

        children.AddRange(namespaceEntryNodes);

        ChildList = children.ToImmutableArray();
    }

    public KeywordToken KeywordToken { get; }
    public IdentifierToken IdentifierToken { get; }
    public ImmutableArray<NamespaceEntryNode> NamespaceEntryNodeList { get; }

    public ImmutableArray<ISyntax> ChildList { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.NamespaceStatementNode;

    /// <summary>
    /// <see cref="GetTopLevelTypeDefinitionNodes"/> provides a collection
    /// which contains all top level type definitions of the namespace.
    /// <br/><br/>
    /// This is to say that, any type definitions which are nested, would not
    /// be in this collection.
    /// </summary>
    public ImmutableArray<TypeDefinitionNode> GetTopLevelTypeDefinitionNodes()
    {
        return NamespaceEntryNodeList
            .SelectMany(bne => bne.CodeBlockNode.ChildList)
            .Where(innerC => innerC.SyntaxKind == SyntaxKind.TypeDefinitionNode)
            .Select(td => (TypeDefinitionNode)td)
            .ToImmutableArray();
    }
}