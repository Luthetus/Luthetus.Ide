namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

public interface IInvocationNode : IExpressionNode
{
	public FunctionParameterListing FunctionParameterListing { get; }
	public bool IsParsingFunctionParameters { get; set; }
}
