using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed record TryStatementCatchNode : ICodeBlockOwner
{
	public TryStatementCatchNode(
		TryStatementNode? parent,
        KeywordToken keywordToken,
        OpenParenthesisToken? openParenthesisToken,
        CloseParenthesisToken? closeParenthesisToken,
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
    public OpenParenthesisToken? OpenParenthesisToken { get; }
    public CloseParenthesisToken? CloseParenthesisToken { get; }
    public OpenBraceToken? OpenBraceToken { get; private set; }
    public CodeBlockNode? CodeBlockNode { get; private set; }

	public ScopeDirectionKind ScopeDirectionKind => ScopeDirectionKind.Down;

    public ImmutableArray<ISyntax> ChildList { get; private set; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.TryStatementCatchNode;
    
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
    	var childrenList = new List<ISyntax>();

        if (KeywordToken is not null)
            childrenList.Add(KeywordToken);
            
        if (CodeBlockNode is not null)
            childrenList.Add(CodeBlockNode);
            
        if (OpenParenthesisToken is not null)
            childrenList.Add(OpenParenthesisToken);
            
        if (CloseParenthesisToken is not null)
            childrenList.Add(CloseParenthesisToken);
            
        if (CodeBlockNode is not null)
            childrenList.Add(CodeBlockNode);

        ChildList = childrenList.ToImmutableArray();
    }
}
