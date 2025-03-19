namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

public interface IFunctionDefinitionNode : IExpressionNode
{
	public FunctionArgumentListing FunctionArgumentListing { get; }
	
	public void SetFunctionArgumentListing(FunctionArgumentListing functionArgumentListing);
}
