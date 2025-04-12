namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

public interface IExpressionNode : ISyntaxNode
{
	/// <summary>
	/// When parsing an expression, there may be a <see cref="FunctionInvocationNode"/>
	/// with a <see cref="GenericParametersListingNode"/>.
	///
	/// The expression code works however by passing around an 'IExpression expressionPrimary'
	/// and an 'IExpression expressionSeconday'.
	///
	/// This means we cannot add common logic for parsing a <see cref="GenericParametersListingNode"/>
	/// unless the expression parsing code were modified, or maybe a call to a separate
	/// function to parse <see cref="GenericParametersListingNode"/> could be done.
	///
	/// But, for now an experiment with making <see cref="GenericParametersListingNode"/>
	/// into a <see cref="IExpressionNode"/> is being tried out.
	///
	/// Any <see cref="IExpressionNode"/> that is part of a complete expression,
	/// will have the <see cref="ResultTypeClauseNode"/> of <see cref="Pseudo"/>
	/// in order to indicate that the node is part of a expression, but not itself an expression.
	///
	/// Use 'TypeClauseNode IExpressionNode.ResultTypeClauseNode => TypeClauseNodeFacts.Pseudo;'
	/// so that some confusion can be avoided since one has to cast it explicitly as an IExpressionNode
	/// in order to access the property. (i.e.: <see cref="GenericParameterEntryNode.TypeClauseNode"/>
	/// is not equal to IExpressionNode.ResultTypeClauseNode).
	/// </summary>
	public TypeReference ResultTypeReference { get; }
}
