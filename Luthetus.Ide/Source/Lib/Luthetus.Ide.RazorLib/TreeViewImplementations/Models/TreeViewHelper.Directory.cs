using Luthetus.Common.RazorLib.FileSystems.Models;
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

        var directoryPathStringsBag = await directoryTreeView.FileSystemProvider.Directory
            .GetDirectoriesAsync(directoryAbsolutePathString);

        var childDirectoryTreeViewModels = directoryPathStringsBag
            .OrderBy(pathString => pathString)
            .Select(x =>
            {
                var absolutePath = new AbsolutePath(x, true, directoryTreeView.EnvironmentProvider);

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

        var filePathStringsBag = await directoryTreeView.FileSystemProvider.Directory
            .GetFilesAsync(directoryAbsolutePathString);

        var childFileTreeViewModels = filePathStringsBag
            .OrderBy(pathString => pathString)
            .Select(x =>
            {
                var absolutePath = new AbsolutePath(x, false, directoryTreeView.EnvironmentProvider);
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

        var directoryPathStringsBag = await directoryTreeView.FileSystemProvider.Directory
            .GetDirectoriesAsync(directoryAbsolutePathString);

        var childDirectoryTreeViewModels = directoryPathStringsBag
            .OrderBy(pathString => pathString)
            .Select(x =>
            {
                return (TreeViewNoType)new TreeViewAbsolutePath(
                    new AbsolutePath(x, true, directoryTreeView.EnvironmentProvider),
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

        var filePathStringsBag = await directoryTreeView.FileSystemProvider.Directory
            .GetFilesAsync(directoryAbsolutePathString);

        var childFileTreeViewModels = filePathStringsBag
            .OrderBy(pathString => pathString)
            .Select(x =>
            {
                return (TreeViewNoType)new TreeViewAbsolutePath(
                    new AbsolutePath(x, false, directoryTreeView.EnvironmentProvider),
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