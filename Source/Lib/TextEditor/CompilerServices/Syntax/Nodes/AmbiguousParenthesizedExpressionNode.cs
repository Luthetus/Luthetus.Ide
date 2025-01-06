using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;

public sealed class AmbiguousParenthesizedExpressionNode : IExpressionNode
{
    public AmbiguousParenthesizedExpressionNode(OpenParenthesisToken openParenthesisToken)
    {
        OpenParenthesisToken = openParenthesisToken;
    }
    
    private ISyntax[] _childList = Array.Empty<ISyntax>();
	private bool _childListIsDirty = true;

    public OpenParenthesisToken OpenParenthesisToken { get; }
    public TypeClauseNode ResultTypeClauseNode => TypeFacts.Pseudo.ToTypeClause();
    
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
    public List<VariableDeclarationNode>? VariableDeclarationNodeList { get; set; }
    public List<ISyntaxToken>? NameableTokenList { get; set; }

    public bool IsFabricated { get; init; }
    public SyntaxKind SyntaxKind => SyntaxKind.AmbiguousParenthesizedExpressionNode;
    
    public ISyntax[] GetChildList()
    {
    	if (!_childListIsDirty)
    		return _childList;
    	
    	var childCount = 2; // OpenParenthesisToken, ResultTypeClauseNode,
            
        var childList = new ISyntax[childCount];
		var i = 0;

		childList[i++] = OpenParenthesisToken;
		childList[i++] = ResultTypeClauseNode;
            
        _childList = childList;
        
    	_childListIsDirty = false;
    	return _childList;
    }
}

