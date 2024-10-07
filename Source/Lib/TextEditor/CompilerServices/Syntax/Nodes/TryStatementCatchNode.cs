namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed record TryStatementCatchNode : ICodeBlockOwner
{
	public TryStatementCatchNode(
        KeywordToken keywordToken,
        CodeBlockNode? codeBlockNode,
        OpenParenthesisToken? openParenthesisToken,
        CloseParenthesisToken? closeParenthesisToken,
        CodeBlockNode? codeBlockNode)
    {
        KeywordToken = keywordToken;
        CodeBlockNode = codeBlockNode;
        OpenParenthesisToken = openParenthesisToken;
        CloseParenthesisToken = closeParenthesisToken;
        CodeBlockNode = codeBlockNode;

        SetChildList();
    }
	
	public KeywordToken KeywordToken { get; }
    public CodeBlockNode? CodeBlockNode { get; }
    public OpenParenthesisToken? OpenParenthesisToken { get; }
    public CloseParenthesisToken? CloseParenthesisToken { get; }
    public OpenBraceToken? OpenBraceToken { get; }
    public CodeBlockNode? CodeBlockNode { get; }

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
