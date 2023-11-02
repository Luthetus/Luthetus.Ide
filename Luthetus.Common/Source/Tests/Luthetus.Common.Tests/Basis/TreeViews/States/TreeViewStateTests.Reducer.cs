using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;

namespace Luthetus.Common.RazorLib.TreeViews.States;

public partial record TreeViewStateTests
{
    private class Reducer
    {
        [ReducerMethod]
        public static TreeViewState ReduceRegisterContainerAction(
            TreeViewState inState, RegisterContainerAction registerContainerAction)
        {
            var inContainer = inState.ContainerBag.FirstOrDefault(
                x => x.Key == registerContainerAction.Container.Key);

            if (inContainer is not null)
                return inState;

            var outContainerBag = inState.ContainerBag.Add(registerContainerAction.Container);

            return inState with { ContainerBag = outContainerBag };
        }

        [ReducerMethod]
        public static TreeViewState ReduceDisposeContainerAction(
            TreeViewState inState, DisposeContainerAction disposeContainerAction)
        {
            var inContainer = inState.ContainerBag.FirstOrDefault(
                x => x.Key == disposeContainerAction.ContainerKey);

            if (inContainer is null)
                return inState;

            var outContainerBag = inState.ContainerBag.Remove(inContainer);

            return inState with { ContainerBag = outContainerBag };
        }

        [ReducerMethod]
        public static TreeViewState ReduceWithRootNodeAction(
            TreeViewState inState, WithRootNodeAction withRootNodeAction)
        {
            var inContainer = inState.ContainerBag.FirstOrDefault(
                x => x.Key == withRootNodeAction.ContainerKey);

            if (inContainer is null)
                return inState;

            var outContainer = inContainer with
            {
                RootNode = withRootNodeAction.Node,
                SelectedNodeBag = new TreeViewNoType[] { withRootNodeAction.Node }.ToImmutableList()
            };

            var outContainerBag = inState.ContainerBag.Replace(inContainer, outContainer);

            return inState with { ContainerBag = outContainerBag };
        }

        [ReducerMethod]
        public static TreeViewState ReduceAddChildNodeAction(
            TreeViewState inState, AddChildNodeAction addChildNodeAction)
        {
            var inContainer = inState.ContainerBag.FirstOrDefault(
                x => x.Key == addChildNodeAction.ContainerKey);

            if (inContainer is null)
                return inState;

            var parent = addChildNodeAction.ParentNode;
            var child = addChildNodeAction.ChildNode;

            child.Parent = parent;
            child.IndexAmongSiblings = parent.ChildBag.Count;
            child.TreeViewChangedKey = Key<TreeViewChanged>.NewKey();

            parent.ChildBag.Add(child);

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
            var inContainer = inState.ContainerBag.FirstOrDefault(
                x => x.Key == reRenderNodeAction.ContainerKey);

            if (inContainer is null)
                return inState;

            PerformMarkForRerender(reRenderNodeAction.Node);

            var outContainer = inContainer with { StateId = Guid.NewGuid() };

            var outContainerBag = inState.ContainerBag.Replace(inContainer, outContainer);

            return inState with { ContainerBag = outContainerBag };
        }

        [ReducerMethod]
        public static TreeViewState ReduceSetActiveNodeAction(
            TreeViewState inState, SetActiveNodeAction setActiveNodeAction)
        {
            var inContainer = inState.ContainerBag.FirstOrDefault(
                x => x.Key == setActiveNodeAction.ContainerKey);

            if (inContainer is null)
                return inState;

            if (inContainer.ActiveNode is not null)
                PerformMarkForRerender(inContainer.ActiveNode);

            if (setActiveNodeAction.NextActiveNode is not null)
                PerformMarkForRerender(setActiveNodeAction.NextActiveNode);

            var outContainerBag = inState.ContainerBag.Replace(inContainer, inContainer with
            {
                SelectedNodeBag = setActiveNodeAction.NextActiveNode is null
                    ? ImmutableList<TreeViewNoType>.Empty
                    : new TreeViewNoType[] { setActiveNodeAction.NextActiveNode }.ToImmutableList()
            });

            return inState with { ContainerBag = outContainerBag };
        }

        [ReducerMethod]
        public static TreeViewState ReduceAddSelectedNodeAction(
            TreeViewState inState, AddSelectedNodeAction addSelectedNodeAction)
        {
            var inContainer = inState.ContainerBag.FirstOrDefault(
                x => x.Key == addSelectedNodeAction.ContainerKey);

            if (inContainer is null)
                return inState;

            var outContainer = PerformAddSelectedNode(
                inContainer,
                addSelectedNodeAction);

            var outContainerBag = inState.ContainerBag.Replace(inContainer, outContainer);

            return inState with { ContainerBag = outContainerBag };
        }

        [ReducerMethod]
        public static TreeViewState ReduceRemoveSelectedNodeAction(
            TreeViewState inState, RemoveSelectedNodeAction removeSelectedNodeAction)
        {
            var inContainer = inState.ContainerBag.FirstOrDefault(
                x => x.Key == removeSelectedNodeAction.ContainerKey);

            if (inContainer is null)
                return inState;

            var nodeToRemove = inContainer.SelectedNodeBag.FirstOrDefault(
                x => x.Key == removeSelectedNodeAction.NodeKey);

            if (nodeToRemove is null)
                return inState;

            PerformMarkForRerender(nodeToRemove);

            var outSelectedNodeBag = inContainer.SelectedNodeBag.Remove(nodeToRemove);

            var outContainerBag = inState.ContainerBag.Replace(inContainer, inContainer with
            {
                SelectedNodeBag = outSelectedNodeBag
            });

            return inState with { ContainerBag = outContainerBag };
        }

        [ReducerMethod]
        public static TreeViewState ReduceClearSelectedNodeBagAction(
            TreeViewState inState, ClearSelectedNodeBagAction clearSelectedNodeBagAction)
        {
            var inContainer = inState.ContainerBag.FirstOrDefault(
                x => x.Key == clearSelectedNodeBagAction.ContainerKey);

            if (inContainer is null)
                return inState;

            var outContainer = PerformClearSelectedNodes(inContainer);
            var outContainerBag = inState.ContainerBag.Replace(inContainer, outContainer);

            return inState with { ContainerBag = outContainerBag };
        }

        [ReducerMethod]
        public static TreeViewState ReduceMoveLeftAction(
            TreeViewState inState, MoveLeftAction moveLeftAction)
        {
            var inContainer = inState.ContainerBag.FirstOrDefault(
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
                    outContainer.ActiveNode.Parent);

                outContainer = PerformSetActiveNode(
                    outContainer,
                    setActiveNodeAction);
            }

            var outContainerBag = inState.ContainerBag.Replace(
                inContainer,
                outContainer);

            return inState with { ContainerBag = outContainerBag };
        }

        [ReducerMethod]
        public static TreeViewState ReduceMoveDownAction(
            TreeViewState inState, MoveDownAction moveDownAction)
        {
            var inContainer = inState.ContainerBag.FirstOrDefault(
                x => x.Key == moveDownAction.ContainerKey);

            if (inContainer is null || inContainer.ActiveNode is null)
                return inState;

            var outContainer = inContainer;

            if (!moveDownAction.ShiftKey)
            {
                outContainer = PerformClearSelectedNodes(outContainer);

                if (outContainer.ActiveNode is null)
                    return inState;
            }

            if (outContainer.ActiveNode.IsExpanded &&
                outContainer.ActiveNode.ChildBag.Any())
            {
                var nextActiveNode = outContainer.ActiveNode.ChildBag[0];

                if (moveDownAction.ShiftKey)
                {
                    var addSelectedNodeAction = new AddSelectedNodeAction(
                        moveDownAction.ContainerKey,
                        nextActiveNode);

                    outContainer = PerformAddSelectedNode(
                        outContainer,
                        addSelectedNodeAction);
                }

                var setActiveNodeAction = new SetActiveNodeAction(
                    outContainer.Key,
                    nextActiveNode);

                outContainer = PerformSetActiveNode(
                    outContainer,
                    setActiveNodeAction);
            }
            else
            {
                var target = outContainer.ActiveNode;

                while (target.Parent is not null &&
                       target.IndexAmongSiblings == target.Parent.ChildBag.Count - 1)
                {
                    target = target.Parent;
                }

                if (target.Parent is null ||
                    target.IndexAmongSiblings == target.Parent.ChildBag.Count - 1)
                {
                    return inState;
                }

                var nextActiveNode = target.Parent.ChildBag[
                    target.IndexAmongSiblings +
                    1];

                if (moveDownAction.ShiftKey)
                {
                    var addSelectedNodeAction = new AddSelectedNodeAction(
                        moveDownAction.ContainerKey,
                        nextActiveNode);

                    outContainer = PerformAddSelectedNode(
                        outContainer,
                        addSelectedNodeAction);
                }

                var setActiveNodeAction = new SetActiveNodeAction(
                    outContainer.Key,
                    nextActiveNode);

                outContainer = PerformSetActiveNode(
                    outContainer,
                    setActiveNodeAction);
            }

            var outContainerBag = inState.ContainerBag.Replace(inContainer, outContainer);

            return inState with { ContainerBag = outContainerBag };
        }

        [ReducerMethod]
        public static TreeViewState ReduceMoveUpAction(
            TreeViewState inState, MoveUpAction moveUpAction)
        {
            var inContainer = inState.ContainerBag.FirstOrDefault(
                x => x.Key == moveUpAction.ContainerKey);

            if (inContainer is null)
                return inState;

            var refContainer = inContainer;

            if (refContainer?.ActiveNode?.Parent is null)
                return inState;

            if (!moveUpAction.ShiftKey)
            {
                refContainer = PerformClearSelectedNodes(refContainer);

                // Another null check here is awkward, but the 'PerformClearSelectedNodes(...)'
                // invocation might return a new refContainer with null members.
                //
                // TODO: Perhaps rewriting some logic is necessary here to avoid awkward null checks
                if (refContainer.ActiveNode?.Parent is null)
                    return inState;
            }

            if (refContainer.ActiveNode.IndexAmongSiblings == 0)
            {
                var nextActiveNode = refContainer.ActiveNode.Parent;

                if (moveUpAction.ShiftKey)
                {
                    var addSelectedNodeAction = new AddSelectedNodeAction(
                        moveUpAction.ContainerKey,
                        nextActiveNode);

                    refContainer = PerformAddSelectedNode(
                        refContainer,
                        addSelectedNodeAction);
                }

                var setActiveNodeAction = new SetActiveNodeAction(
                    refContainer.Key,
                    refContainer.ActiveNode!.Parent);

                refContainer = PerformSetActiveNode(
                    refContainer,
                    setActiveNodeAction);
            }
            else
            {
                var target = refContainer.ActiveNode.Parent.ChildBag[
                    refContainer.ActiveNode.IndexAmongSiblings - 1];

                while (true)
                {
                    if (moveUpAction.ShiftKey)
                    {
                        var addSelectedNodeAction = new AddSelectedNodeAction(
                            moveUpAction.ContainerKey,
                            target);

                        refContainer = PerformAddSelectedNode(
                            refContainer,
                            addSelectedNodeAction);
                    }

                    if (target.IsExpanded &&
                        target.ChildBag.Any())
                    {
                        target = target.ChildBag.Last();
                    }
                    else
                    {
                        break;
                    }
                }

                var setActiveNodeAction = new SetActiveNodeAction(
                    refContainer.Key,
                    target);

                refContainer = PerformSetActiveNode(
                    refContainer,
                    setActiveNodeAction);
            }

            var outContainerBag = inState.ContainerBag.Replace(inContainer, refContainer);

            return inState with { ContainerBag = outContainerBag };
        }

        [ReducerMethod]
        public static TreeViewState ReduceMoveRightAction(
            TreeViewState inState, MoveRightAction moveRightAction)
        {
            var inContainer = inState.ContainerBag.FirstOrDefault(
                x => x.Key == moveRightAction.ContainerKey);

            if (inContainer is null || inContainer.ActiveNode is null)
                return inState;

            var outContainer = inState.ContainerBag.FirstOrDefault(
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
                if (outContainer.ActiveNode.ChildBag.Any())
                {
                    var setActiveNodeAction = new SetActiveNodeAction(
                        outContainer.Key,
                        outContainer.ActiveNode.ChildBag[0]);

                    outContainer = PerformSetActiveNode(
                        outContainer,
                        setActiveNodeAction);
                }
            }
            else if (outContainer.ActiveNode.IsExpandable)
            {
                outContainer.ActiveNode.IsExpanded = true;

                moveRightAction.LoadChildBagAction.Invoke(
                    outContainer.ActiveNode);
            }

            var outContainerBag = inState.ContainerBag.Replace(inContainer, outContainer);

            return inState with { ContainerBag = outContainerBag };
        }

        [ReducerMethod]
        public static TreeViewState ReduceMoveHomeAction(
            TreeViewState inState, MoveHomeAction moveHomeAction)
        {
            var inContainer = inState.ContainerBag.FirstOrDefault(
                x => x.Key == moveHomeAction.ContainerKey);

            if (inContainer is null || inContainer.ActiveNode is null)
                return inState;

            var outContainer = inContainer;

            if (!moveHomeAction.ShiftKey)
            {
                outContainer = PerformClearSelectedNodes(outContainer);

                if (outContainer.ActiveNode is null)
                    return inState;
            }

            TreeViewNoType target;

            if (outContainer.RootNode is TreeViewAdhoc)
            {
                if (outContainer.RootNode.ChildBag.Any())
                    target = outContainer.RootNode.ChildBag[0];
                else
                    target = outContainer.RootNode;
            }
            else
            {
                target = outContainer.RootNode;
            }

            if (moveHomeAction.ShiftKey)
            {
                var addSelectedNodeAction = new AddSelectedNodeAction(
                    moveHomeAction.ContainerKey,
                    target);

                outContainer = PerformAddSelectedNode(
                    outContainer,
                    addSelectedNodeAction);
            }

            var setActiveNodeAction = new SetActiveNodeAction(
                outContainer.Key,
                target);

            outContainer = PerformSetActiveNode(
                outContainer,
                setActiveNodeAction);

            var outContainerBag = inState.ContainerBag.Replace(inContainer, outContainer);

            return inState with { ContainerBag = outContainerBag };
        }

        [ReducerMethod]
        public static TreeViewState ReduceMoveEndAction(
            TreeViewState inState, MoveEndAction moveEndAction)
        {
            var inContainer = inState.ContainerBag.FirstOrDefault(
                x => x.Key == moveEndAction.ContainerKey);

            if (inContainer is null || inContainer.ActiveNode is null)
                return inState;

            var outContainer = inContainer;

            if (!moveEndAction.ShiftKey)
            {
                outContainer = PerformClearSelectedNodes(outContainer);

                if (outContainer.ActiveNode is null)
                    return inState;
            }

            var target = outContainer.RootNode;

            while (target.IsExpanded && target.ChildBag.Any())
            {
                target = target.ChildBag.Last();
            }

            if (moveEndAction.ShiftKey)
            {
                var addSelectedNodeAction = new AddSelectedNodeAction(
                    moveEndAction.ContainerKey,
                    target);

                outContainer = PerformAddSelectedNode(
                    outContainer,
                    addSelectedNodeAction);
            }

            var setActiveNodeAction = new SetActiveNodeAction(
                outContainer.Key,
                target);

            outContainer = PerformSetActiveNode(
                outContainer,
                setActiveNodeAction);

            var outContainerBag = inState.ContainerBag.Replace(inContainer, outContainer);

            return inState with { ContainerBag = outContainerBag };
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

            var selectedNodes = inContainer.SelectedNodeBag;

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

            var outSelectedNodeBag = selectedNodes.Add(addSelectedNodeAction.SelectedNode);

            PerformMarkForRerender(addSelectedNodeAction.SelectedNode);
            PerformMarkForRerender(inContainer.ActiveNode);

            return inContainer with
            {
                SelectedNodeBag = outSelectedNodeBag
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
                SelectedNodeBag = setActiveNodeAction.NextActiveNode is null
                    ? ImmutableList<TreeViewNoType>.Empty
                    : new TreeViewNoType[] { setActiveNodeAction.NextActiveNode }.ToImmutableList()
            };
        }

        private static TreeViewContainer PerformClearSelectedNodes(TreeViewContainer inContainer)
        {
            if (!inContainer.SelectedNodeBag.Any())
                return inContainer;

            foreach (var node in inContainer.SelectedNodeBag)
            {
                PerformMarkForRerender(node);
            }

            var outSelectedNodeBag = inContainer.ActiveNode is null
                ? ImmutableList<TreeViewNoType>.Empty
                : new TreeViewNoType[] { inContainer.ActiveNode }.ToImmutableList();

            return inContainer with
            {
                SelectedNodeBag = outSelectedNodeBag
            };
        }
    }
}