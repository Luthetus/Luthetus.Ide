using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;

namespace Luthetus.Extensions.DotNet.Outputs.Models;

public record struct OutputState(Guid DotNetRunParseResultId)
{
	public static readonly Key<TreeViewContainer> TreeViewContainerKey = Key<TreeViewContainer>.NewKey();
	
	public OutputState() : this(Guid.Empty)
	{
	}
}
