namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

public interface IFunctionDefinitionNode : IExpressionNode
{
	/// <summary>
	/// TODO: does this having a setter bug TypeDefinitionNode '_memberList'.
	/// </summary>
	public FunctionArgumentListing FunctionArgumentListing { get; set; }
}
