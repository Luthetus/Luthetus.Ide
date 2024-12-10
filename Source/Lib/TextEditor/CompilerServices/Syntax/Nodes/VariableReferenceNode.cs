using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class VariableReferenceNode : IExpressionNode
{
    public VariableReferenceNode(
        INameToken nameToken,
        IVariableDeclarationNode variableDeclarationNode)
    {
        NameToken = nameToken;
        VariableDeclarationNode = variableDeclarationNode;
    }

	private ISyntax[] _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

    public INameToken NameToken { get; }
    /// <summary>
    /// The <see cref="VariableDeclarationNode"/> is null when the variable is undeclared
    /// </summary>
    public IVariableDeclarationNode VariableDeclarationNode { get; }
    public TypeClauseNode ResultTypeClauseNode => VariableDeclarationNode.TypeClauseNode;

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.VariableReferenceNode;
    
    public ISyntax[] GetChildList()
    {
    	if (!_childListIsDirty)
    		return _childList;
    	
    	_childList = new ISyntax[]
        {
            NameToken,
            VariableDeclarationNode,
        };
        
    	_childListIsDirty = false;
    	return _childList;
    }
}
