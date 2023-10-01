using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Ide.RazorLib.TreeViewImplementations.Models;

namespace Luthetus.Ide.RazorLib.TreeViewImplementations.Models;

public partial class TreeViewHelper
{
    public static Task<List<TreeViewNoType>> CSharpClassLoadChildrenAsync(
        this TreeViewNamespacePath cSharpClassTreeView)
    {
        if (cSharpClassTreeView.Item is null)
            return Task.FromResult<List<TreeViewNoType>>(new());

        return Task.FromResult(cSharpClassTreeView.ChildBag);
    }
}