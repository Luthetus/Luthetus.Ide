namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

public interface IGenericParameterNode : IExpressionNode
{
	public GenericParameterListing GenericParameterListing { get; set; }
	public bool IsParsingGenericParameters { get; set; }
}
