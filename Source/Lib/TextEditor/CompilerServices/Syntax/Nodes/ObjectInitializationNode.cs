using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

/// <summary>TODO: Correctly implement this node. For now, just skip over it when parsing.</summary>
public sealed class ObjectInitializationNode : ISyntaxNode
{
    public ObjectInitializationNode(OpenBraceToken openBraceToken, CloseBraceToken closeBraceToken)
    {
        OpenBraceToken = openBraceToken;
        CloseBraceToken = closeBraceToken;

        SetChildList();
    }

    public OpenBraceToken OpenBraceToken { get; }
    public CloseBraceToken CloseBraceToken { get; }

    public ISyntax[] ChildList { get; private set; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.ObjectInitializationNode;
    
    public void SetChildList()
    {
    	var childCount = 2; // OpenBraceToken, CloseBraceToken,
            
        var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = OpenBraceToken;
		childList[i++] = CloseBraceToken;
            
        ChildList = childList;
    }
}