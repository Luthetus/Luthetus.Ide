using Luthetus.Extensions.CompilerServices.Syntax.Nodes;

namespace Luthetus.Ide.RazorLib.FindAllReferences.Models;

public interface IFindAllReferencesService
{
	public event Action? FindAllReferencesStateChanged;
	
	public FindAllReferencesState GetFindAllReferencesState();
	public void SetFullyQualifiedName(string namespaceName, string syntaxName, TypeDefinitionNode typeDefinitionNode);
}
