using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed record NamespaceStatementNode : ICodeBlockOwner
{
    public NamespaceStatementNode(
        KeywordToken keywordToken,
        IdentifierToken identifierToken,
        CodeBlockNode codeBlockNode)
    {
        KeywordToken = keywordToken;
        IdentifierToken = identifierToken;
        CodeBlockNode = codeBlockNode;

        var children = new List<ISyntax>
        {
            KeywordToken,
            IdentifierToken,
            CodeBlockNode,
        };

        ChildList = children.ToImmutableArray();
    }

    public KeywordToken KeywordToken { get; }
    public IdentifierToken IdentifierToken { get; }
    public CodeBlockNode CodeBlockNode { get; }

	public ScopeDirectionKind ScopeDirectionKind => ScopeDirectionKind.Both;

    public ImmutableArray<ISyntax> ChildList { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.NamespaceStatementNode;

    /// <summary>
    /// <see cref="GetTopLevelTypeDefinitionNodes"/> provides a collection
    /// which contains all top level type definitions of the <see cref="NamespaceStatementNode"/>.
    /// </summary>
    public ImmutableArray<TypeDefinitionNode> GetTopLevelTypeDefinitionNodes()
    {
        return CodeBlockNode.ChildList
            .Where(innerC => innerC.SyntaxKind == SyntaxKind.TypeDefinitionNode)
            .Select(td => (TypeDefinitionNode)td)
            .ToImmutableArray();
    }
}
