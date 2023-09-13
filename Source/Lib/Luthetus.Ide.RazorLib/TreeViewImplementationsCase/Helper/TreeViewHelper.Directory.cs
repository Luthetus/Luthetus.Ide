using Luthetus.Common.RazorLib.FileSystem.Classes.LuthetusPath;
using Luthetus.Common.RazorLib.Namespaces;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using Luthetus.Ide.RazorLib.TreeViewImplementationsCase;

namespace Luthetus.Ide.ClassLib.TreeViewImplementationsCase.Helper;

public partial class TreeViewHelper
{
    public static async Task<List<TreeViewNoType>> DirectoryLoadChildrenAsync(
        this TreeViewNamespacePath directoryTreeView)
    {
        var directoryAbsolutePathString = directoryTreeView.Item.AbsolutePath
            .FormattedInput;

        var childDirectoryTreeViewModels =
            (await directoryTreeView.FileSystemProvider
                .Directory.GetDirectoriesAsync(directoryAbsolutePathString))
                .OrderBy(pathString => pathString)
                .Select(x =>
                {
                    var absolutePath = new AbsolutePath(
                        x,
                        true,
                        directoryTreeView.EnvironmentProvider);

                    var namespaceString = directoryTreeView.Item.Namespace +
                                          NAMESPACE_DELIMITER +
                                          absolutePath.NameNoExtension;

                    var namespacePath = new NamespacePath(
                        namespaceString,
                        absolutePath);

                    return (TreeViewNoType)new TreeViewNamespacePath(
                        namespacePath,
                        directoryTreeView.LuthetusIdeComponentRenderers,
                        directoryTreeView.LuthetusCommonComponentRenderers,
                        directoryTreeView.FileSystemProvider,
                        directoryTreeView.EnvironmentProvider,
                        true,
                        false)
                    {
                        TreeViewChangedKey = TreeViewChangedKey.NewKey()
                    };
                });

        var childFileTreeViewModels =
            (await directoryTreeView.FileSystemProvider
                .Directory.GetFilesAsync(directoryAbsolutePathString))
                .OrderBy(pathString => pathString)
                .Select(x =>
                {
                    var absolutePath = new AbsolutePath(
                        x,
                        false,
                        directoryTreeView.EnvironmentProvider);

                    var namespaceString = directoryTreeView.Item.Namespace;

                    var namespacePath = new NamespacePath(
                        namespaceString,
                        absolutePath);

                    return (TreeViewNoType)new TreeViewNamespacePath(
                        namespacePath,
                        directoryTreeView.LuthetusIdeComponentRenderers,
                        directoryTreeView.LuthetusCommonComponentRenderers,
                        directoryTreeView.FileSystemProvider,
                        directoryTreeView.EnvironmentProvider,
                        false,
                        false)
                    {
                        TreeViewChangedKey = TreeViewChangedKey.NewKey()
                    };
                }).ToList();

        var copyOfChildrenToFindRelatedFiles = new List<TreeViewNoType>(childFileTreeViewModels);

        foreach (var child in childFileTreeViewModels)
        {
            child.RemoveRelatedFilesFromParent(
                copyOfChildrenToFindRelatedFiles);
        }

        // The parent directory gets what is left over after the
        // children take their respective 'code behinds'
        childFileTreeViewModels = copyOfChildrenToFindRelatedFiles;

        return childDirectoryTreeViewModels
            .Union(childFileTreeViewModels)
            .ToList();
    }

    public static async Task<List<TreeViewNoType>> LoadChildrenForDirectoryAsync(
        TreeViewAbsolutePath directoryTreeView)
    {
        var directoryAbsolutePathString = directoryTreeView.Item
            .FormattedInput;

        var childDirectoryTreeViewModels =
            (await directoryTreeView.FileSystemProvider
                .Directory.GetDirectoriesAsync(directoryAbsolutePathString))
                .OrderBy(pathString => pathString)
                .Select(x =>
                {
                    var absolutePath = new AbsolutePath(
                        x,
                        true,
                        directoryTreeView.EnvironmentProvider);

                    return (TreeViewNoType)new TreeViewAbsolutePath(
                        absolutePath,
                        directoryTreeView.LuthetusIdeComponentRenderers,
                        directoryTreeView.LuthetusCommonComponentRenderers,
                        directoryTreeView.FileSystemProvider,
                        directoryTreeView.EnvironmentProvider,
                        true,
                        false)
                    {
                        TreeViewChangedKey = TreeViewChangedKey.NewKey()
                    };
                });

        var childFileTreeViewModels =
            (await directoryTreeView.FileSystemProvider
                .Directory.GetFilesAsync(directoryAbsolutePathString))
                .OrderBy(pathString => pathString)
                .Select(x =>
                {
                    var absolutePath = new AbsolutePath(
                        x,
                        false,
                        directoryTreeView.EnvironmentProvider);

                    return (TreeViewNoType)new TreeViewAbsolutePath(
                        absolutePath,
                        directoryTreeView.LuthetusIdeComponentRenderers,
                        directoryTreeView.LuthetusCommonComponentRenderers,
                        directoryTreeView.FileSystemProvider,
                        directoryTreeView.EnvironmentProvider,
                        false,
                        false)
                    {
                        TreeViewChangedKey = TreeViewChangedKey.NewKey()
                    };
                });

        return childDirectoryTreeViewModels
            .Union(childFileTreeViewModels)
            .ToList();
    }
}