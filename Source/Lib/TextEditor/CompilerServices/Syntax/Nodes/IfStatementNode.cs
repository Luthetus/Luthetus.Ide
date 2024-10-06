using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed record IfStatementNode : ICodeBlockOwner
{
    public IfStatementNode(
        KeywordToken keywordToken,
        IExpressionNode expressionNode,
        CodeBlockNode? codeBlockNode)
    {
        KeywordToken = keywordToken;
        ExpressionNode = expressionNode;
        CodeBlockNode = codeBlockNode;

        var childrenList = new List<ISyntax>
        {
            KeywordToken,
            ExpressionNode,
        };

        if (CodeBlockNode is not null)
            childrenList.Add(CodeBlockNode);

        ChildList = childrenList.ToImmutableArray();
    }

    public KeywordToken KeywordToken { get; }
    public IExpressionNode ExpressionNode { get; }
    public CodeBlockNode? CodeBlockNode { get; private set; }

	public ScopeDirectionKind ScopeDirectionKind => ScopeDirectionKind.Down;

    public ImmutableArray<ISyntax> ChildList { get; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.IfStatementNode;
    
    public TypeClauseNode? GetReturnTypeClauseNode()
    {
    	return null;
    }
    
    public ICodeBlockOwner WithCodeBlockNode(CSharpParserModel parserModel, CodeBlockNode codeBlockNode)
    {
    	CodeBlockNode = codeBlockNode;
    	return this;
    }
}