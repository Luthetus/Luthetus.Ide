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
        ImmutableList<TreeViewNoType> selectedNodeList)
    {
		/*
			Here in the constructor we can see that there is a list that contains all the selected nodes.
			
			If there is more than 1 selected nodes, perhaps we'd combine the outputs, but for now
			    we can just render the text '> 1 nodes are selected'.

			If there is just 1 node, we can look at the Item on the treeview node.
			I say this with prior knowledge of what the treeview node class looks like,
			so I should bring up that class.
		*/

        rootNode ??= TreeViewAdhoc.ConstructTreeViewAdhoc();

        Key = key;
        RootNode = rootNode;
        SelectedNodeList = selectedNodeList;
    }

    public Key<TreeViewContainer> Key { get; init; }
    public TreeViewNoType RootNode { get; init; }
    /// <summary>
    /// The <see cref="ActiveNode"/> is the last or default entry in <see cref="SelectedNodeList"/>
    /// </summary>
    public TreeViewNoType? ActiveNode => SelectedNodeList.FirstOrDefault();
    public ImmutableList<TreeViewNoType> SelectedNodeList { get; init; }
    public Guid StateId { get; init; } = Guid.NewGuid();
}