using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class SwitchExpressionNode : ICodeBlockOwner
{
    public SwitchExpressionNode(
        KeywordToken keywordToken,
        IExpressionNode expressionNode,
        CodeBlockNode? codeBlockNode)
    {
        KeywordToken = keywordToken;
        ExpressionNode = expressionNode;
        CodeBlockNode = codeBlockNode;

        SetChildList();
    }

    public KeywordToken KeywordToken { get; }
    public IExpressionNode ExpressionNode { get; }
    public OpenBraceToken OpenBraceToken { get; private set; }
    public CodeBlockNode? CodeBlockNode { get; private set; }

	public ScopeDirectionKind ScopeDirectionKind => ScopeDirectionKind.Down;

    public ISyntax[] ChildList { get; private set; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.SwitchExpressionNode;
    
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
        var childCount = 2; // KeywordToken, ExpressionNode,
        if (CodeBlockNode is not null)
            childCount++;
            
        var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = KeywordToken;
		childList[i++] = ExpressionNode;
        if (CodeBlockNode is not null)
            childList[i++] = CodeBlockNode;
            
        ChildList = childList;
    }
}
