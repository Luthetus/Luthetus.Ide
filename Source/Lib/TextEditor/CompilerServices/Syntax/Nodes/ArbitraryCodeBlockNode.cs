using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed record ArbitraryCodeBlockNode : ICodeBlockOwner
{
    public ArbitraryCodeBlockNode(
    	ICodeBlockOwner parentCodeBlockOwner,
    	OpenBraceToken openBraceToken)
    {
        ParentCodeBlockOwner = parentCodeBlockOwner;
        Parent = ParentCodeBlockOwner;
        
        OpenBraceToken = openBraceToken;
        
        var children = new List<ISyntax>
        {
            OpenBraceToken,
        };

        ChildList = children.ToImmutableArray();
    }

    public ICodeBlockOwner ParentCodeBlockOwner { get; }
    public OpenBraceToken OpenBraceToken { get; }
	public CodeBlockNode? CodeBlockNode { get; private set; }

	public ScopeDirectionKind ScopeDirectionKind => ParentCodeBlockOwner.ScopeDirectionKind;

    public ImmutableArray<ISyntax> ChildList { get; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.ArbitraryCodeBlockNode;
    
    public TypeClauseNode? GetReturnTypeClauseNode()
    {
    	return ParentCodeBlockOwner.GetReturnTypeClauseNode();
    }
    
    public ICodeBlockOwner WithCodeBlockNode(CSharpParserModel parserModel, CodeBlockNode codeBlockNode)
    {
    	CodeBlockNode = codeBlockNode;
    	return this;
    }
}
