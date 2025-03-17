using Luthetus.Extensions.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;

namespace Luthetus.Extensions.CompilerServices.Syntax.Nodes;

/// <summary>
/// Used when invoking a syntax which contains a generic type.
/// </summary>
public struct GenericParameterEntry
{
	public GenericParameterEntry(TypeClauseNode typeClauseNode)
	{
		TypeClauseNode = typeClauseNode;
	}

	public TypeClauseNode TypeClauseNode { get; }
	
	public bool ConstructorWasInvoked => TypeClauseNode is not null;
}