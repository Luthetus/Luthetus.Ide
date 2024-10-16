using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class TryStatementCatchNode : ICodeBlockOwner
{
	public TryStatementCatchNode(
		TryStatementNode? parent,
        KeywordToken keywordToken,
        OpenParenthesisToken openParenthesisToken,
        CloseParenthesisToken closeParenthesisToken,
        CodeBlockNode? codeBlockNode)
    {
    	Parent = parent;
        KeywordToken = keywordToken;
        CodeBlockNode = codeBlockNode;
        OpenParenthesisToken = openParenthesisToken;
        CloseParenthesisToken = closeParenthesisToken;
        CodeBlockNode = codeBlockNode;

        SetChildList();
    }
	
	public KeywordToken KeywordToken { get; }
    public OpenParenthesisToken OpenParenthesisToken { get; }
    public CloseParenthesisToken CloseParenthesisToken { get; }
    public OpenBraceToken OpenBraceToken { get; private set; }
    public CodeBlockNode? CodeBlockNode { get; private set; }

	public ScopeDirectionKind ScopeDirectionKind => ScopeDirectionKind.Down;

    public ISyntax[] ChildList { get; private set; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.TryStatementCatchNode;
    
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
    	// Do nothing.
    	return;
    }
    
    public void SetChildList()
    {
        var childCount = 0;
        if (KeywordToken.ConstructorWasInvoked)
            childCount++;
        if (CodeBlockNode is not null)
            childCount++;
        if (OpenParenthesisToken.ConstructorWasInvoked)
            childCount++;
        if (CloseParenthesisToken.ConstructorWasInvoked)
            childCount++;
        if (CodeBlockNode is not null)
            childCount++;
            
        var childList = new ISyntax[childCount];
		var i = 0;
		
		if (KeywordToken.ConstructorWasInvoked)
            childList[i++] = KeywordToken;
        if (CodeBlockNode is not null)
            childList[i++] = CodeBlockNode;
        if (OpenParenthesisToken.ConstructorWasInvoked)
            childList[i++] = OpenParenthesisToken;
        if (CloseParenthesisToken.ConstructorWasInvoked)
            childList[i++] = CloseParenthesisToken;
        if (CodeBlockNode is not null)
            childList[i++] = CodeBlockNode;
            
        ChildList = childList;
    }
}
