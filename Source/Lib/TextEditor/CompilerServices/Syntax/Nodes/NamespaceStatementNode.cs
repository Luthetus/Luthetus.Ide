using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class NamespaceStatementNode : ICodeBlockOwner
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
    public OpenBraceToken OpenBraceToken { get; private set; }
    public CodeBlockNode? CodeBlockNode { get; private set; }

	public ScopeDirectionKind ScopeDirectionKind => ScopeDirectionKind.Both;

    public ISyntax[] ChildList { get; private set; }
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
    
    public ICodeBlockOwner SetCodeBlockNode(OpenBraceToken openBraceToken, CodeBlockNode codeBlockNode)
    {    
    	OpenBraceToken = openBraceToken;
    	CodeBlockNode = codeBlockNode;
    	SetChildList();
    	return this;
    }
    
    public void OnBoundScopeCreatedAndSetAsCurrent(IParserModel parserModel)
    {
        var namespaceString = IdentifierToken.TextSpan.GetText();
        parserModel.Binder.AddNamespaceToCurrentScope(namespaceString, parserModel);
    }
    
    public ICodeBlockOwner SetFileScoped(CodeBlockNode codeBlockNode)
    {    
    	CodeBlockNode = codeBlockNode;
    	SetChildList();
    	return this;
    }
    
    public void SetChildList()
    {
    	var childCount = 2; // KeywordToken, IdentifierToken,
        if (OpenBraceToken.ConstructorWasInvoked)
    		childCount++;
    	if (CodeBlockNode is not null)
    		childCount++;
            
        var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = KeywordToken;
		childList[i++] = IdentifierToken;
		if (OpenBraceToken.ConstructorWasInvoked)
    		childList[i++] = OpenBraceToken;
    	if (CodeBlockNode is not null)
    		childList[i++] = CodeBlockNode;
            
        ChildList = childList;
    }
}
