using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.TreeViews.Models;

/// <summary>
/// Without this datatype one cannot for example hold all their <see cref="TreeViewWithType{T}"/> in a <see cref="List{T}"/> unless
/// all implementation instances share the same generic argument type.
/// </summary>
public abstract class TreeViewNoType
{
    public abstract object UntypedItem { get; }
    public abstract Type ItemType { get; }

    public TreeViewNoType? Parent { get; set; }
    public List<TreeViewNoType> ChildBag { get; set; } = new();

    /// <summary>
    /// <see cref="IndexAmongSiblings"/> refers to the index which this <see cref="TreeViewNoType"/>
    /// is found at within their <see cref="Parent"/>'s <see cref="ChildBag"/>
    /// </summary>
    public int IndexAmongSiblings { get; set; }
    public bool IsRoot { get; set; }
    public bool IsHidden { get; set; }
    public bool IsExpandable { get; set; }
    public bool IsExpanded { get; set; }
    public Key<TreeViewChanged> TreeViewChangedKey { get; set; } = Key<TreeViewChanged>.NewKey();
    public Key<TreeViewNoType> Key { get; set; } = Key<TreeViewNoType>.NewKey();

    public abstract TreeViewRenderer GetTreeViewRenderer();
    /// <summary>
    /// TODO: SphagettiCode - <see cref="LoadChildBagAsync"/> has the same logic over
    /// and over prior to returning. That is, 'foreach (var newChild in Children)'.
    /// Inside the foreach a check for a previous instance is made so the IsExpanded state and
    /// etc... can be remembered. This logic should be generalized into a reusable method?(2023-09-19)
    /// </summary>
    public abstract Task LoadChildBagAsync();

    /// <summary>
    /// <see cref="LinkChildren"/> will set foreach child: child.Parent = this;
    /// As well it sets the child.IndexAmongSiblings, and maintains expanded state.
    /// </summary>
    public virtual void LinkChildren(
        List<TreeViewNoType> previousChildren,
        List<TreeViewNoType> nextChildren)
    {
        var oldChildrenMap = previousChildren.ToDictionary(child => child);

        for (int i = 0; i < nextChildren.Count; i++)
        {
            var child = nextChildren[i];

            child.Parent = this;
            child.IndexAmongSiblings = i;
        }

        foreach (var newChild in nextChildren)
        {
            if (oldChildrenMap.TryGetValue(newChild, out var oldChild))
            {
                newChild.IsExpanded = oldChild.IsExpanded;
                newChild.IsExpandable = oldChild.IsExpandable;
                newChild.IsHidden = oldChild.IsHidden;
                newChild.Key = oldChild.Key;
                newChild.ChildBag = oldChild.ChildBag;
            }
        }
    }

    /// <summary>
    /// <see cref="RemoveRelatedFilesFromParent"/> is used for showing codebehinds such that a file on
    /// the filesystem can be displayed as having children in the TreeView.<br/><br/>
    /// In the case of a directory loading its children. After the directory loads all its children it
    /// will loop through the children invoking <see cref="RemoveRelatedFilesFromParent"/> on each of
    /// the children.<br/><br/>
    /// For example: if a directory has the children { 'Component.razor', 'Component.razor.cs' }  then
    /// 'Component.razor' will remove 'Component.razor.cs' from the parent directories children and
    /// mark itself as expandable as it saw a related file in its parent.
    /// </summary>
    public virtual void RemoveRelatedFilesFromParent(List<TreeViewNoType> siblingsAndSelfTreeViews)
    {
        // The default implementation of this method is to do nothing.
        // Override this method to implement some functionality if desired.
    }
}