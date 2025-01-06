using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class AmbiguousParenthesizedExpressionNode : IExpressionNode
{
    public AmbiguousParenthesizedExpressionNode(
        OpenParenthesisToken openParenthesisToken,
        IExpressionNode innerExpression,
        CloseParenthesisToken closeParenthesisToken)
    {
        OpenParenthesisToken = openParenthesisToken;
        InnerExpression = innerExpression;
        CloseParenthesisToken = closeParenthesisToken;
    }
    
    public AmbiguousParenthesizedExpressionNode(OpenParenthesisToken openParenthesisToken, TypeClauseNode typeClauseNode)
    	: this(openParenthesisToken, new EmptyExpressionNode(typeClauseNode), default)
    {
    }
    
    private ISyntax[] _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

    public OpenParenthesisToken OpenParenthesisToken { get; }
    public IExpressionNode InnerExpression { get; private set; }
    public CloseParenthesisToken CloseParenthesisToken { get; private set; }
    public TypeClauseNode ResultTypeClauseNode => InnerExpression.ResultTypeClauseNode;
    
    /// <summary>
    /// This class is a "builder" class of sorts.
    /// The node does not actually make sense in the finalized compilation unit.
    /// This class is used to collect syntax that can apply to various
    /// nodes that have 'parenthesized' syntax.
    ///
    /// LambdaExpressionNode:
    ///     (x, y) => 2;
    ///     (int x, bool y) => 2;
    /// 
    /// TupleExpressionNode:
    ///     (int, bool) myVariable;
    ///     (int counter, bool isAvailable) myVariable;
    ///
    /// -----------------------------------------------
    ///
    /// If we ignore the syntax that follows the CloseParenthesisToken
    /// we can isolate the common cases between the two nodes,
    /// and generally hold their information in this type until disambiguation.
    ///
    ///     (x, y)
    ///     (int, bool)
    ///     (int x, bool y)
    ///     (int counter, bool isAvailable)
    ///
    /// -----------------------------------
    ///
    /// I re-ordered the cases by their similarity.
    /// It is important to note that:
    ///     '(x, y)' and '(int, bool)'
    ///     are not equivalent cases.
    ///
    /// '(x, y)' was from the LambdaExpressionNode,
    /// '(int, bool)' was from the TupleExpressionNode.
    ///
    /// '(x, y)' takes both 'x' and 'y' to be the names for a VariableDeclarationNode.
    /// '(int, bool)' takes both 'int' and 'bool' to be TypeClauseNode(s).
    ///
    /// Thus, the solution is to hold comma deliminated "nameable tokens" /
    /// "convertible to IdentifierToken tokens" as just the ISyntaxToken itself until disambiguation.
    ///
    /// i.e.: List<ISyntaxToken> _nameableTokenList;
    ///
    /// How would one differentiate between an explicit cast node, and an AmbiguousParenthesizedExpressionNode.
    /// ````var x = 2;
    /// ````return (double)x;
    ///
    /// The differentiation is because there is only a single nameable token, and there
    /// are no CommaToken(s).
    ///
    /// -------------------------------------------------------------------------------
    ///
    /// Now the remaining cases are:
    ///     (int x, bool y)
    ///     (int counter, bool isAvailable)
    ///
    /// This is comma separated list of VariableDeclarationNode(s).
    ///
    /// i.e.: List<IVariableDeclarationNode> _variableDeclarationNodeList;
    ///
    /// Both '_nameableTokenList' and '_variableDeclarationNodeList'
    /// should be nullable, in order to only allocate
    /// one or the other, based on what appears before the first CommaToken.
    ///
    /// --------------------------------------------------------------------
    ///
    /// What about a ParenthesizedExpressionNode node,
    /// where the inner expression is a VariableReferenceNode.
    ///
    /// One would have to parse this as an 'AmbiguousParenthesizedExpression'
    /// until it is ruled out that it cannot be a LambdaExpressionNode.
    /// </summary>
    public List<VariableDeclarationNode> VariableDeclarationNodeList { get; set; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.ParenthesizedExpressionNode;
    
    public ParenthesizedExpressionNode SetCloseParenthesisToken(CloseParenthesisToken closeParenthesisToken)
    {
    	CloseParenthesisToken = closeParenthesisToken;
    	
    	_childListIsDirty = true;
    	return this;
    }
    
    public ParenthesizedExpressionNode SetInnerExpression(IExpressionNode innerExpression)
    {
    	InnerExpression = innerExpression;
    	
    	_childListIsDirty = true;
    	return this;
    }
    
    public ISyntax[] GetChildList()
    {
    	if (!_childListIsDirty)
    		return _childList;
    	
    	var childCount = 4; // OpenParenthesisToken, InnerExpression, CloseParenthesisToken, ResultTypeClauseNode,
            
        var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = OpenParenthesisToken;
		childList[i++] = InnerExpression;
		childList[i++] = CloseParenthesisToken;
		childList[i++] = ResultTypeClauseNode;
            
        _childList = childList;
        
    	_childListIsDirty = false;
    	return _childList;
    }
}

