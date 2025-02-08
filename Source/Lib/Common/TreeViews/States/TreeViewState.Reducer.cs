using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;

namespace Luthetus.Common.RazorLib.TreeViews.States;

public partial record TreeViewState
{
    public class Reducer
    {
        [ReducerMethod]
        public static TreeViewState ReduceRegisterContainerAction(
            TreeViewState inState, RegisterContainerAction registerContainerAction)
        {
            var inContainer = inState.ContainerList.FirstOrDefault(
                x => x.Key == registerContainerAction.Container.Key);

            if (inContainer is not null)
                return inState;

            var outContainerList = new List<TreeViewContainer>(inState.ContainerList);
            outContainerList.Add(registerContainerAction.Container);
            
            return inState with { ContainerList = outContainerList };
        }

        [ReducerMethod]
        public static TreeViewState ReduceDisposeContainerAction(
            TreeViewState inState, DisposeContainerAction disposeContainerAction)
        {
            var indexContainer = inState.ContainerList.FindIndex(
                x => x.Key == disposeContainerAction.ContainerKey);

            if (indexContainer == -1)
                return inState;
            
            var outContainerList = new List<TreeViewContainer>(inState.ContainerList);
            outContainerList.RemoveAt(indexContainer);
            
            return inState with { ContainerList = outContainerList };
        }

        [ReducerMethod]
        public static TreeViewState ReduceWithRootNodeAction(
            TreeViewState inState, WithRootNodeAction withRootNodeAction)
        {
            var indexContainer = inState.ContainerList.FindIndex(
                x => x.Key == withRootNodeAction.ContainerKey);

            if (indexContainer == -1)
                return inState;

            var inContainer = inState.ContainerList[indexContainer];
            
            var outContainer = inContainer with
            {
                RootNode = withRootNodeAction.Node,
                SelectedNodeList = new() { withRootNodeAction.Node }
            };

            var outContainerList = new List<TreeViewContainer>(inState.ContainerList);
            outContainerList[indexContainer] = outContainer;
            
            return inState with { ContainerList = outContainerList };
        }

        [ReducerMethod]
        public static TreeViewState ReduceAddChildNodeAction(
            TreeViewState inState, AddChildNodeAction addChildNodeAction)
        {
            var inContainer = inState.ContainerList.FirstOrDefault(
                x => x.Key == addChildNodeAction.ContainerKey);

            if (inContainer is null)
                return inState;

            var parent = addChildNodeAction.ParentNode;
            var child = addChildNodeAction.ChildNode;

            child.Parent = parent;
            child.IndexAmongSiblings = parent.ChildList.Count;
            child.TreeViewChangedKey = Key<TreeViewChanged>.NewKey();

            parent.ChildList.Add(child);

            var reRenderNodeAction = new ReRenderNodeAction(
                addChildNodeAction.ContainerKey,
                parent);

            return ReduceReRenderNodeAction(
                inState,
                reRenderNodeAction);
        }

        [ReducerMethod]
        public static TreeViewState ReduceReRenderNodeAction(
            TreeViewState inState, ReRenderNodeAction reRenderNodeAction)
        {
            var indexContainer = inState.ContainerList.FindIndex(
                x => x.Key == reRenderNodeAction.ContainerKey);

            if (indexContainer == -1)
                return inState;

            var inContainer = inState.ContainerList[indexContainer];
            
            var outContainer = PerformReRenderNode(inContainer, reRenderNodeAction);

            var outContainerList = new List<TreeViewContainer>(inState.ContainerList);
            outContainerList[indexContainer] = outContainer;
            
            return inState with { ContainerList = outContainerList };
        }

        [ReducerMethod]
        public static TreeViewState ReduceSetActiveNodeAction(
            TreeViewState inState, SetActiveNodeAction setActiveNodeAction)
        {
            var indexContainer = inState.ContainerList.FindIndex(
                x => x.Key == setActiveNodeAction.ContainerKey);

            if (indexContainer == -1)
                return inState;

			var inContainer = inState.ContainerList[indexContainer];
			
			var outContainer = PerformSetActiveNode(inContainer, setActiveNodeAction);

			var outContainerList = new List<TreeViewContainer>(inState.ContainerList);
			outContainerList[indexContainer] = outContainer;

            return inState with { ContainerList = outContainerList };
        }

        [ReducerMethod]
        public static TreeViewState ReduceRemoveSelectedNodeAction(
            TreeViewState inState, RemoveSelectedNodeAction removeSelectedNodeAction)
        {
            var indexContainer = inState.ContainerList.FindIndex(
                x => x.Key == removeSelectedNodeAction.ContainerKey);

            if (indexContainer == -1)
                return inState;

			var inContainer = inState.ContainerList[indexContainer];
			
			var outContainer = PerformRemoveSelectedNode(inContainer, removeSelectedNodeAction);

			var outContainerList = new List<TreeViewContainer>(inState.ContainerList);
				
			outContainerList[indexContainer] = outContainer;

            return inState with { ContainerList = outContainerList };
        }

        [ReducerMethod]
        public static TreeViewState ReduceMoveLeftAction(
            TreeViewState inState, MoveLeftAction moveLeftAction)
        {
            var indexContainer = inState.ContainerList.FindIndex(
                x => x.Key == moveLeftAction.ContainerKey);

			if (indexContainer == -1)
				return inState;
				
			var inContainer = inState.ContainerList[indexContainer];
            if (inContainer?.ActiveNode is null)
                return inState;

            var outContainer = PerformMoveLeft(inContainer, moveLeftAction);

            var outContainerList = new List<TreeViewContainer>(inState.ContainerList);
            outContainerList[indexContainer] = outContainer;

            return inState with { ContainerList = outContainerList };
        }

        [ReducerMethod]
        public static TreeViewState ReduceMoveDownAction(
            TreeViewState inState, MoveDownAction moveDownAction)
        {
            var indexContainer = inState.ContainerList.FindIndex(
                x => x.Key == moveDownAction.ContainerKey);

            if (indexContainer == -1)
            	return inState;
            
            var inContainer = inState.ContainerList[indexContainer];
            if (inContainer?.ActiveNode is null)
                return inState;

            var outContainer = PerformMoveDown(inContainer, moveDownAction);

            var outContainerList = new List<TreeViewContainer>(inState.ContainerList);
            outContainerList[indexContainer] = outContainer;
            
            return inState with { ContainerList = outContainerList };
        }

        [ReducerMethod]
        public static TreeViewState ReduceMoveUpAction(
            TreeViewState inState, MoveUpAction moveUpAction)
        {
            var indexContainer = inState.ContainerList.FindIndex(
                x => x.Key == moveUpAction.ContainerKey);

			if (indexContainer == -1)
                return inState;

            var inContainer = inState.ContainerList[indexContainer];
            
            var outContainer = PerformMoveUp(inContainer, moveUpAction);

            var outContainerList = new List<TreeViewContainer>(inState.ContainerList);
            outContainerList[indexContainer] = outContainer;
            
            return inState with { ContainerList = outContainerList };
        }

        [ReducerMethod]
        public static TreeViewState ReduceMoveRightAction(
            TreeViewState inState, MoveRightAction moveRightAction)
        {
        	var indexContainer = inState.ContainerList.FindIndex(
                x => x.Key == moveRightAction.ContainerKey);
			
			if (indexContainer == -1)
                return inState;
			
            var inContainer = inState.ContainerList[indexContainer];
                
            if (inContainer?.ActiveNode is null)
                return inState;

            var outContainer = PerformMoveRight(inContainer, moveRightAction);

			var outContainerList = new List<TreeViewContainer>(inState.ContainerList);
            outContainerList[indexContainer] = outContainer;
            
            return inState with { ContainerList = outContainerList };
        }

        [ReducerMethod]
        public static TreeViewState ReduceMoveHomeAction(
            TreeViewState inState, MoveHomeAction moveHomeAction)
        {
        	var indexContainer = inState.ContainerList.FindIndex(
                x => x.Key == moveHomeAction.ContainerKey);
                
			if (indexContainer == -1)
                return inState;
                
            var inContainer = inState.ContainerList[indexContainer];
            if (inContainer?.ActiveNode is null)
                return inState;

            var outContainer = PerformMoveHome(inContainer, moveHomeAction);

			var outContainerList = new List<TreeViewContainer>(inState.ContainerList);
            outContainerList[indexContainer] = outContainer;
            
            return inState with { ContainerList = outContainerList };
        }

        [ReducerMethod]
        public static TreeViewState ReduceMoveEndAction(
            TreeViewState inState, MoveEndAction moveEndAction)
        {
        	var indexContainer = inState.ContainerList.FindIndex(
                x => x.Key == moveEndAction.ContainerKey);
			
			if (indexContainer == -1)
                return inState;
            
            var inContainer = inState.ContainerList[indexContainer];
            if (inContainer?.ActiveNode is null)
                return inState;

            var outContainer = PerformMoveEnd(inContainer, moveEndAction);

			var outContainerList = new List<TreeViewContainer>(inState.ContainerList);
            outContainerList[indexContainer] = outContainer;
            
            return inState with { ContainerList = outContainerList };
        }

		private static void PerformMarkForRerender(TreeViewNoType node)
        {
            var markForRerenderTarget = node;

            while (markForRerenderTarget is not null)
            {
                markForRerenderTarget.TreeViewChangedKey = Key<TreeViewChanged>.NewKey();
                markForRerenderTarget = markForRerenderTarget.Parent;
            }
        }

		private static TreeViewContainer PerformReRenderNode(
			TreeViewContainer inContainer,
			ReRenderNodeAction reRenderNodeAction)
		{
			PerformMarkForRerender(reRenderNodeAction.Node);
            return inContainer with { StateId = Guid.NewGuid() };
		}

		private static TreeViewContainer PerformSetActiveNode(
			TreeViewContainer inContainer,
			SetActiveNodeAction setActiveNodeAction)
		{
			if (inContainer.ActiveNode is not null)
                PerformMarkForRerender(inContainer.ActiveNode);

            if (setActiveNodeAction.NextActiveNode is not null)
                PerformMarkForRerender(setActiveNodeAction.NextActiveNode);

			var inSelectedNodeList = inContainer.SelectedNodeList;
			var selectedNodeListWasCleared = false;

			TreeViewContainer outContainer;

			// TODO: I'm adding multi-select. I'd like to single out the...
			// ...SelectNodesBetweenCurrentAndNextActiveNode case for now...
			// ...and DRY the code after. (2024-01-13) 
			if (setActiveNodeAction.SelectNodesBetweenCurrentAndNextActiveNode)
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
					var nextTarget = setActiveNodeAction.NextActiveNode;

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

					var moveDownAction = new MoveDownAction(
						setActiveNodeAction.ContainerKey,
						true,
						false);

					var previousNode = outContainer.ActiveNode;

					while (true)
					{
						outContainer = PerformMoveDown(
							outContainer,
							moveDownAction);

						if (previousNode.Key == outContainer.ActiveNode.Key)
						{
							// No change occurred, avoid an infinite loop and break
							break;
						}
						else
						{
							previousNode = outContainer.ActiveNode;
						}

						if (setActiveNodeAction.NextActiveNode.Key ==
								outContainer.ActiveNode.Key)
						{
							// Target acquired
							break;
						}
					}
				}
				else if (direction < 0)
				{
					// Move up

					var moveUpAction = new MoveUpAction(
						setActiveNodeAction.ContainerKey,
						true,
						false);

					var previousNode = outContainer.ActiveNode;

					while (true)
					{
						outContainer = PerformMoveUp(
							outContainer,
							moveUpAction);

						if (previousNode.Key == outContainer.ActiveNode.Key)
						{
							// No change occurred, avoid an infinite loop and break
							break;
						}
						else
						{
							previousNode = outContainer.ActiveNode;
						}

						if (setActiveNodeAction.NextActiveNode.Key ==
								outContainer.ActiveNode.Key)
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
				if (setActiveNodeAction.NextActiveNode is null)
				{
					selectedNodeListWasCleared = true;
	
					outContainer = inContainer with
		            {
		                SelectedNodeList = TreeViewNoType.GetEmptyTreeViewNoTypeList()
		            };
				}
				else if (!setActiveNodeAction.AddSelectedNodes)
				{
					selectedNodeListWasCleared = true;
	
					outContainer = inContainer with
		            {
		                SelectedNodeList = new()
						{
							setActiveNodeAction.NextActiveNode
						}
		            };
				}
				else
				{
					var alreadyExistingIndex = inContainer.SelectedNodeList.FindIndex(
						x => setActiveNodeAction.NextActiveNode.Equals(x));
					
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
						outSelectedNodeList.Insert(0, setActiveNodeAction.NextActiveNode);
						
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
        
        private static TreeViewContainer PerformRemoveSelectedNode(
			TreeViewContainer inContainer,
            RemoveSelectedNodeAction removeSelectedNodeAction)
        {
            var indexOfNodeToRemove = inContainer.SelectedNodeList.FindIndex(
                x => x.Key == removeSelectedNodeAction.KeyOfNodeToRemove);

			var outSelectedNodeList = new List<TreeViewNoType>(inContainer.SelectedNodeList);
			outSelectedNodeList.RemoveAt(indexOfNodeToRemove);

            return inContainer with
            {
                SelectedNodeList = outSelectedNodeList
            };
        }
        
        private static TreeViewContainer PerformMoveLeft(
			TreeViewContainer inContainer,
			MoveLeftAction moveLeftAction)
		{
			var outContainer = inContainer;

			if (moveLeftAction.AddSelectedNodes)
                return outContainer;

            if (outContainer.ActiveNode is null)
                return outContainer;

            if (outContainer.ActiveNode.IsExpanded &&
                outContainer.ActiveNode.IsExpandable)
            {
                outContainer.ActiveNode.IsExpanded = false;

                var reRenderNodeAction = new ReRenderNodeAction(
                    outContainer.Key,
                    outContainer.ActiveNode);

                return PerformReRenderNode(outContainer, reRenderNodeAction);
            }

            if (outContainer.ActiveNode.Parent is not null)
            {
                var setActiveNodeAction = new SetActiveNodeAction(
                    outContainer.Key,
                    outContainer.ActiveNode.Parent,
                    false,
					false);

                outContainer = PerformSetActiveNode(
                    outContainer,
                    setActiveNodeAction);
            }

			return outContainer;
		}

		private static TreeViewContainer PerformMoveDown(
			TreeViewContainer inContainer,
			MoveDownAction moveDownAction)
		{
			var outContainer = inContainer;

            if (outContainer.ActiveNode.IsExpanded &&
                outContainer.ActiveNode.ChildList.Any())
            {
                var nextActiveNode = outContainer.ActiveNode.ChildList[0];

                var setActiveNodeAction = new SetActiveNodeAction(
                    outContainer.Key,
                    nextActiveNode,
					moveDownAction.AddSelectedNodes,
					moveDownAction.SelectNodesBetweenCurrentAndNextActiveNode);

                outContainer = PerformSetActiveNode(
                    outContainer,
                    setActiveNodeAction);
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

                var setActiveNodeAction = new SetActiveNodeAction(
                    outContainer.Key,
                    nextActiveNode,
					moveDownAction.AddSelectedNodes,
					moveDownAction.SelectNodesBetweenCurrentAndNextActiveNode);

                outContainer = PerformSetActiveNode(
                    outContainer,
                    setActiveNodeAction);
            }

			return outContainer;
		}

		private static TreeViewContainer PerformMoveUp(
			TreeViewContainer inContainer,
			MoveUpAction moveUpAction)
		{
			var outContainer = inContainer;

			if (outContainer?.ActiveNode?.Parent is null)
                return outContainer;

            if (outContainer.ActiveNode.IndexAmongSiblings == 0)
            {
                var setActiveNodeAction = new SetActiveNodeAction(
                    outContainer.Key,
                    outContainer.ActiveNode!.Parent,
					moveUpAction.AddSelectedNodes,
					moveUpAction.SelectNodesBetweenCurrentAndNextActiveNode);

                outContainer = PerformSetActiveNode(
                    outContainer,
                    setActiveNodeAction);
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

                var setActiveNodeAction = new SetActiveNodeAction(
                    outContainer.Key,
                    target,
					moveUpAction.AddSelectedNodes,
					moveUpAction.SelectNodesBetweenCurrentAndNextActiveNode);

                outContainer = PerformSetActiveNode(
                    outContainer,
                    setActiveNodeAction);
            }

			return outContainer;
		}

		private static TreeViewContainer PerformMoveRight(
			TreeViewContainer inContainer,
			MoveRightAction moveRightAction)
		{
			var outContainer = inContainer;

			if (outContainer is null || outContainer.ActiveNode is null)
                return outContainer;

            if (moveRightAction.AddSelectedNodes)
                return outContainer;

            if (outContainer.ActiveNode is null)
                return outContainer;

            if (outContainer.ActiveNode.IsExpanded)
            {
                if (outContainer.ActiveNode.ChildList.Any())
                {
                    var setActiveNodeAction = new SetActiveNodeAction(
                        outContainer.Key,
                        outContainer.ActiveNode.ChildList[0],
						moveRightAction.AddSelectedNodes,
						moveRightAction.SelectNodesBetweenCurrentAndNextActiveNode);

                    outContainer = PerformSetActiveNode(
                        outContainer,
                        setActiveNodeAction);
                }
            }
            else if (outContainer.ActiveNode.IsExpandable)
            {
                outContainer.ActiveNode.IsExpanded = true;

                moveRightAction.LoadChildListAction.Invoke(
                    outContainer.ActiveNode);
            }

			return outContainer;
		}

		private static TreeViewContainer PerformMoveHome(
			TreeViewContainer inContainer,
			MoveHomeAction moveHomeAction)
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

            var setActiveNodeAction = new SetActiveNodeAction(
                outContainer.Key,
                target,
				moveHomeAction.AddSelectedNodes,
				moveHomeAction.SelectNodesBetweenCurrentAndNextActiveNode);

            return PerformSetActiveNode(
                outContainer,
                setActiveNodeAction);
		}

		private static TreeViewContainer PerformMoveEnd(
			TreeViewContainer inContainer,
			MoveEndAction moveEndAction)
		{
			var outContainer = inContainer;

            var target = outContainer.RootNode;

            while (target.IsExpanded && target.ChildList.Any())
            {
                target = target.ChildList.Last();
            }

            var setActiveNodeAction = new SetActiveNodeAction(
                outContainer.Key,
                target,
				moveEndAction.AddSelectedNodes,
				moveEndAction.SelectNodesBetweenCurrentAndNextActiveNode);

            return PerformSetActiveNode(
                outContainer,
                setActiveNodeAction);
		}
    }
}