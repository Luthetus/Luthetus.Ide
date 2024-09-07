using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Namespaces.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Extensions.DotNet.Namespaces.Models;

namespace Luthetus.Ide.RazorLib.Namespaces.Models;

public class TreeViewHelperNamespacePathDirectory
{
    /// <summary>Used with <see cref="TreeViewNamespacePath"/></summary>
    public static async Task<List<TreeViewNoType>> LoadChildrenAsync(TreeViewNamespacePath directoryTreeView)
    {
        var directoryAbsolutePathString = directoryTreeView.Item.AbsolutePath.Value;

        var directoryPathStringsList = await directoryTreeView.FileSystemProvider.Directory
            .GetDirectoriesAsync(directoryAbsolutePathString)
            .ConfigureAwait(false);

        var childDirectoryTreeViewModels = directoryPathStringsList
            .OrderBy(pathString => pathString)
            .Select(x =>
            {
                var absolutePath = directoryTreeView.EnvironmentProvider.AbsolutePathFactory(x, true);

                var namespaceString = directoryTreeView.Item.Namespace +
                    TreeViewNamespaceHelper.NAMESPACE_DELIMITER +
                    absolutePath.NameNoExtension;

                return (TreeViewNoType)new TreeViewNamespacePath(
                    new NamespacePath(namespaceString, absolutePath),
                    directoryTreeView.DotNetComponentRenderers,
                    directoryTreeView.IdeComponentRenderers,
                    directoryTreeView.CommonComponentRenderers,
                    directoryTreeView.FileSystemProvider,
                    directoryTreeView.EnvironmentProvider,
                    true,
                    false);
            });

        var filePathStringsList = await directoryTreeView.FileSystemProvider.Directory
            .GetFilesAsync(directoryAbsolutePathString)
            .ConfigureAwait(false);

        var childFileTreeViewModels = filePathStringsList
            .OrderBy(pathString => pathString)
            .Select(x =>
            {
                var absolutePath = directoryTreeView.EnvironmentProvider.AbsolutePathFactory(x, false);
                var namespaceString = directoryTreeView.Item.Namespace;

                return (TreeViewNoType)new TreeViewNamespacePath(
                    new NamespacePath(namespaceString, absolutePath),
                    directoryTreeView.DotNetComponentRenderers,
                    directoryTreeView.IdeComponentRenderers,
                    directoryTreeView.CommonComponentRenderers,
                    directoryTreeView.FileSystemProvider,
                    directoryTreeView.EnvironmentProvider,
                    false,
                    false);
            }).ToList();

		var result = new List<TreeViewNoType>();
		result.AddRange(childDirectoryTreeViewModels);
		result.AddRange(childFileTreeViewModels);
		
        return result;
    }
}
