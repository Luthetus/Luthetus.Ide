using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class ObjectInitializationParameterEntryNode : ISyntaxNode
{
    public ObjectInitializationParameterEntryNode(
        IdentifierToken propertyIdentifierToken,
        EqualsToken equalsToken,
        IExpressionNode expressionNode)
    {
        PropertyIdentifierToken = propertyIdentifierToken;
        EqualsToken = equalsToken;
        ExpressionNode = expressionNode;
        
        SetChildList();
    }

    public IdentifierToken PropertyIdentifierToken { get; set; }
    public EqualsToken EqualsToken { get; set; }
    public IExpressionNode ExpressionNode { get; set; }

    public ISyntax[] ChildList { get; private set; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.ObjectInitializationParameterEntryNode;
    
    public void SetChildList()
    {
    	var childCount = 2; // PropertyIdentifierToken, ExpressionNode,
            
        var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = PropertyIdentifierToken;
		childList[i++] = ExpressionNode;
            
        ChildList = childList;
    }
}