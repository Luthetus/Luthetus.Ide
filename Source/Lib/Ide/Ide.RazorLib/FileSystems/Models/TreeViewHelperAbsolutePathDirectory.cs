using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;

namespace Luthetus.Ide.RazorLib.FileSystems.Models;

public class TreeViewHelperAbsolutePathDirectory
{
    public static async Task<List<TreeViewNoType>> LoadChildrenAsync(TreeViewAbsolutePath directoryTreeView)
    {
        var directoryAbsolutePathString = directoryTreeView.Item.Value;

        var directoryPathStringsList = await directoryTreeView.FileSystemProvider.Directory
            .GetDirectoriesAsync(directoryAbsolutePathString)
            .ConfigureAwait(false);

        var childDirectoryTreeViewModels = directoryPathStringsList
            .OrderBy(pathString => pathString)
            .Select(x =>
            {
                return (TreeViewNoType)new TreeViewAbsolutePath(
                    directoryTreeView.EnvironmentProvider.AbsolutePathFactory(x, true),
                    directoryTreeView.IdeComponentRenderers,
                    directoryTreeView.CommonComponentRenderers,
                    directoryTreeView.FileSystemProvider,
                    directoryTreeView.EnvironmentProvider,
                    true,
                    false)
                {
                    TreeViewChangedKey = Key<TreeViewChanged>.NewKey()
                };
            });

        var filePathStringsList = await directoryTreeView.FileSystemProvider.Directory
            .GetFilesAsync(directoryAbsolutePathString)
            .ConfigureAwait(false);

        var childFileTreeViewModels = filePathStringsList
            .OrderBy(pathString => pathString)
            .Select(x =>
            {
                return (TreeViewNoType)new TreeViewAbsolutePath(
                    directoryTreeView.EnvironmentProvider.AbsolutePathFactory(x, false),
                    directoryTreeView.IdeComponentRenderers,
                    directoryTreeView.CommonComponentRenderers,
                    directoryTreeView.FileSystemProvider,
                    directoryTreeView.EnvironmentProvider,
                    false,
                    false)
                {
                    TreeViewChangedKey = Key<TreeViewChanged>.NewKey()
                };
            });

        return childDirectoryTreeViewModels.Union(childFileTreeViewModels).ToList();
    }
}