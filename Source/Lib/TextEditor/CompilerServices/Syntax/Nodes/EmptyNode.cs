using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

/// <summary>
/// At times, this node is used in place of 'null'.
/// </summary>
public sealed class EmptyNode : ISyntaxNode
{
    public EmptyNode()
    {
    	SetChildList();
    }

    public ISyntax[] ChildList { get; private set; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.EmptyNode;
    
    public void SetChildList()
    {
    	ChildList = ImmutableArray<ISyntax>.Empty;
    	throw new NotImplementedException();
    }
}
