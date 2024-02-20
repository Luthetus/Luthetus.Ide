using Luthetus.Common.RazorLib.TreeViews.Models;

namespace Luthetus.Ide.Tests.Basis.TreeViewImplementations.Models;

public partial class TreeViewHelperCSharpClassTests
{
    public static Task<List<TreeViewNoType>> CSharpClassLoadChildrenAsync(
        this TreeViewNamespacePath cSharpClassTreeView)
    {
        if (cSharpClassTreeView.Item is null)
            return Task.FromResult<List<TreeViewNoType>>(new());

        return Task.FromResult(cSharpClassTreeView.ChildList);
    }
}