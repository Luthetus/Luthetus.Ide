using System.Collections.Immutable;
using Fluxor;
using Luthetus.Common.RazorLib.TreeViews.Models;

namespace Luthetus.Common.RazorLib.TreeViews.States;

[FeatureState]
public partial record TreeViewState(List<TreeViewContainer> ContainerList)
{
    public TreeViewState() : this(new List<TreeViewContainer>())
    {
    }
}