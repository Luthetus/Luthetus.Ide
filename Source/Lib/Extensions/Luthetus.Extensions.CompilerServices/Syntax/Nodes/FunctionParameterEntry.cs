using Luthetus.Extensions.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

/// <summary>
/// Used when invoking a function.
/// </summary>
public struct FunctionParameterEntry
{
	public FunctionParameterEntry(
		IExpressionNode expressionNode,
		bool hasOutKeyword,
		bool hasInKeyword,
		bool hasRefKeyword)
	{
		ExpressionNode = expressionNode;
		HasOutKeyword = hasOutKeyword;
		HasInKeyword = hasInKeyword;
		HasRefKeyword = hasRefKeyword;
	}

	public IExpressionNode ExpressionNode { get; set; }
	public bool HasOutKeyword { get; }
	public bool HasInKeyword { get; }
	public bool HasRefKeyword { get; }
}
