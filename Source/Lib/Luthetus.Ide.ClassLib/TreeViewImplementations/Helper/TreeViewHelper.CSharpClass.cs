using Luthetus.Ide.ClassLib.FileConstants;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using Luthetus.Common.RazorLib.Namespaces;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Common.RazorLib.FileSystem.Classes.FilePath;

namespace Luthetus.Ide.ClassLib.TreeViewImplementations.Helper;

public partial class TreeViewHelper
{
    public static async Task<List<TreeViewNoType>> CSharpClassLoadChildrenAsync(
        this TreeViewNamespacePath cSharpClassTreeView)
    {
        if (cSharpClassTreeView.Item is null)
            return new();

        return cSharpClassTreeView.Children;
    }
}