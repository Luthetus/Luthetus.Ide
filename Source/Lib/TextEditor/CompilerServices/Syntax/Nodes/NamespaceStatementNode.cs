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

        SetChildList();
    }

    public KeywordToken KeywordToken { get; }
    public IdentifierToken IdentifierToken { get; }
    public CodeBlockNode? CodeBlockNode { get; private set; }
    public OpenBraceToken? OpenBraceToken { get; private set; }

	public ScopeDirectionKind ScopeDirectionKind => ScopeDirectionKind.Both;

    public ImmutableArray<ISyntax> ChildList { get; private set; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.NamespaceStatementNode;

    /// <summary>
    /// <see cref="GetTopLevelTypeDefinitionNodes"/> provides a collection
    /// which contains all top level type definitions of the <see cref="NamespaceStatementNode"/>.
    /// </summary>
    public ImmutableArray<TypeDefinitionNode> GetTopLevelTypeDefinitionNodes()
    {
    	var localCodeBlockNode = CodeBlockNode;
    
    	if (localCodeBlockNode is null)
    		return ImmutableArray<TypeDefinitionNode>.Empty;
    
        return localCodeBlockNode.ChildList
            .Where(innerC => innerC.SyntaxKind == SyntaxKind.TypeDefinitionNode)
            .Select(td => (TypeDefinitionNode)td)
            .ToImmutableArray();
    }
    
    public TypeClauseNode? GetReturnTypeClauseNode()
    {
    	return null;
    }
    
    public ICodeBlockOwner WithCodeBlockNode(OpenBraceToken openBraceToken, CodeBlockNode codeBlockNode)
    {    
    	OpenBraceToken = openBraceToken;
    	CodeBlockNode = codeBlockNode;
    	return this;
    }
    
    public void SetChildList()
    {
    	var children = new List<ISyntax>
        {
            KeywordToken,
            IdentifierToken,
        };
        
        if (OpenBraceToken is not null)
    		children.Add(OpenBraceToken);
    		
    	if (CodeBlockNode is not null)
    		children.Add(CodeBlockNode);

        ChildList = children.ToImmutableArray();
    }
}
