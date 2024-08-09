using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;

namespace Luthetus.Extensions.DotNet.Outputs.States;

[FeatureState]
public partial record OutputState(Guid DotNetRunParseResultId)
{
	public static readonly Key<TreeViewContainer> TreeViewContainerKey = Key<TreeViewContainer>.NewKey();
	
	public OutputState() : this(Guid.Empty)
	{
	}
}
