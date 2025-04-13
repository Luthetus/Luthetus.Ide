using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

/// <summary>
/// If the parser cannot parse an expression, then replace
/// the expression result with an instance of this type.
///
/// The <see cref="SyntaxList"/> contains the 'primaryExpression'
/// that was attempted to have merge with the 'token' or 'secondaryExpression'.
/// As well it contains either the 'token' or the 'secondaryExpression'.
/// And then as well any other syntax that was parsed up until
/// the expression ended.
/// </summary>
public sealed class BadExpressionNode : IExpressionNode
{
	public BadExpressionNode(TypeReference resultTypeReference, ISyntax syntaxPrimary, ISyntax syntaxSecondary)
	{
		#if DEBUG
		Luthetus.Common.RazorLib.Installations.Models.LuthetusDebugSomething.BadExpressionNode++;
		#endif
	
		ResultTypeReference = resultTypeReference;
		SyntaxPrimary = syntaxPrimary;
		SyntaxSecondary = syntaxSecondary;
	}
	
	public ISyntax SyntaxPrimary { get; }
	public ISyntax SyntaxSecondary { get; }

	/// <summary>
	/// This type tracks the cause of the BadExpressionNode in the form of 'SyntaxPrimary', and 'SyntaxSecondary'.
	///
	/// But, once a 'BadExpressionNode' is made, it might go on to clobber the expression loop
	/// until the end of file is reached.
	///
	/// So, this ClobberCount is the amount of times this 'BadExpressionNode' was merged with some other syntax,
	/// and in the process resulted in this 'BadExpressionNode' being the primaryExpression.
	///
	/// (this doesn't count the initial failure to merge 'SyntaxPrimary', and 'SyntaxSecondary').
	/// </summary>
	public int ClobberCount { get; set; }

	public TypeReference ResultTypeReference { get; }

	public bool IsFabricated { get; init; }
	public SyntaxKind SyntaxKind => SyntaxKind.BadExpressionNode;
}