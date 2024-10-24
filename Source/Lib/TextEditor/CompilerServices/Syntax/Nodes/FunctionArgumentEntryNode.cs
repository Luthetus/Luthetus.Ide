using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

/// <summary>
/// Used when defining a function.
/// </summary>
public sealed class FunctionArgumentEntryNode : ISyntaxNode
{
    public FunctionArgumentEntryNode(
        VariableDeclarationNode variableDeclarationNode,
        ISyntaxToken? optionalCompileTimeConstantToken,
        bool isOptional,
        bool hasParamsKeyword,
        bool hasOutKeyword,
        bool hasInKeyword,
        bool hasRefKeyword)
    {
        VariableDeclarationNode = variableDeclarationNode;
        OptionalCompileTimeConstantToken = optionalCompileTimeConstantToken;
        IsOptional = isOptional;
        HasParamsKeyword = hasParamsKeyword;
        HasOutKeyword = hasOutKeyword;
        HasInKeyword = hasInKeyword;
        HasRefKeyword = hasRefKeyword;
    }

	private ISyntax[] _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

    public VariableDeclarationNode VariableDeclarationNode { get; }
    public ISyntaxToken? OptionalCompileTimeConstantToken { get; }
    public bool IsOptional { get; }
    public bool HasParamsKeyword { get; }
    public bool HasOutKeyword { get; }
    public bool HasInKeyword { get; }
    public bool HasRefKeyword { get; }

    public ISyntax[] ChildList { get; private set; }
    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.FunctionArgumentEntryNode;
    
    public ISyntax[] GetChildList()
    {
    	if (!_childListIsDirty)
    		_childListIsDirty;
    	
    	var childCount = 1; // VariableDeclarationNode,
        if (OptionalCompileTimeConstantToken is not null)
            childCount++;
            
        var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = VariableDeclarationNode;
        if (OptionalCompileTimeConstantToken is not null)
            childList[i++] = OptionalCompileTimeConstantToken;
            
        _childList = childList;
        
    	_childListIsDirty = false;
    	return _childList;
    }
}