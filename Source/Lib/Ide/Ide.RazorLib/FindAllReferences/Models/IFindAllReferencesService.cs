using Luthetus.Extensions.CompilerServices.Syntax.Nodes;

namespace Luthetus.Ide.RazorLib.FindAllReferences.Models;

public interface IFindAllReferencesService
{
	/// <summary>
	/// Since the IDE is supposed to support any programming language,
	/// I cannot directly write code here that understands what a C# project is.
	///
	/// So I'll try going with this route. It isn't perfect,
	/// since it still presumes the IDE is only working with a single language at a time.
	/// But I think this is a fine stepping stone solution.
	/// </summary>
	public List<(string Name, string Path)> PathGroupList { get; set; }

	public event Action? FindAllReferencesStateChanged;
	
	public FindAllReferencesState GetFindAllReferencesState();
	public void SetFullyQualifiedName(string namespaceName, string syntaxName, TypeDefinitionNode typeDefinitionNode);
}
