using Luthetus.Extensions.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

public sealed class AmbiguousParenthesizedExpressionNode : IExpressionNode
{
	public AmbiguousParenthesizedExpressionNode(
		SyntaxToken openParenthesisToken,
		bool isParserContextKindForceStatementExpression)
	{
		#if DEBUG
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.AmbiguousParenthesizedExpressionNode++;
		#endif
	
		OpenParenthesisToken = openParenthesisToken;
		IsParserContextKindForceStatementExpression = isParserContextKindForceStatementExpression;
	}

	/// <summary>
	/// The expression parsing code will "merge" an instance of this type
	/// with whatever 'ISyntaxToken' is next.
	///
	/// But, the first time this "merge" occurs there is some
	/// disambiguation that needs to be done.
	///
	/// Not all of the disambiguation can be done here,
	/// because if it is a ParenthesizedExpressionNode,
	/// this would mean writing code to handle function invocation
	/// within the AmbiguousParenthesizedExpressionNode itself.
	///
	/// We only want to look ahead just enough to determine if this is a
	/// - ParenthesizedExpressionNode
	/// - or should we continue as an AmbiguousParenthesizedExpressionNode
	///
	/// One needs to be able to handle a tuple "literal" not just
	/// a TypeClauseNode which is a tuple.
	///
	/// tuple "literal":
	/// ````(3, "abc")
	/// 
	/// tuple TypeClauseNode:
	/// ````(int, string)
	///
	/// This is something I skipped over in my head when first thinking about this.
	/// I currently am transforming the AmbiguousParenthesizedExpressionNode
	/// to a ParenthesizedExpressionNode if I see a non-nameable-token as
	/// the first entry in the comma separate list.
	///
	/// But, with this current way, I'd have to once again
	/// go from ParenthesizedExpressionNode to AmbiguousParenthesizedExpressionNode
	/// if it turns out there is an un-accounted for comma amongst the inner expression of the ParenthesizedExpressionNode.
	///
	/// This also opens up the idea of maybe having a 'tuple' type that can be used as a TypeClauseNode,
	/// and having a separate type that can be used for a 'tuple literal'?
	/// </summary>
	public bool IsFirstLoop { get; set; } = true;

	public SyntaxToken OpenParenthesisToken { get; }
	public bool IsParserContextKindForceStatementExpression { get; }
	public TypeReference ResultTypeReference => TypeFacts.Pseudo.ToTypeReference();

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

	public List<ISyntaxNode> NodeList { get; set; } = new();
	public bool? ShouldMatchVariableDeclarationNodes = null;

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.AmbiguousParenthesizedExpressionNode;

	#if DEBUG	
	~AmbiguousParenthesizedExpressionNode()
	{
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.AmbiguousParenthesizedExpressionNode--;
	}
	#endif
}

