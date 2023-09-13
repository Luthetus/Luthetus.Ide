using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;

namespace Luthetus.Ide.RazorLib.TreeViewImplementationsCase.Helper;

public partial class TreeViewHelper
{
    public static Task<List<TreeViewNoType>> CSharpClassLoadChildrenAsync(
        this TreeViewNamespacePath cSharpClassTreeView)
    {
        if (cSharpClassTreeView.Item is null)
            return Task.FromResult<List<TreeViewNoType>>(new());

        return Task.FromResult(cSharpClassTreeView.Children);
    }
}