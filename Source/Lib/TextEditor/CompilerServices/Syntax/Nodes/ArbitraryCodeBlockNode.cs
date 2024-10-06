using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed record ArbitraryCodeBlockNode : ICodeBlockOwner
{
    public ArbitraryCodeBlockNode(
    	CodeBlockOwner parentCodeBlockOwner)
    {
        ParentCodeBlockOwner = parentCodeBlockOwner;
        Parent = ParentCodeBlockOwner;

        var children = new List<ISyntax>
        {
            ParentCodeBlockOwner,
        };

        ChildList = children.ToImmutableArray();
    }

    public CodeBlockOwner ParentCodeBlockOwner { get; }

	public ScopeDirectionKind ScopeDirectionKind => ParentCodeBlockOwner.ScopeDirectionKind;

    public ImmutableArray<ISyntax> ChildList { get; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.ArbitraryCodeBlockNode;
    
    public TypeClauseNode? GetReturnTypeClauseNode()
    {
    	return ParentCodeBlockOwner.GetReturnTypeClauseNode();
    }
}
