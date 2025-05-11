using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.ListExtensions;

namespace Luthetus.Common.RazorLib.TreeViews.Models;

public class TreeViewService : ITreeViewService
{
    private readonly BackgroundTaskService _backgroundTaskService;

    public TreeViewService(BackgroundTaskService backgroundTaskService)
    {
        _backgroundTaskService = backgroundTaskService;
    }

    public CommonBackgroundTaskApi CommonBackgroundTaskApi { get; set; }

    private TreeViewState _treeViewState = new();
    
    public event Action? TreeViewStateChanged;
    
    public TreeViewState GetTreeViewState() => _treeViewState;
    
    public TreeViewContainer GetTreeViewContainer(Key<TreeViewContainer> containerKey) =>
    	_treeViewState.ContainerList.FirstOrDefault(x => x.Key == containerKey);

    public bool TryGetTreeViewContainer(Key<TreeViewContainer> containerKey, out TreeViewContainer? container)
    {
        container = GetTreeViewState().ContainerList.FirstOrDefault(
            x => x.Key == containerKey);

        return container is not null;
    }

    public void MoveRight(
		Key<TreeViewContainer> containerKey,
		bool addSelectedNodes,
		bool selectNodesBetweenCurrentAndNextActiveNode)
    {
        ReduceMoveRightAction(
            containerKey,
			addSelectedNodes,
			selectNodesBetweenCurrentAndNextActiveNode,
            treeViewNoType =>
            {
                CommonBackgroundTaskApi.Enqueue_TreeViewService_LoadChildList(
                    containerKey,
                    treeViewNoType);
            });
    }

    public string GetNodeElementId(TreeViewNoType node)
    {
        return $"luth_node-{node.Key}";
    }

    public string GetTreeViewContainerElementId(Key<TreeViewContainer> containerKey)
    {
        return $"luth_tree-container-{containerKey.Guid}";
    }
		
	// Reducer methods
    public void ReduceRegisterContainerAction(TreeViewContainer container)
    {
    	var inState = GetTreeViewState();
    
        var inContainer = inState.ContainerList.FirstOrDefault(
            x => x.Key == container.Key);

        if (inContainer is not null)
        {
            TreeViewStateChanged?.Invoke();
            return;
        }

        var outContainerList = new List<TreeViewContainer>(inState.ContainerList);
        outContainerList.Add(container);
        
        _treeViewState = inState with { ContainerList = outContainerList };
        TreeViewStateChanged?.Invoke();
        return;
    }

    public void ReduceDisposeContainerAction(Key<TreeViewContainer> containerKey)
    {
    	var inState = GetTreeViewState();
    
        var indexContainer = inState.ContainerList.FindIndex(
            x => x.Key == containerKey);

        if (indexContainer == -1)
        {
            TreeViewStateChanged?.Invoke();
        	return;
        }
        
        var outContainerList = new List<TreeViewContainer>(inState.ContainerList);
        outContainerList.RemoveAt(indexContainer);
        
        _treeViewState = inState with { ContainerList = outContainerList };
        TreeViewStateChanged?.Invoke();
        return;
    }

    public void ReduceWithRootNodeAction(Key<TreeViewContainer> containerKey, TreeViewNoType node)
    {
    	var inState = GetTreeViewState();
    
        var indexContainer = inState.ContainerList.FindIndex(
            x => x.Key == containerKey);

        if (indexContainer == -1)
        {
            TreeViewStateChanged?.Invoke();
        	return;
		}
        
        var inContainer = inState.ContainerList[indexContainer];
        
        var outContainer = inContainer with
        {
            RootNode = node,
            SelectedNodeList = new List<TreeViewNoType>() { node }
        };

        var outContainerList = new List<TreeViewContainer>(inState.ContainerList);
        outContainerList[indexContainer] = outContainer;
        
        _treeViewState = inState with { ContainerList = outContainerList };
        TreeViewStateChanged?.Invoke();
        return;
    }

    public void ReduceAddChildNodeAction(Key<TreeViewContainer> containerKey, TreeViewNoType parentNode, TreeViewNoType childNode)
    {
    	var inState = GetTreeViewState();
    
        var inContainer = inState.ContainerList.FirstOrDefault(
            x => x.Key == containerKey);

        if (inContainer is null)
        {
            TreeViewStateChanged?.Invoke();
        	return;
        }

        var parent = parentNode;
        var child = childNode;

        child.Parent = parent;
        child.IndexAmongSiblings = parent.ChildList.Count;
        child.TreeViewChangedKey = Key<TreeViewChanged>.NewKey();

        parent.ChildList.Add(child);

        ReduceReRenderNodeAction(containerKey, parent);
        return;
    }

    public void ReduceReRenderNodeAction(Key<TreeViewContainer> containerKey, TreeViewNoType node)
    {
    	var inState = GetTreeViewState();
    
        var indexContainer = inState.ContainerList.FindIndex(
            x => x.Key == containerKey);

        if (indexContainer == -1)
        {
            TreeViewStateChanged?.Invoke();
        	return;
        }

        var inContainer = inState.ContainerList[indexContainer];
        
        var outContainer = PerformReRenderNode(inContainer, containerKey, node);

        var outContainerList = new List<TreeViewContainer>(inState.ContainerList);
        outContainerList[indexContainer] = outContainer;
        
        _treeViewState = inState with { ContainerList = outContainerList };
        TreeViewStateChanged?.Invoke();
        return;
    }

    public void ReduceSetActiveNodeAction(
    	Key<TreeViewContainer> containerKey,
		TreeViewNoType? nextActiveNode,
		bool addSelectedNodes,
		bool selectNodesBetweenCurrentAndNextActiveNode)
    {
    	var inState = GetTreeViewState();
    
        var indexContainer = inState.ContainerList.FindIndex(
            x => x.Key == containerKey);

        if (indexContainer == -1)
        {
            TreeViewStateChanged?.Invoke();
       	 return;
        }

		var inContainer = inState.ContainerList[indexContainer];
		
		var outContainer = PerformSetActiveNode(
			inContainer, containerKey, nextActiveNode, addSelectedNodes, selectNodesBetweenCurrentAndNextActiveNode);

		var outContainerList = new List<TreeViewContainer>(inState.ContainerList);
		outContainerList[indexContainer] = outContainer;

        _treeViewState = inState with { ContainerList = outContainerList };
        TreeViewStateChanged?.Invoke();
        return;
    }

    public void ReduceRemoveSelectedNodeAction(
    	Key<TreeViewContainer> containerKey,
        Key<TreeViewNoType> keyOfNodeToRemove)
    {
    	var inState = GetTreeViewState();
    
        var indexContainer = inState.ContainerList.FindIndex(
            x => x.Key == containerKey);

        if (indexContainer == -1)
        {
            TreeViewStateChanged?.Invoke();
        	return;
        }

		var inContainer = inState.ContainerList[indexContainer];
		
		var outContainer = PerformRemoveSelectedNode(inContainer, containerKey, keyOfNodeToRemove);

		var outContainerList = new List<TreeViewContainer>(inState.ContainerList);
			
		outContainerList[indexContainer] = outContainer;

        _treeViewState = inState with { ContainerList = outContainerList };
        TreeViewStateChanged?.Invoke();
        return;
    }

    public void ReduceMoveLeftAction(
    	Key<TreeViewContainer> containerKey,
		bool addSelectedNodes,
		bool selectNodesBetweenCurrentAndNextActiveNode)
    {
    	var inState = GetTreeViewState();
    
        var indexContainer = inState.ContainerList.FindIndex(
            x => x.Key == containerKey);

		if (indexContainer == -1)
		{
			TreeViewStateChanged?.Invoke();
        	return;
		}
			
		var inContainer = inState.ContainerList[indexContainer];
        if (inContainer?.ActiveNode is null)
        {
            TreeViewStateChanged?.Invoke();
        	return;
        }

        var outContainer = PerformMoveLeft(inContainer, containerKey, addSelectedNodes, selectNodesBetweenCurrentAndNextActiveNode);

        var outContainerList = new List<TreeViewContainer>(inState.ContainerList);
        outContainerList[indexContainer] = outContainer;

        _treeViewState = inState with { ContainerList = outContainerList };
        TreeViewStateChanged?.Invoke();
        return;
    }

    public void ReduceMoveDownAction(
    	Key<TreeViewContainer> containerKey,
		bool addSelectedNodes,
		bool selectNodesBetweenCurrentAndNextActiveNode)
    {
    	var inState = GetTreeViewState();
    
        var indexContainer = inState.ContainerList.FindIndex(
            x => x.Key == containerKey);

        if (indexContainer == -1)
        {
        	TreeViewStateChanged?.Invoke();
        	return;
        }
        
        var inContainer = inState.ContainerList[indexContainer];
        if (inContainer?.ActiveNode is null)
        {
            TreeViewStateChanged?.Invoke();
        	return;
        }

        var outContainer = PerformMoveDown(inContainer, containerKey, addSelectedNodes, selectNodesBetweenCurrentAndNextActiveNode);

        var outContainerList = new List<TreeViewContainer>(inState.ContainerList);
        outContainerList[indexContainer] = outContainer;
        
        _treeViewState = inState with { ContainerList = outContainerList };
        TreeViewStateChanged?.Invoke();
        return;
    }

    public void ReduceMoveUpAction(
    	Key<TreeViewContainer> containerKey,
		bool addSelectedNodes,
		bool selectNodesBetweenCurrentAndNextActiveNode)
    {
    	var inState = GetTreeViewState();
    
        var indexContainer = inState.ContainerList.FindIndex(
            x => x.Key == containerKey);

		if (indexContainer == -1)
		{
            TreeViewStateChanged?.Invoke();
        	return;
        }

        var inContainer = inState.ContainerList[indexContainer];
        
        var outContainer = PerformMoveUp(inContainer, containerKey, addSelectedNodes, selectNodesBetweenCurrentAndNextActiveNode);

        var outContainerList = new List<TreeViewContainer>(inState.ContainerList);
        outContainerList[indexContainer] = outContainer;
        
        _treeViewState = inState with { ContainerList = outContainerList };
        TreeViewStateChanged?.Invoke();
        return;
    }

    public void ReduceMoveRightAction(
        Key<TreeViewContainer> containerKey,
		bool addSelectedNodes,
		bool selectNodesBetweenCurrentAndNextActiveNode,
		Action<TreeViewNoType> loadChildListAction)
    {
    	var inState = GetTreeViewState();
    
    	var indexContainer = inState.ContainerList.FindIndex(
            x => x.Key == containerKey);
		
		if (indexContainer == -1)
		{
            TreeViewStateChanged?.Invoke();
        	return;
        }
		
        var inContainer = inState.ContainerList[indexContainer];
            
        if (inContainer?.ActiveNode is null)
        {
            TreeViewStateChanged?.Invoke();
        	return;
        }

        var outContainer = PerformMoveRight(inContainer, containerKey, addSelectedNodes, selectNodesBetweenCurrentAndNextActiveNode, loadChildListAction);

		var outContainerList = new List<TreeViewContainer>(inState.ContainerList);
        outContainerList[indexContainer] = outContainer;
        
        _treeViewState = inState with { ContainerList = outContainerList };
        TreeViewStateChanged?.Invoke();
        return;
    }

    public void ReduceMoveHomeAction(
		Key<TreeViewContainer> containerKey,
		bool addSelectedNodes,
		bool selectNodesBetweenCurrentAndNextActiveNode)
    {
    	var inState = GetTreeViewState();
    
    	var indexContainer = inState.ContainerList.FindIndex(
            x => x.Key == containerKey);
            
		if (indexContainer == -1)
		{
            TreeViewStateChanged?.Invoke();
        	return;
        }
            
        var inContainer = inState.ContainerList[indexContainer];
        if (inContainer?.ActiveNode is null)
        {
            TreeViewStateChanged?.Invoke();
        	return;
        }

        var outContainer = PerformMoveHome(inContainer, containerKey, addSelectedNodes, selectNodesBetweenCurrentAndNextActiveNode);

		var outContainerList = new List<TreeViewContainer>(inState.ContainerList);
        outContainerList[indexContainer] = outContainer;
        
        _treeViewState = inState with { ContainerList = outContainerList };
        TreeViewStateChanged?.Invoke();
        return;
    }

    public void ReduceMoveEndAction(
		Key<TreeViewContainer> containerKey,
		bool addSelectedNodes,
		bool selectNodesBetweenCurrentAndNextActiveNode)
    {
    	var inState = GetTreeViewState();
    
    	var indexContainer = inState.ContainerList.FindIndex(
            x => x.Key == containerKey);
		
		if (indexContainer == -1)
		{
            TreeViewStateChanged?.Invoke();
        	return;
        }
        
        var inContainer = inState.ContainerList[indexContainer];
        if (inContainer?.ActiveNode is null)
        {
            TreeViewStateChanged?.Invoke();
        	return;
        }

        var outContainer = PerformMoveEnd(inContainer, containerKey, addSelectedNodes, selectNodesBetweenCurrentAndNextActiveNode);

		var outContainerList = new List<TreeViewContainer>(inState.ContainerList);
        outContainerList[indexContainer] = outContainer;
        
        _treeViewState = inState with { ContainerList = outContainerList };
        TreeViewStateChanged?.Invoke();
        return;
    }

	private void PerformMarkForRerender(TreeViewNoType node)
    {
        var markForRerenderTarget = node;

        while (markForRerenderTarget is not null)
        {
            markForRerenderTarget.TreeViewChangedKey = Key<TreeViewChanged>.NewKey();
            markForRerenderTarget = markForRerenderTarget.Parent;
        }
    }

	private TreeViewContainer PerformReRenderNode(
		TreeViewContainer inContainer,
		Key<TreeViewContainer> containerKey,
		TreeViewNoType node)
	{
		PerformMarkForRerender(node);
        return inContainer with { StateId = Guid.NewGuid() };
	}

	private TreeViewContainer PerformSetActiveNode(
		TreeViewContainer inContainer,
		Key<TreeViewContainer> containerKey,
		TreeViewNoType? nextActiveNode,
		bool addSelectedNodes,
		bool selectNodesBetweenCurrentAndNextActiveNode)
	{
		if (inContainer.ActiveNode is not null)
            PerformMarkForRerender(inContainer.ActiveNode);

        if (nextActiveNode is not null)
            PerformMarkForRerender(nextActiveNode);

		var inSelectedNodeList = inContainer.SelectedNodeList;
		var selectedNodeListWasCleared = false;

		TreeViewContainer outContainer;

		// TODO: I'm adding multi-select. I'd like to single out the...
		// ...SelectNodesBetweenCurrentAndNextActiveNode case for now...
		// ...and DRY the code after. (2024-01-13) 
		if (selectNodesBetweenCurrentAndNextActiveNode)
		{
			outContainer = inContainer;
			int direction;

			// Step 1: Determine the selection's direction.
			//
			// That is to say, on the UI, which node appears
			// vertically closer to the root node.
			//
			// Process: -Discover the closest common ancestor node.
			//          -Then backtrack one node depth.
			//          -One now has the two nodes at which
			//              they differ.
			//	      -Compare the 'TreeViewNoType.IndexAmongSiblings'
			//          -Next.IndexAmongSiblings - Current.IndexAmongSiblings
			//          	if (difference > 0)
			//              	then: direction is towards end
			//          	if (difference < 0)
			//              	then: direction is towards home (AKA root)
			{
				var currentTarget = inContainer.ActiveNode;
				var nextTarget = nextActiveNode;

				while (currentTarget.Parent != nextTarget.Parent)
				{
					if (currentTarget.Parent is null || nextTarget.Parent is null)
						break;

					currentTarget = currentTarget.Parent;
					nextTarget = nextTarget.Parent;
				}
				
				direction = nextTarget.IndexAmongSiblings - currentTarget.IndexAmongSiblings;
			}

			if (direction > 0)
			{
				// Move down

				var previousNode = outContainer.ActiveNode;

				while (true)
				{
					outContainer = PerformMoveDown(
						outContainer,
						containerKey,
						true,
						false);

					if (previousNode.Key == outContainer.ActiveNode.Key)
					{
						// No change occurred, avoid an infinite loop and break
						break;
					}
					else
					{
						previousNode = outContainer.ActiveNode;
					}

					if (nextActiveNode.Key == outContainer.ActiveNode.Key)
					{
						// Target acquired
						break;
					}
				}
			}
			else if (direction < 0)
			{
				// Move up

				var previousNode = outContainer.ActiveNode;

				while (true)
				{
					outContainer = PerformMoveUp(
						outContainer,
						containerKey,
						true,
						false);

					if (previousNode.Key == outContainer.ActiveNode.Key)
					{
						// No change occurred, avoid an infinite loop and break
						break;
					}
					else
					{
						previousNode = outContainer.ActiveNode;
					}

					if (nextActiveNode.Key == outContainer.ActiveNode.Key)
					{
						// Target acquired
						break;
					}
				}
			}
			else
			{
				// The next target is the same as the current target.
				return outContainer;
			}
		}
		else
		{
			if (nextActiveNode is null)
			{
				selectedNodeListWasCleared = true;

				outContainer = inContainer with
	            {
	                SelectedNodeList = Array.Empty<TreeViewNoType>()
	            };
			}
			else if (!addSelectedNodes)
			{
				selectedNodeListWasCleared = true;

				outContainer = inContainer with
	            {
	                SelectedNodeList = new List<TreeViewNoType>()
					{
						nextActiveNode
					}
	            };
			}
			else
			{
				var alreadyExistingIndex = inContainer.SelectedNodeList.FindIndex(
					x => nextActiveNode.Equals(x));
				
				if (alreadyExistingIndex != -1)
				{
					var outSelectedNodeList = new List<TreeViewNoType>(inContainer.SelectedNodeList);
					outSelectedNodeList.RemoveAt(alreadyExistingIndex);
				
					inContainer = inContainer with
		            {
		                SelectedNodeList = outSelectedNodeList
		            };
				}

				// Variable name collision on 'outSelectedNodeLists'.
				{
					var outSelectedNodeList = new List<TreeViewNoType>(inContainer.SelectedNodeList);
					outSelectedNodeList.Insert(0, nextActiveNode);
					
					outContainer = inContainer with
		            {
		                SelectedNodeList = outSelectedNodeList
		            };
		        }
			}
		}
		
		if (selectedNodeListWasCleared)
		{
			foreach (var node in inSelectedNodeList)
			{
				PerformMarkForRerender(node);
			}
		}

        return outContainer;
	}
    
    private TreeViewContainer PerformRemoveSelectedNode(
		TreeViewContainer inContainer,
        Key<TreeViewContainer> containerKey,
        Key<TreeViewNoType> keyOfNodeToRemove)
    {
        var indexOfNodeToRemove = inContainer.SelectedNodeList.FindIndex(
            x => x.Key == keyOfNodeToRemove);

		var outSelectedNodeList = new List<TreeViewNoType>(inContainer.SelectedNodeList);
		outSelectedNodeList.RemoveAt(indexOfNodeToRemove);

        return inContainer with
        {
            SelectedNodeList = outSelectedNodeList
        };
    }
    
    private TreeViewContainer PerformMoveLeft(
		TreeViewContainer inContainer,
		Key<TreeViewContainer> containerKey,
		bool addSelectedNodes,
		bool selectNodesBetweenCurrentAndNextActiveNode)
	{
		var outContainer = inContainer;

		if (addSelectedNodes)
            return outContainer;

        if (outContainer.ActiveNode is null)
            return outContainer;

        if (outContainer.ActiveNode.IsExpanded &&
            outContainer.ActiveNode.IsExpandable)
        {
            outContainer.ActiveNode.IsExpanded = false;
            return PerformReRenderNode(outContainer, outContainer.Key, outContainer.ActiveNode);
        }

        if (outContainer.ActiveNode.Parent is not null)
        {
            outContainer = PerformSetActiveNode(
                outContainer,
                outContainer.Key,
                outContainer.ActiveNode.Parent,
                false,
				false);
        }

		return outContainer;
	}

	private TreeViewContainer PerformMoveDown(
		TreeViewContainer inContainer,
		Key<TreeViewContainer> containerKey,
		bool addSelectedNodes,
		bool selectNodesBetweenCurrentAndNextActiveNode)
	{
		var outContainer = inContainer;

        if (outContainer.ActiveNode.IsExpanded &&
            outContainer.ActiveNode.ChildList.Any())
        {
            var nextActiveNode = outContainer.ActiveNode.ChildList[0];

            outContainer = PerformSetActiveNode(
                outContainer,
                outContainer.Key,
                nextActiveNode,
				addSelectedNodes,
				selectNodesBetweenCurrentAndNextActiveNode);
        }
        else
        {
            var target = outContainer.ActiveNode;

            while (target.Parent is not null &&
                   target.IndexAmongSiblings == target.Parent.ChildList.Count - 1)
            {
                target = target.Parent;
            }

            if (target.Parent is null ||
                target.IndexAmongSiblings == target.Parent.ChildList.Count - 1)
            {
                return outContainer;
            }

            var nextActiveNode = target.Parent.ChildList[
                target.IndexAmongSiblings +
                1];

            outContainer = PerformSetActiveNode(
                outContainer,
                outContainer.Key,
                nextActiveNode,
				addSelectedNodes,
				selectNodesBetweenCurrentAndNextActiveNode);
        }

		return outContainer;
	}

	private TreeViewContainer PerformMoveUp(
		TreeViewContainer inContainer,
		Key<TreeViewContainer> containerKey,
		bool addSelectedNodes,
		bool selectNodesBetweenCurrentAndNextActiveNode)
	{
		var outContainer = inContainer;

		if (outContainer?.ActiveNode?.Parent is null)
            return outContainer;

        if (outContainer.ActiveNode.IndexAmongSiblings == 0)
        {
            outContainer = PerformSetActiveNode(
                outContainer,
                outContainer.Key,
                outContainer.ActiveNode!.Parent,
				addSelectedNodes,
				selectNodesBetweenCurrentAndNextActiveNode);
        }
        else
        {
            var target = outContainer.ActiveNode.Parent.ChildList[
                outContainer.ActiveNode.IndexAmongSiblings - 1];

            while (true)
            {
                if (target.IsExpanded &&
                    target.ChildList.Any())
                {
                    target = target.ChildList.Last();
                }
                else
                {
                    break;
                }
            }

            outContainer = PerformSetActiveNode(
                outContainer,
                outContainer.Key,
                target,
				addSelectedNodes,
				selectNodesBetweenCurrentAndNextActiveNode);
        }

		return outContainer;
	}

	private TreeViewContainer PerformMoveRight(
		TreeViewContainer inContainer,
		Key<TreeViewContainer> containerKey,
		bool addSelectedNodes,
		bool selectNodesBetweenCurrentAndNextActiveNode,
		Action<TreeViewNoType> loadChildListAction)
	{
		var outContainer = inContainer;

		if (outContainer is null || outContainer.ActiveNode is null)
            return outContainer;

        if (addSelectedNodes)
            return outContainer;

        if (outContainer.ActiveNode is null)
            return outContainer;

        if (outContainer.ActiveNode.IsExpanded)
        {
            if (outContainer.ActiveNode.ChildList.Any())
            {
                outContainer = PerformSetActiveNode(
                    outContainer,
                    outContainer.Key,
                    outContainer.ActiveNode.ChildList[0],
					addSelectedNodes,
					selectNodesBetweenCurrentAndNextActiveNode);
            }
        }
        else if (outContainer.ActiveNode.IsExpandable)
        {
            outContainer.ActiveNode.IsExpanded = true;

            loadChildListAction.Invoke(outContainer.ActiveNode);
        }

		return outContainer;
	}

	private TreeViewContainer PerformMoveHome(
		TreeViewContainer inContainer,
		Key<TreeViewContainer> containerKey,
		bool addSelectedNodes,
		bool selectNodesBetweenCurrentAndNextActiveNode)
	{
		var outContainer = inContainer;

        TreeViewNoType target;

        if (outContainer.RootNode is TreeViewAdhoc)
        {
            if (outContainer.RootNode.ChildList.Any())
                target = outContainer.RootNode.ChildList[0];
            else
                target = outContainer.RootNode;
        }
        else
        {
            target = outContainer.RootNode;
        }

        return PerformSetActiveNode(
            outContainer,
            outContainer.Key,
            target,
			addSelectedNodes,
			selectNodesBetweenCurrentAndNextActiveNode);
	}

	private TreeViewContainer PerformMoveEnd(
		TreeViewContainer inContainer,
		Key<TreeViewContainer> containerKey,
		bool addSelectedNodes,
		bool selectNodesBetweenCurrentAndNextActiveNode)
	{
		var outContainer = inContainer;

        var target = outContainer.RootNode;

        while (target.IsExpanded && target.ChildList.Any())
        {
            target = target.ChildList.Last();
        }

        return PerformSetActiveNode(
            outContainer,
            outContainer.Key,
            target,
			addSelectedNodes,
			selectNodesBetweenCurrentAndNextActiveNode);
	}
}