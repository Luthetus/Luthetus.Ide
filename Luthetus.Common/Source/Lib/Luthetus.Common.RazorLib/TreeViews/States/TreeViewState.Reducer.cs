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

            var outContainerList = inState.ContainerList.Add(registerContainerAction.Container);

            return inState with { ContainerList = outContainerList };
        }

        [ReducerMethod]
        public static TreeViewState ReduceDisposeContainerAction(
            TreeViewState inState, DisposeContainerAction disposeContainerAction)
        {
            var inContainer = inState.ContainerList.FirstOrDefault(
                x => x.Key == disposeContainerAction.ContainerKey);

            if (inContainer is null)
                return inState;

            var outContainerList = inState.ContainerList.Remove(inContainer);

            return inState with { ContainerList = outContainerList };
        }

        [ReducerMethod]
        public static TreeViewState ReduceWithRootNodeAction(
            TreeViewState inState, WithRootNodeAction withRootNodeAction)
        {
            var inContainer = inState.ContainerList.FirstOrDefault(
                x => x.Key == withRootNodeAction.ContainerKey);

            if (inContainer is null)
                return inState;

            var outContainer = inContainer with
            {
                RootNode = withRootNodeAction.Node,
                SelectedNodeList = new TreeViewNoType[] { withRootNodeAction.Node }.ToImmutableList()
            };

            var outContainerList = inState.ContainerList.Replace(inContainer, outContainer);

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
            var inContainer = inState.ContainerList.FirstOrDefault(
                x => x.Key == reRenderNodeAction.ContainerKey);

            if (inContainer is null)
                return inState;

            PerformMarkForRerender(reRenderNodeAction.Node);

            var outContainer = inContainer with { StateId = Guid.NewGuid() };

            var outContainerList = inState.ContainerList.Replace(inContainer, outContainer);

            return inState with { ContainerList = outContainerList };
        }

        [ReducerMethod]
        public static TreeViewState ReduceSetActiveNodeAction(
            TreeViewState inState, SetActiveNodeAction setActiveNodeAction)
        {
            var inContainer = inState.ContainerList.FirstOrDefault(
                x => x.Key == setActiveNodeAction.ContainerKey);

            if (inContainer is null)
                return inState;

            if (inContainer.ActiveNode is not null)
                PerformMarkForRerender(inContainer.ActiveNode);

            if (setActiveNodeAction.NextActiveNode is not null)
                PerformMarkForRerender(setActiveNodeAction.NextActiveNode);

			ImmutableList<TreeViewContainer> outContainerList;

			if (setActiveNodeAction.NextActiveNode is null)
			{
				outContainerList = inState.ContainerList.Replace(inContainer, inContainer with
	            {
	                SelectedNodeList = ImmutableList<TreeViewNoType>.Empty
	            });
			}
			else if (setActiveNodeAction.ShouldClearSelectedNodes)
			{
				outContainerList = inState.ContainerList.Replace(inContainer, inContainer with
	            {
	                SelectedNodeList = new TreeViewNoType[] 
					{
						setActiveNodeAction.NextActiveNode
					}.ToImmutableList()
	            });
			}
			else
			{
				outContainerList = inState.ContainerList.Replace(inContainer, inContainer with
	            {
	                SelectedNodeList = inContainer.SelectedNodeList.Insert(
						0,
						setActiveNodeAction.NextActiveNode)
	            });
			}

            return inState with { ContainerList = outContainerList };
        }

        [ReducerMethod]
        public static TreeViewState ReduceAddSelectedNodeAction(
            TreeViewState inState, AddSelectedNodeAction addSelectedNodeAction)
        {
            var inContainer = inState.ContainerList.FirstOrDefault(
                x => x.Key == addSelectedNodeAction.ContainerKey);

            if (inContainer is null)
                return inState;

            var outContainer = PerformAddSelectedNode(
                inContainer,
                addSelectedNodeAction);

            var outContainerList = inState.ContainerList.Replace(inContainer, outContainer);

            return inState with { ContainerList = outContainerList };
        }

        [ReducerMethod]
        public static TreeViewState ReduceRemoveSelectedNodeAction(
            TreeViewState inState, RemoveSelectedNodeAction removeSelectedNodeAction)
        {
            var inContainer = inState.ContainerList.FirstOrDefault(
                x => x.Key == removeSelectedNodeAction.ContainerKey);

            if (inContainer is null)
                return inState;

            var nodeToRemove = inContainer.SelectedNodeList.FirstOrDefault(
                x => x.Key == removeSelectedNodeAction.NodeKey);

            if (nodeToRemove is null)
                return inState;

            PerformMarkForRerender(nodeToRemove);

            var outSelectedNodeList = inContainer.SelectedNodeList.Remove(nodeToRemove);

            var outContainerList = inState.ContainerList.Replace(inContainer, inContainer with
            {
                SelectedNodeList = outSelectedNodeList
            });

            return inState with { ContainerList = outContainerList };
        }

        [ReducerMethod]
        public static TreeViewState ReduceClearSelectedNodeListAction(
            TreeViewState inState, ClearSelectedNodeListAction clearSelectedNodeListAction)
        {
            var inContainer = inState.ContainerList.FirstOrDefault(
                x => x.Key == clearSelectedNodeListAction.ContainerKey);

            if (inContainer is null)
                return inState;

            var outContainer = PerformClearSelectedNodes(inContainer);
            var outContainerList = inState.ContainerList.Replace(inContainer, outContainer);

            return inState with { ContainerList = outContainerList };
        }

        [ReducerMethod]
        public static TreeViewState ReduceMoveLeftAction(
            TreeViewState inState, MoveLeftAction moveLeftAction)
        {
            var inContainer = inState.ContainerList.FirstOrDefault(
                x => x.Key == moveLeftAction.ContainerKey);

            if (inContainer is null || inContainer.ActiveNode is null)
                return inState;

            var outContainer = inContainer;

            if (moveLeftAction.ShiftKey)
                return inState;

            outContainer = PerformClearSelectedNodes(outContainer);

            if (outContainer.ActiveNode is null)
                return inState;

            if (outContainer.ActiveNode.IsExpanded &&
                outContainer.ActiveNode.IsExpandable)
            {
                outContainer.ActiveNode.IsExpanded = false;

                var reRenderNodeAction = new ReRenderNodeAction(
                    outContainer.Key,
                    outContainer.ActiveNode);

                return ReduceReRenderNodeAction(
                    inState,
                    reRenderNodeAction);
            }

            if (outContainer.ActiveNode.Parent is not null)
            {
                var setActiveNodeAction = new SetActiveNodeAction(
                    outContainer.Key,
                    outContainer.ActiveNode.Parent,
					true);

                outContainer = PerformSetActiveNode(
                    outContainer,
                    setActiveNodeAction);
            }

            var outContainerList = inState.ContainerList.Replace(
                inContainer,
                outContainer);

            return inState with { ContainerList = outContainerList };
        }

        [ReducerMethod]
        public static TreeViewState ReduceMoveDownAction(
            TreeViewState inState, MoveDownAction moveDownAction)
        {
            var inContainer = inState.ContainerList.FirstOrDefault(
                x => x.Key == moveDownAction.ContainerKey);

            if (inContainer is null || inContainer.ActiveNode is null)
                return inState;

            var outContainer = inContainer;
			var shouldClearSelectedNodes = false;

            if (!moveDownAction.ShiftKey)
				shouldClearSelectedNodes = true;

            if (outContainer.ActiveNode.IsExpanded &&
                outContainer.ActiveNode.ChildList.Any())
            {
                var nextActiveNode = outContainer.ActiveNode.ChildList[0];

                var setActiveNodeAction = new SetActiveNodeAction(
                    outContainer.Key,
                    nextActiveNode,
					shouldClearSelectedNodes);

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
                    return inState;
                }

                var nextActiveNode = target.Parent.ChildList[
                    target.IndexAmongSiblings +
                    1];

                var setActiveNodeAction = new SetActiveNodeAction(
                    outContainer.Key,
                    nextActiveNode,
					shouldClearSelectedNodes);

                outContainer = PerformSetActiveNode(
                    outContainer,
                    setActiveNodeAction);
            }

            var outContainerList = inState.ContainerList.Replace(inContainer, outContainer);

            return inState with { ContainerList = outContainerList };
        }

        [ReducerMethod]
        public static TreeViewState ReduceMoveUpAction(
            TreeViewState inState, MoveUpAction moveUpAction)
        {
            var inContainer = inState.ContainerList.FirstOrDefault(
                x => x.Key == moveUpAction.ContainerKey);

            if (inContainer is null)
                return inState;

            var refContainer = inContainer;

            if (refContainer?.ActiveNode?.Parent is null)
                return inState;

			var shouldClearSelectedNodes = false;

            if (!moveUpAction.ShiftKey)
				shouldClearSelectedNodes = true;

            if (refContainer.ActiveNode.IndexAmongSiblings == 0)
            {
                var nextActiveNode = refContainer.ActiveNode.Parent;

                var setActiveNodeAction = new SetActiveNodeAction(
                    refContainer.Key,
                    refContainer.ActiveNode!.Parent,
					shouldClearSelectedNodes);

                refContainer = PerformSetActiveNode(
                    refContainer,
                    setActiveNodeAction);
            }
            else
            {
                var target = refContainer.ActiveNode.Parent.ChildList[
                    refContainer.ActiveNode.IndexAmongSiblings - 1];

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
                    refContainer.Key,
                    target,
					shouldClearSelectedNodes);

                refContainer = PerformSetActiveNode(
                    refContainer,
                    setActiveNodeAction);
            }

            var outContainerList = inState.ContainerList.Replace(inContainer, refContainer);

            return inState with { ContainerList = outContainerList };
        }

        [ReducerMethod]
        public static TreeViewState ReduceMoveRightAction(
            TreeViewState inState, MoveRightAction moveRightAction)
        {
            var inContainer = inState.ContainerList.FirstOrDefault(
                x => x.Key == moveRightAction.ContainerKey);

            if (inContainer is null || inContainer.ActiveNode is null)
                return inState;

            var outContainer = inState.ContainerList.FirstOrDefault(
                x => x.Key == moveRightAction.ContainerKey);

            if (outContainer is null || outContainer.ActiveNode is null)
                return inState;

            if (moveRightAction.ShiftKey)
                return inState;

            outContainer = PerformClearSelectedNodes(outContainer);

            if (outContainer.ActiveNode is null)
                return inState;

            if (outContainer.ActiveNode.IsExpanded)
            {
                if (outContainer.ActiveNode.ChildList.Any())
                {
                    var setActiveNodeAction = new SetActiveNodeAction(
                        outContainer.Key,
                        outContainer.ActiveNode.ChildList[0],
						true);

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

            var outContainerList = inState.ContainerList.Replace(inContainer, outContainer);

            return inState with { ContainerList = outContainerList };
        }

        [ReducerMethod]
        public static TreeViewState ReduceMoveHomeAction(
            TreeViewState inState, MoveHomeAction moveHomeAction)
        {
            var inContainer = inState.ContainerList.FirstOrDefault(
                x => x.Key == moveHomeAction.ContainerKey);

            if (inContainer is null || inContainer.ActiveNode is null)
                return inState;

            var outContainer = inContainer;

           var shouldClearSelectedNodes = false;

            if (!moveHomeAction.ShiftKey)
				shouldClearSelectedNodes = true;

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
				shouldClearSelectedNodes);

            outContainer = PerformSetActiveNode(
                outContainer,
                setActiveNodeAction);

            var outContainerList = inState.ContainerList.Replace(inContainer, outContainer);

            return inState with { ContainerList = outContainerList };
        }

        [ReducerMethod]
        public static TreeViewState ReduceMoveEndAction(
            TreeViewState inState, MoveEndAction moveEndAction)
        {
            var inContainer = inState.ContainerList.FirstOrDefault(
                x => x.Key == moveEndAction.ContainerKey);

            if (inContainer is null || inContainer.ActiveNode is null)
                return inState;

            var outContainer = inContainer;

            var shouldClearSelectedNodes = false;

            if (!moveEndAction.ShiftKey)
				shouldClearSelectedNodes = true;

            var target = outContainer.RootNode;

            while (target.IsExpanded && target.ChildList.Any())
            {
                target = target.ChildList.Last();
            }

            var setActiveNodeAction = new SetActiveNodeAction(
                outContainer.Key,
                target,
				shouldClearSelectedNodes);

            outContainer = PerformSetActiveNode(
                outContainer,
                setActiveNodeAction);

            var outContainerList = inState.ContainerList.Replace(inContainer, outContainer);

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

        private static TreeViewContainer PerformAddSelectedNode(
            TreeViewContainer inContainer, AddSelectedNodeAction addSelectedNodeAction)
        {
            if (inContainer.ActiveNode is null)
                return inContainer;

            var selectedNodes = inContainer.SelectedNodeList;

            if (!selectedNodes.Any())
            {
                selectedNodes = new TreeViewNoType[]
                {
                    inContainer.ActiveNode
                }.ToImmutableList();
            }

            if (selectedNodes.Any(x => x.Key == addSelectedNodeAction.SelectedNode.Key))
            {
                return inContainer;
            }

            var outSelectedNodeList = selectedNodes.Add(addSelectedNodeAction.SelectedNode);

            PerformMarkForRerender(addSelectedNodeAction.SelectedNode);
            PerformMarkForRerender(inContainer.ActiveNode);

            return inContainer with
            {
                SelectedNodeList = outSelectedNodeList
            };
        }

        private static TreeViewContainer PerformSetActiveNode(
            TreeViewContainer inContainer, SetActiveNodeAction setActiveNodeAction)
        {
            if (inContainer.ActiveNode is not null)
                PerformMarkForRerender(inContainer.ActiveNode);

            if (setActiveNodeAction.NextActiveNode is not null)
                PerformMarkForRerender(setActiveNodeAction.NextActiveNode);

            return inContainer with
            {
                SelectedNodeList = setActiveNodeAction.NextActiveNode is null
                    ? ImmutableList<TreeViewNoType>.Empty
                    : new TreeViewNoType[] { setActiveNodeAction.NextActiveNode }.ToImmutableList()
            };
        }

        private static TreeViewContainer PerformClearSelectedNodes(TreeViewContainer inContainer)
        {
            if (!inContainer.SelectedNodeList.Any())
                return inContainer;

            foreach (var node in inContainer.SelectedNodeList)
            {
                PerformMarkForRerender(node);
            }

            var outSelectedNodeList = inContainer.ActiveNode is null
                ? ImmutableList<TreeViewNoType>.Empty
                : new TreeViewNoType[] { inContainer.ActiveNode }.ToImmutableList();

            return inContainer with
            {
                SelectedNodeList = outSelectedNodeList
            };
        }
    }
}