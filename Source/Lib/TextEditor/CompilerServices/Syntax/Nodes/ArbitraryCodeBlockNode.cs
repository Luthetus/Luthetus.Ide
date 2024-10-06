using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed record ArbitraryCodeBlockNode : ICodeBlockOwner
{
    public ArbitraryCodeBlockNode(ICodeBlockOwner parentCodeBlockOwner)
    {
        ParentCodeBlockOwner = parentCodeBlockOwner;
        Parent = ParentCodeBlockOwner;
        
        var children = new List<ISyntax>
        {
            OpenBraceToken,
        };

        ChildList = children.ToImmutableArray();
    }

    public ICodeBlockOwner ParentCodeBlockOwner { get; }
    public OpenBraceToken OpenBraceToken { get; private set; }
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
    
    public ICodeBlockOwner WithCodeBlockNode(OpenBraceToken openBraceToken, CodeBlockNode codeBlockNode)
    {
    	OpenBraceToken = openBraceToken;
    	CodeBlockNode = codeBlockNode;
    	return this;
    }
}
