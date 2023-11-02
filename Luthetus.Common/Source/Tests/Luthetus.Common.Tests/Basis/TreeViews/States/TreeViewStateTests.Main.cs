using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.TreeViews.Models;

namespace Luthetus.Common.RazorLib.TreeViews.States;

[FeatureState]
public partial record TreeViewStateTests(ImmutableList<TreeViewContainer> ContainerBag)
{
    private TreeViewState() : this(ImmutableList<TreeViewContainer>.Empty)
    {
    }
}