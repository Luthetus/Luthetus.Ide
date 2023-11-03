namespace Luthetus.Common.RazorLib.TreeViews.States;

public class TreeViewStateActionsTests
{
    [Fact]
    public void RegisterContainerAction()
    {
        /*
        public record RegisterContainerAction(TreeViewContainer Container);
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void DisposeContainerAction()
    {
        /*
        public record DisposeContainerAction(Key<TreeViewContainer> ContainerKey);
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void WithRootNodeAction()
    {
        /*
        public record WithRootNodeAction(Key<TreeViewContainer> ContainerKey, TreeViewNoType Node);
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void TryGetContainerAction()
    {
        /*
        public record TryGetContainerAction(Key<TreeViewContainer> ContainerKey);
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void ReplaceContainerAction()
    {
        /*
        public record ReplaceContainerAction(Key<TreeViewContainer> ContainerKey, TreeViewContainer Container);
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void AddChildNodeAction()
    {
        /*
        public record AddChildNodeAction(
            Key<TreeViewContainer> ContainerKey, TreeViewNoType ParentNode, TreeViewNoType ChildNode);
         */

        throw new NotImplementedException();
    }

    
    [Fact]
    public void ReRenderNodeAction()
    {
        /*
        public record ReRenderNodeAction(Key<TreeViewContainer> ContainerKey, TreeViewNoType Node);
         */

	    throw new NotImplementedException();
    }
    
    [Fact]
    public void SetActiveNodeAction()
    {
        /*
        public record SetActiveNodeAction(Key<TreeViewContainer> ContainerKey, TreeViewNoType? NextActiveNode);
         */

	    throw new NotImplementedException();
    }
    
    [Fact]
    public void AddSelectedNodeAction()
    {
        /*
        public record AddSelectedNodeAction(Key<TreeViewContainer> ContainerKey, TreeViewNoType SelectedNode);
         */

	    throw new NotImplementedException();
    }

    [Fact]
    public void RemoveSelectedNodeAction()
    {
        /*
        public record RemoveSelectedNodeAction(Key<TreeViewContainer> ContainerKey, Key<TreeViewNoType> NodeKey);
         */

        throw new NotImplementedException();
    }


    [Fact]
    public void ClearSelectedNodeBagAction()
    {
        /*
        public record ClearSelectedNodeBagAction(Key<TreeViewContainer> ContainerKey);
         */

        throw new NotImplementedException();
    }
    
    [Fact]
    public void MoveLeftAction()
    {
        /*
        public record MoveLeftAction(Key<TreeViewContainer> ContainerKey, bool ShiftKey);
     
         */

	    throw new NotImplementedException();
    }
    
    [Fact]
    public void MoveDownAction()
    {
        /*
        public record MoveDownAction(Key<TreeViewContainer> ContainerKey, bool ShiftKey);
         */

	    throw new NotImplementedException();
    }
    
    [Fact]
    public void MoveUpAction()
    {
        /*
        public record MoveUpAction(Key<TreeViewContainer> ContainerKey, bool ShiftKey);
         */

	    throw new NotImplementedException();
    }

    [Fact]
    public void MoveRightAction()
    {
        /*
        public record MoveRightAction(
            Key<TreeViewContainer> ContainerKey, bool ShiftKey, Action<TreeViewNoType> LoadChildBagAction);
         */

        throw new NotImplementedException();
    }

    
    [Fact]
    public void MoveHomeAction()
    {
        /*
        public record MoveHomeAction(Key<TreeViewContainer> ContainerKey, bool ShiftKey);
         */

	    throw new NotImplementedException();
    }
    
    [Fact]
    public void MoveEndAction()
    {
        /*
        public record MoveEndAction(Key<TreeViewContainer> ContainerKey, bool ShiftKey);
         */

	    throw new NotImplementedException();
    }

    [Fact]
    public void LoadChildBagAction()
    {
        /*
        public record LoadChildBagAction(Key<TreeViewContainer> ContainerKey, TreeViewNoType Node);
         */

        throw new NotImplementedException();
    }
}