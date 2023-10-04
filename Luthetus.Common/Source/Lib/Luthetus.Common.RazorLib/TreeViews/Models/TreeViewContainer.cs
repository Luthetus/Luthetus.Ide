using Luthetus.Common.RazorLib.Keys.Models;
using System.Collections.Immutable;

namespace Luthetus.Common.RazorLib.TreeViews.Models;

/// <summary>
/// TODO: SphagettiCode - some logic was added to multi-select nodes, yet it was never
/// finished, and is buggy.(2023-09-19)
/// </summary>
public record TreeViewContainer
{
    /// <summary>
    /// If <see cref="rootNode"/> is null then <see cref="TreeViewAdhoc.ConstructTreeViewAdhoc()"/>
    /// will be invoked and the return value will be used as the <see cref="RootNode"/>
    /// </summary>
    public TreeViewContainer(
        Key<TreeViewContainer> key,
        TreeViewNoType? rootNode,
        TreeViewNoType? activeNode,
        ImmutableList<TreeViewNoType> selectedNodeBag)
    {
        rootNode ??= TreeViewAdhoc.ConstructTreeViewAdhoc();

        Key = key;
        RootNode = rootNode;
        ActiveNode = activeNode;
        SelectedNodeBag = selectedNodeBag;
    }

    public Key<TreeViewContainer> Key { get; init; }
    public TreeViewNoType RootNode { get; init; }
    public TreeViewNoType? ActiveNode { get; init; }
    /// <summary>
    /// When <see cref="SelectedNodeBag"/> is Empty, then the <see cref="ActiveNode"/>
    /// will be treated as the single selected node.
    /// </summary>
    public ImmutableList<TreeViewNoType> SelectedNodeBag { get; init; }
    public Guid StateId { get; init; } = Guid.NewGuid();
}