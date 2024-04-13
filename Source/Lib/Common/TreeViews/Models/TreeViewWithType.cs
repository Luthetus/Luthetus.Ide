namespace Luthetus.Common.RazorLib.TreeViews.Models;

/// <summary>
/// Implement the abstract class <see cref="TreeViewWithType{T}"/> in order to make a TreeView.<br/><br/>
/// An abstract class is used because a good deal of customization is required on a per
/// TreeView basis depending on what data type one displays in that TreeView.
/// </summary>
public abstract class TreeViewWithType<T> : TreeViewNoType where T : notnull
{
    public TreeViewWithType(T item, bool isExpandable, bool isExpanded)
    {
        Item = item;
        IsExpandable = isExpandable;
        IsExpanded = isExpanded;
    }

    /// <summary>
    /// Do not allow <see cref="Item"/> to be null.<br/><br/>
    /// If one wishes to have a 'null' concept as a TreeViewNode, then 
    /// make a datatype that acts as a psuedo null.
    /// </summary>
    public T Item { get; }
    public override object UntypedItem => Item;
    public override Type ItemType => typeof(T);
}