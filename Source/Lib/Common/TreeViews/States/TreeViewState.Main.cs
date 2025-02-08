using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.TreeViews.Models;

namespace Luthetus.Common.RazorLib.TreeViews.States;

/// <summary>
/// The list provided should not be modified after passing it as a parameter.
/// Make a shallow copy, and pass the shallow copy, if further modification of your list will be necessary.
/// 
/// ---
///
/// Experimenting with 'List' instead of 'ImmutableList / ImmutableArray / Array'
/// Presumption being that in the case of 'ImmutableList / ImmutableArray'
/// the data would be stored as a tree structure, where each node is an object allocation.
/// That being said, I wonder if 'ImmutableList' and etc... are
/// trees that are made via a List that contains the nodes side by side and maybe tracks the length
/// of each sub-tree in order to traverse.
/// If it is done this way each node can more easily be a struct?
/// I want the C# parser to have the CompilationUnit be a struct foreach node. I'm not
/// sure how feasible that is I'm also super delirious at the moment.
/// Array.Empty issue.... store new List<T> somewhere and reference it?
/// but what if someone modifies it.
/// have property be IReadOnlyList and private List that is the?
/// </summary>
[FeatureState]
public partial record TreeViewState(List<TreeViewContainer> ContainerList)
{
    public TreeViewState() : this(new List<TreeViewContainer>())
    {
    }
}