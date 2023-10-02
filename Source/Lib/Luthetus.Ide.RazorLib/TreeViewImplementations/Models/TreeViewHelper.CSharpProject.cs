using Luthetus.CompilerServices.Lang.DotNetSolution.CSharp;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Namespaces.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.FileSystems.Models;

namespace Luthetus.Ide.RazorLib.TreeViewImplementations.Models;

public partial class TreeViewHelper
{
    public static async Task<List<TreeViewNoType>> CSharpProjectLoadChildrenAsync(
        this TreeViewNamespacePath cSharpProjectTreeView)
    {
        var parentDirectoryOfCSharpProject = cSharpProjectTreeView.Item.AbsolutePath.AncestorDirectoryBag.Last();
        var parentAbsolutePathString = parentDirectoryOfCSharpProject.FormattedInput;
        var hiddenFiles = HiddenFileFacts.GetHiddenFilesByContainerFileExtension(ExtensionNoPeriodFacts.C_SHARP_PROJECT);

        var directoryPathStringsBag = await cSharpProjectTreeView.FileSystemProvider.Directory
            .GetDirectoriesAsync(parentAbsolutePathString);

        var childDirectoryTreeViewModelsBag = directoryPathStringsBag
            .OrderBy(pathString => pathString)
            .Where(x => hiddenFiles.All(hidden => !x.EndsWith(hidden)))
            .Select(x =>
            {
                var absolutePath = new AbsolutePath(x, true, cSharpProjectTreeView.EnvironmentProvider);

                var namespaceString = cSharpProjectTreeView.Item.Namespace +
                    NAMESPACE_DELIMITER +
                    absolutePath.NameNoExtension;

                return new TreeViewNamespacePath(new NamespacePath(namespaceString, absolutePath),
                    cSharpProjectTreeView.LuthetusIdeComponentRenderers,
                    cSharpProjectTreeView.LuthetusCommonComponentRenderers,
                    cSharpProjectTreeView.FileSystemProvider,
                    cSharpProjectTreeView.EnvironmentProvider,
                    true,
                    false)
                {
                    TreeViewChangedKey = Key<TreeViewChanged>.NewKey()
                };
            });

        var uniqueDirectories = UniqueFileFacts.GetUniqueFilesByContainerFileExtension(ExtensionNoPeriodFacts.C_SHARP_PROJECT);
        var foundUniqueDirectories = new List<TreeViewNamespacePath>();
        var foundDefaultDirectories = new List<TreeViewNamespacePath>();

        foreach (var directoryTreeViewModel in childDirectoryTreeViewModelsBag)
        {
            if (uniqueDirectories.Any(unique => directoryTreeViewModel.Item.AbsolutePath.NameNoExtension == unique))
                foundUniqueDirectories.Add(directoryTreeViewModel);
            else
                foundDefaultDirectories.Add(directoryTreeViewModel);
        }

        foundUniqueDirectories = foundUniqueDirectories.OrderBy(x => x.Item.AbsolutePath.NameNoExtension).ToList();
        foundDefaultDirectories = foundDefaultDirectories.OrderBy(x => x.Item.AbsolutePath.NameNoExtension).ToList();

        var filePathStringsBag = await cSharpProjectTreeView.FileSystemProvider.Directory
            .GetFilesAsync(parentAbsolutePathString);

        var childFileTreeViewModels = filePathStringsBag
            .OrderBy(pathString => pathString)
            .Where(x => !x.EndsWith(ExtensionNoPeriodFacts.C_SHARP_PROJECT))
            .Select(x =>
            {
                var absolutePath = new AbsolutePath(x, false, cSharpProjectTreeView.EnvironmentProvider);
                var namespaceString = cSharpProjectTreeView.Item.Namespace;

                return (TreeViewNoType)new TreeViewNamespacePath(new NamespacePath(namespaceString, absolutePath),
                    cSharpProjectTreeView.LuthetusIdeComponentRenderers,
                    cSharpProjectTreeView.LuthetusCommonComponentRenderers,
                    cSharpProjectTreeView.FileSystemProvider,
                    cSharpProjectTreeView.EnvironmentProvider,
                    false,
                    false)
                {
                    TreeViewChangedKey = Key<TreeViewChanged>.NewKey()
                };
            });

        var cSharpProjectDependenciesTreeViewNode = new TreeViewCSharpProjectDependencies(
            new CSharpProjectDependencies(cSharpProjectTreeView.Item),
            cSharpProjectTreeView.LuthetusIdeComponentRenderers,
            cSharpProjectTreeView.FileSystemProvider,
            cSharpProjectTreeView.EnvironmentProvider,
            true,
            false)
        {
            TreeViewChangedKey = Key<TreeViewChanged>.NewKey()
        };

        return new TreeViewNoType[] { cSharpProjectDependenciesTreeViewNode }
            .Union(foundUniqueDirectories)
            .Union(foundDefaultDirectories)
            .Union(childFileTreeViewModels)
            .ToList();
    }
}