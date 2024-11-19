using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Enums;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

/// <summary>
/// While parsing expression, it is necessary that there exists a node
/// that indicates a lambda expression is being parsed.
///
/// This type might be more along the lines of a "builder" type.
/// It is meant to be made when starting lambda expression,
/// then the primary expression can be equal to an instae of this type.
///
/// This then directs the parser accordingly until the lambda expression
/// is fully parsed.
///
/// At this point, it is planned that a FunctionDefinitionNode will be
/// made, and a 'MethodGroupExpressionNode' (this type does not yet exist) will be returned as the
/// primary expression.
/// </summary>
public sealed class LambdaExpressionNode : IExpressionNode
{
    public LambdaExpressionNode(TypeClauseNode resultTypeClauseNode)
    {
    	ResultTypeClauseNode = resultTypeClauseNode;
    }

	private ISyntax[] _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

    public TypeClauseNode ResultTypeClauseNode { get; }
    
    /// <summary>
    /// () => "Abc";
    ///     Then this property is true;
    ///
    /// () => { return "Abc" };
    ///     Then this property is false;
    /// </summary>
    public bool CodeBlockNodeIsExpression { get; set; } = true;
    public bool HasReadParameters { get; set; }
    public List<IVariableDeclarationNode> VariableDeclarationNodeList { get; } = new();

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.LambdaExpressionNode;
    
    public void AddVariableDeclarationNode(IVariableDeclarationNode variableDeclarationNode)
    {
    	VariableDeclarationNodeList.Add(variableDeclarationNode);
    	_childListIsDirty = true;
    }
    
    public int GetStartInclusiveIndex()
    {
    }
    
    public int GetEndExclusiveIndex()
    {
    }
    
    public ISyntax[] GetChildList()
    {
    	if (!_childListIsDirty)
    		return _childList;
    	
    	_childList = new ISyntax[]
        {
            ResultTypeClauseNode
        };
        
    	_childListIsDirty = false;
    	return _childList;
    }
}
