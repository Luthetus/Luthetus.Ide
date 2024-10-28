using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class VariableDeclarationNode : IVariableDeclarationNode
{
    public VariableDeclarationNode(
        TypeClauseNode typeClauseNode,
        IdentifierToken identifierToken,
        VariableKind variableKind,
        bool isInitialized)
    {
        TypeClauseNode = typeClauseNode;
        IdentifierToken = identifierToken;
        VariableKind = variableKind;
        IsInitialized = isInitialized;
    }

	private ISyntax[] _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

    public TypeClauseNode TypeClauseNode { get; }
    public IdentifierToken IdentifierToken { get; }
    public VariableKind VariableKind { get; }
    public bool IsInitialized { get; set; }
    /// <summary>
    /// TODO: Remove the 'set;' on this property
    /// </summary>
    public bool HasGetter { get; set; }
    /// <summary>
    /// TODO: Remove the 'set;' on this property
    /// </summary>
    public bool GetterIsAutoImplemented { get; set; }
    /// <summary>
    /// TODO: Remove the 'set;' on this property
    /// </summary>
    public bool HasSetter { get; set; }
    /// <summary>
    /// TODO: Remove the 'set;' on this property
    /// </summary>
    public bool SetterIsAutoImplemented { get; set; }

    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.VariableDeclarationNode;
    
    public ISyntax[] GetChildList()
    {
    	if (!_childListIsDirty)
    		return _childList;
    	
    	_childList = new ISyntax[]
        {
            TypeClauseNode,
            IdentifierToken,
        };
        
    	_childListIsDirty = false;
    	return _childList;
    }
}
