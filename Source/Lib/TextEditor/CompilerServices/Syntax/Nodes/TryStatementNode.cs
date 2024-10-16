using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class TryStatementNode : ISyntaxNode
{
    public TryStatementNode(
        TryStatementTryNode? tryNode,
        TryStatementCatchNode? catchNode,
        TryStatementFinallyNode? finallyNode)
    {
        TryNode = tryNode;
        CatchNode = catchNode;
        FinallyNode = finallyNode;

        SetChildList();
    }

    public TryStatementTryNode? TryNode { get; private set; }
    public TryStatementCatchNode? CatchNode { get; private set; }
    public TryStatementFinallyNode? FinallyNode { get; private set; }

	public ScopeDirectionKind ScopeDirectionKind => ScopeDirectionKind.Down;

    public ImmutableArray<ISyntax> ChildList { get; private set; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.TryStatementNode;
    
    public void SetChildList()
    {
    	var childrenList = new List<ISyntax>();

        if (TryNode is not null)
            childrenList.Add(TryNode);
            
        if (CatchNode is not null)
            childrenList.Add(CatchNode);
            
        if (FinallyNode is not null)
            childrenList.Add(FinallyNode);

        ChildList = childrenList.ToImmutableArray();
    }
    
    public void SetTryStatementTryNode(TryStatementTryNode tryStatementTryNode)
    {
    	TryNode = tryStatementTryNode;
    	SetChildList();
    }
    
    public void SetTryStatementCatchNode(TryStatementCatchNode tryStatementCatchNode)
    {
    	CatchNode = tryStatementCatchNode;
    	SetChildList();
    }
    
    public void SetTryStatementFinallyNode(TryStatementFinallyNode tryStatementFinallyNode)
    {
    	FinallyNode = tryStatementFinallyNode;
    	SetChildList();
    }
}
