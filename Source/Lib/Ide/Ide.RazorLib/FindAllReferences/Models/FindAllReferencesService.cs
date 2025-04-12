using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.TreeViews.Models.Utils;
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
	
	public List<(string Name, string Path)> PathGroupList { get; set; }
	
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
			
			TreeViewAdhoc adhocRoot;
			TreeViewNoType firstNode;
			List<TreeViewNoType> activeNodes;
			
			if (PathGroupList.Count > 0)
			{
				var groupedTreeViewNodeMap = new Dictionary<string, (string Path, List<TreeViewNoType> List)>();
				
				foreach (var pathGroup in PathGroupList)
				{
					Console.WriteLine(pathGroup.Path);
				
					groupedTreeViewNodeMap.Add(pathGroup.Name, (pathGroup.Path, new()));
				}
				
				var miscellaneousGroupName = "Miscellaneous";
				
				groupedTreeViewNodeMap.Add("Miscellaneous", (string.Empty, new()));
			
				foreach (var treeViewNode in treeViewNodeList)
				{
					var foundMatch = false;
					
					foreach (var pathGroup in PathGroupList)
					{
						if (((TreeViewFindAllReferences)treeViewNode).Item.Value.StartsWith(pathGroup.Path))
						{
							groupedTreeViewNodeMap[pathGroup.Name].List.Add(treeViewNode);
							foundMatch = true;
						}
					}
					
					if (!foundMatch)
						groupedTreeViewNodeMap[miscellaneousGroupName].List.Add(treeViewNode);
				}
				
				var groupedTreeViewNodeList = groupedTreeViewNodeMap.Select(x =>
				{
					var treeViewGroup = new TreeViewGroup(
						displayText: x.Key,
						isExpandable: true,
						isExpanded: false)
					{
						TitleText = x.Value.Path,
					};
						
					treeViewGroup.ChildList = x.Value.List;
					return treeViewGroup;
				}).ToArray();
				
				adhocRoot = TreeViewAdhoc.ConstructTreeViewAdhoc(groupedTreeViewNodeList);
		        firstNode = groupedTreeViewNodeList.FirstOrDefault();
		
		        activeNodes = firstNode is null
		            ? new List<TreeViewNoType>()
		            : new() { firstNode };
			}
			else
			{
				adhocRoot = TreeViewAdhoc.ConstructTreeViewAdhoc(treeViewNodeList);
		        firstNode = treeViewNodeList.FirstOrDefault();
		
		        activeNodes = firstNode is null
		            ? new List<TreeViewNoType>()
		            : new() { firstNode };
			}
			
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
