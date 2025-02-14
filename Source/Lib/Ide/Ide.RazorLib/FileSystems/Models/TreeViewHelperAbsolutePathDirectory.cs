using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;

namespace Luthetus.Ide.RazorLib.FileSystems.Models;

public class TreeViewHelperAbsolutePathDirectory
{
    public static async Task<List<TreeViewNoType>> LoadChildrenAsync(TreeViewAbsolutePath directoryTreeView)
    {
        var directoryAbsolutePathString = directoryTreeView.Item.Value;

        var directoryPathStringsList = await directoryTreeView.CommonApi.FileSystemProviderApi.Directory
            .GetDirectoriesAsync(directoryAbsolutePathString)
            .ConfigureAwait(false);

        var childDirectoryTreeViewModels = directoryPathStringsList
            .OrderBy(pathString => pathString)
            .Select(x =>
            {
                return (TreeViewNoType)new TreeViewAbsolutePath(
                    directoryTreeView.CommonApi.EnvironmentProviderApi.AbsolutePathFactory(x, true),
                    directoryTreeView.CommonApi,
                    directoryTreeView.IdeComponentRenderers,
                    true,
                    false)
                {
                    TreeViewChangedKey = Key<TreeViewChanged>.NewKey()
                };
            });

        var filePathStringsList = await directoryTreeView.CommonApi.FileSystemProviderApi.Directory
            .GetFilesAsync(directoryAbsolutePathString)
            .ConfigureAwait(false);

        var childFileTreeViewModels = filePathStringsList
            .OrderBy(pathString => pathString)
            .Select(x =>
            {
                return (TreeViewNoType)new TreeViewAbsolutePath(
                    directoryTreeView.CommonApi.EnvironmentProviderApi.AbsolutePathFactory(x, false),
                    directoryTreeView.CommonApi,
                    directoryTreeView.IdeComponentRenderers,
                    false,
                    false)
                {
                    TreeViewChangedKey = Key<TreeViewChanged>.NewKey()
                };
            });

        return childDirectoryTreeViewModels.Union(childFileTreeViewModels).ToList();
    }
}