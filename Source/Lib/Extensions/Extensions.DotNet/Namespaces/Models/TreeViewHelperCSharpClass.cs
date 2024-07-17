using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Ide.RazorLib.Namespaces.Models;

namespace Luthetus.Extensions.DotNet.Namespaces.Models;

public class TreeViewHelperCSharpClass
{
	public static Task<List<TreeViewNoType>> LoadChildrenAsync(TreeViewNamespacePath cSharpClassTreeView)
	{
		if (cSharpClassTreeView.Item is null)
			return Task.FromResult<List<TreeViewNoType>>(new());

		return Task.FromResult(cSharpClassTreeView.ChildList);
	}
}