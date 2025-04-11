using Luthetus.Extensions.CompilerServices.Syntax.Nodes;

namespace Luthetus.Ide.RazorLib.FindAllReferences.Models;

public class FindAllReferencesService : IFindAllReferencesService
{
	private readonly object _stateModificationLock = new();

	private FindAllReferencesState _findAllReferencesState = new();

	public event Action? FindAllReferencesStateChanged;
	
	public FindAllReferencesState GetFindAllReferencesState() => _findAllReferencesState;
	
	public void SetFullyQualifiedName(string namespaceName, string syntaxName, TypeDefinitionNode typeDefinitionNode)
	{
		lock (_stateModificationLock)
        {
    	    var inState = GetFindAllReferencesState();
            
            _findAllReferencesState = inState with 
            {
            	NamespaceName = namespaceName,
            	SyntaxName = syntaxName,
            	TypeDefinitionNode = typeDefinitionNode,
            };
        }

        FindAllReferencesStateChanged?.Invoke();
	}
}
