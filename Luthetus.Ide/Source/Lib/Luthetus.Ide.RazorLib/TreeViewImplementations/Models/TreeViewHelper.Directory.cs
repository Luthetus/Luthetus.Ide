using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Namespaces.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;

namespace Luthetus.Ide.RazorLib.TreeViewImplementations.Models;

public partial class TreeViewHelper
{
    /// <summary>Used with <see cref="TreeViewNamespacePath"/></summary>
    public static async Task<List<TreeViewNoType>> DirectoryLoadChildrenAsync(
        this TreeViewNamespacePath directoryTreeView)
    {
        var directoryAbsolutePathString = directoryTreeView.Item.AbsolutePath.Value;

        var directoryPathStringsList = await directoryTreeView.FileSystemProvider.Directory
            .GetDirectoriesAsync(directoryAbsolutePathString);

        var childDirectoryTreeViewModels = directoryPathStringsList
            .OrderBy(pathString => pathString)
            .Select(x =>
            {
                var absolutePath = directoryTreeView.EnvironmentProvider.AbsolutePathFactory(x, true);

                var namespaceString = directoryTreeView.Item.Namespace +
                    NAMESPACE_DELIMITER +
                    absolutePath.NameNoExtension;

                return (TreeViewNoType)new TreeViewNamespacePath(
                    new NamespacePath(namespaceString, absolutePath),
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
            .GetFilesAsync(directoryAbsolutePathString);

        var childFileTreeViewModels = filePathStringsList
            .OrderBy(pathString => pathString)
            .Select(x =>
            {
                var absolutePath = directoryTreeView.EnvironmentProvider.AbsolutePathFactory(x, false);
                var namespaceString = directoryTreeView.Item.Namespace;

                return (TreeViewNoType)new TreeViewNamespacePath(
                    new NamespacePath(namespaceString, absolutePath),
                    directoryTreeView.IdeComponentRenderers,
                    directoryTreeView.CommonComponentRenderers,
                    directoryTreeView.FileSystemProvider,
                    directoryTreeView.EnvironmentProvider,
                    false,
                    false)
                {
                    TreeViewChangedKey = Key<TreeViewChanged>.NewKey()
                };
            }).ToList();

        var copyOfChildrenToFindRelatedFiles = new List<TreeViewNoType>(childFileTreeViewModels);

        foreach (var child in childFileTreeViewModels)
        {
            child.RemoveRelatedFilesFromParent(copyOfChildrenToFindRelatedFiles);
        }

        // The parent directory gets what is left over after the
        // children take their respective 'code behinds'
        childFileTreeViewModels = copyOfChildrenToFindRelatedFiles;

        return childDirectoryTreeViewModels.Union(childFileTreeViewModels).ToList();
    }

    /// <summary>Used with <see cref="TreeViewAbsolutePath"/></summary>
    public static async Task<List<TreeViewNoType>> LoadChildrenForDirectoryAsync(
        TreeViewAbsolutePath directoryTreeView)
    {
        var directoryAbsolutePathString = directoryTreeView.Item.Value;

        var directoryPathStringsList = await directoryTreeView.FileSystemProvider.Directory
            .GetDirectoriesAsync(directoryAbsolutePathString);

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
            .GetFilesAsync(directoryAbsolutePathString);

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