using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes;

namespace Luthetus.Ide.RazorLib.FindAllReferences.Models;

public class FindAllReferencesService : IFindAllReferencesService
{
	private readonly ITreeViewService _treeViewService;

	public FindAllReferencesService(ITreeViewService treeViewService)
	{
		_treeViewService = treeViewService;
	}

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
            
            // The following is way too much code for a lock that might've
            // been entered by the UI thread.
            
            var treeViewNodeList = typeDefinitionNode.ReferenceHashSet.Select(x =>
	    		(TreeViewNoType)new TreeViewFindAllReferences(
	    			x,
					false,
					false))
				.ToArray();
				
			var adhocRoot = TreeViewAdhoc.ConstructTreeViewAdhoc(treeViewNodeList);
	        var firstNode = treeViewNodeList.FirstOrDefault();
	
	        var activeNodes = firstNode is null
	            ? new List<TreeViewNoType>()
	            : new() { firstNode };
	
	        if (!_treeViewService.TryGetTreeViewContainer(FindAllReferencesState.TreeViewContainerKey, out _))
	        {
	            _treeViewService.ReduceRegisterContainerAction(new TreeViewContainer(
	                FindAllReferencesState.TreeViewContainerKey,
	                adhocRoot,
	                activeNodes));
	        }
	        else
	        {
	            _treeViewService.ReduceWithRootNodeAction(FindAllReferencesState.TreeViewContainerKey, adhocRoot);
	
	            _treeViewService.ReduceSetActiveNodeAction(
	                FindAllReferencesState.TreeViewContainerKey,
	                firstNode,
	                true,
	                false);
	        }
        }

        FindAllReferencesStateChanged?.Invoke();
	}
}
