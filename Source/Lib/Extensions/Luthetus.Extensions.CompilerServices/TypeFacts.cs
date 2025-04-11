using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes;
using Luthetus.Extensions.CompilerServices.Syntax;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Enums;

namespace Luthetus.Extensions.CompilerServices;

public static class TypeFacts
{
	/// <summary>
	/// If a <see cref="ISyntaxNode"/> has a <see cref="TypeClauseNode"/>,
	/// but is constructed during parsing process, prior to having found the
	/// <see cref="TypeClauseNode"/>, then this will be used as the
	/// <see cref="TypeClauseNode"/> for the time being until the actual is parsed.
	/// </summary>
	public static readonly TypeDefinitionNode Empty = new(
		AccessModifierKind.Public,
		false,
		StorageModifierKind.Class,
		new SyntaxToken(SyntaxKind.IdentifierToken, new TextEditorTextSpan(0, "empty".Length, (byte)GenericDecorationKind.None, ResourceUri.Empty, "empty")),
		typeof(void),
		default,
		primaryConstructorFunctionArgumentListing: default,
		null,
		string.Empty,
		referenceHashSet: new());

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
	/// will have the <see cref="IExpressionNode.ResultTypeClauseNode"/> of <see cref="Pseudo"/>
	/// in order to indicate that the node is part of a expression, but not itself an expression.
	///
	/// Use 'TypeClauseNode GenericParameterEntryNode.ResultTypeClauseNode => TypeClauseNodeFacts.Pseudo;'
	/// so that some confusion can be avoided since one has to cast it explicitly as an IExpressionNode
	/// in order to access the property. (i.e.: <see cref="GenericParameterEntryNode.TypeClauseNode"/>
	/// is not equal to GenericParameterEntryNode.ResultTypeClauseNode).
	/// </summary>
	public static readonly TypeDefinitionNode Pseudo = new(
		AccessModifierKind.Public,
		false,
		StorageModifierKind.Class,
		new SyntaxToken(SyntaxKind.IdentifierToken, new TextEditorTextSpan(0, "empty".Length, (byte)GenericDecorationKind.None, ResourceUri.Empty, "empty")),
		typeof(void),
		default,
		primaryConstructorFunctionArgumentListing: default,
		null,
		string.Empty,
		referenceHashSet: new());
}
