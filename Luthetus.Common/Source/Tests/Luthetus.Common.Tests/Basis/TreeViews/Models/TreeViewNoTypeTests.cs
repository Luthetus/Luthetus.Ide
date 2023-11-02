using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.TreeViews.Models;

public abstract class TreeViewNoTypeTests
{
    [Fact]
    public void UntypedItem()
    {
        /*
        public abstract object UntypedItem { get; }
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void ItemType()
    {
        /*
        public abstract Type ItemType { get; }
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void Parent()
    {
        /*
        public TreeViewNoType? Parent { get; set; }
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void ChildBag()
    {
        /*
        public List<TreeViewNoType> ChildBag { get; set; } = new();
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void IndexAmongSiblings()
    {
        /*
        public int IndexAmongSiblings { get; set; }
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void IsRoot()
    {
        /*
        public bool IsRoot { get; set; }
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void IsHidden()
    {
        /*
        public bool IsHidden { get; set; }
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void IsExpandable()
    {
        /*
        public bool IsExpandable { get; set; }
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void IsExpanded()
    {
        /*
        public bool IsExpanded { get; set; }
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void TreeViewChangedKey()
    {
        /*
        public Key<TreeViewChanged> TreeViewChangedKey { get; set; } = Key<TreeViewChanged>.NewKey();
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void Key()
    {
        /*
        public Key<TreeViewNoType> Key { get; set; } = Key<TreeViewNoType>.NewKey();
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void GetTreeViewRenderer()
    {
        /*
        public abstract TreeViewRenderer GetTreeViewRenderer();
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void LoadChildBagAsync()
    {
        /*
        public abstract Task LoadChildBagAsync();
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void RemoveRelatedFilesFromParent()
    {
        /*
        public virtual void RemoveRelatedFilesFromParent(List<TreeViewNoType> siblingsAndSelfTreeViews)
         */

        throw new NotImplementedException();
    }
}