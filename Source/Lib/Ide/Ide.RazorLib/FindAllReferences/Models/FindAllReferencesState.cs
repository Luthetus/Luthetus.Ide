using Luthetus.Extensions.CompilerServices.Syntax.Nodes;

namespace Luthetus.Ide.RazorLib.FindAllReferences.Models;

public record struct FindAllReferencesState
{
	public FindAllReferencesState()
	{
	}

	public TypeDefinitionNode? TypeDefinitionNode { get; init; }
	public string? NamespaceName { get; init; }
	public string? SyntaxName { get; init; }
}
