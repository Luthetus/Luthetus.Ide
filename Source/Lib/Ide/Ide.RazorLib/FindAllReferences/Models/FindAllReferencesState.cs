/*
// FindAllReferences
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes;

namespace Luthetus.Ide.RazorLib.FindAllReferences.Models;

public record struct FindAllReferencesState
{
	public static Key<TreeViewContainer> TreeViewContainerKey { get; } = Key<TreeViewContainer>.NewKey();

	public FindAllReferencesState()
	{
	}

	public TypeDefinitionNode? TypeDefinitionNode { get; init; }
	public string? NamespaceName { get; init; }
	public string? SyntaxName { get; init; }
}
*/