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

    public ISyntaxNode? Parent { get; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.LambdaExpressionNode;
    
    public LambdaExpressionNode SetVariableDeclarationNodeListFromBadExpressionNode(BadExpressionNode badExpressionNode, IParserModel model)
    {
    	Console.Write("SetVariableDeclarationNodeList_" + badExpressionNode.SyntaxKind + '_');
    	
    	Console.Write(badExpressionNode.SyntaxList.Count);
    	
    	if (badExpressionNode.SyntaxList.Count > 1)
    		Console.Write('+' + badExpressionNode.SyntaxList[1].SyntaxKind);
    	
    	if (badExpressionNode.SyntaxList.Count == 2 &&
    		badExpressionNode.SyntaxList[1].SyntaxKind == SyntaxKind.IdentifierToken)
    	{
    		Console.Write("inside");
    		var identifierToken = (IdentifierToken)badExpressionNode.SyntaxList[1];
    		
    		var variableDeclarationNode = model.HandleVariableDeclarationExpression(
		        TypeFacts.Empty.ToTypeClause(),
		        identifierToken,
		        VariableKind.Local,
		        model);
		        
    		VariableDeclarationNodeList.Add(variableDeclarationNode);
    	}
    	else
    	{
    		Console.Write("not");
    	}
    
    	_childListIsDirty = true;
    	return this;
    }
    
    public LambdaExpressionNode SetVariableDeclarationNodeList(IExpressionNode expressionNode, IParserModel model)
    {
    	if (expressionNode.SyntaxKind == SyntaxKind.BadExpressionNode)
    		return SetVariableDeclarationNodeListFromBadExpressionNode((BadExpressionNode)expressionNode, model);
    		
    	Console.Write("SetVariableDeclarationNodeList_" + expressionNode.SyntaxKind + '_');
    
    	if (expressionNode.SyntaxKind == SyntaxKind.AmbiguousIdentifierExpressionNode)
    	{
    		var token = ((AmbiguousIdentifierExpressionNode)expressionNode).Token;
    		
    		if (token.SyntaxKind != SyntaxKind.IdentifierToken)
    			return this;
    	
    		var variableDeclarationNode = model.HandleVariableDeclarationExpression(
		        TypeFacts.Empty.ToTypeClause(),
		        (IdentifierToken)token,
		        VariableKind.Local,
		        model);
		        
    		VariableDeclarationNodeList.Add(variableDeclarationNode);
    	}
    
    	_childListIsDirty = true;
    	return this;
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
