using Luthetus.CompilerServices.Lang.DotNetSolution.CSharp;
using Luthetus.Ide.RazorLib.FileSystemCase.Models;
using Luthetus.Common.RazorLib.FileSystem.Models;
using Luthetus.Common.RazorLib.TreeView.Models;
using Luthetus.Common.RazorLib.Namespaces.Models;
using Luthetus.Common.RazorLib.KeyCase;

namespace Luthetus.Ide.RazorLib.TreeViewImplementationsCase.Models;

public partial class TreeViewHelper
{
    public static async Task<List<TreeViewNoType>> CSharpProjectLoadChildrenAsync(
        this TreeViewNamespacePath cSharpProjectTreeView)
    {
        var parentDirectoryOfCSharpProject = (IAbsolutePath)
            cSharpProjectTreeView.Item.AbsolutePath.AncestorDirectories
                .Last();

        var parentAbsolutePathString = parentDirectoryOfCSharpProject.FormattedInput;

        var hiddenFiles = HiddenFileFacts
            .GetHiddenFilesByContainerFileExtension(ExtensionNoPeriodFacts.C_SHARP_PROJECT);

        var childDirectoryTreeViewModels =
            (await cSharpProjectTreeView.FileSystemProvider
                .Directory.GetDirectoriesAsync(parentAbsolutePathString))
                .OrderBy(pathString => pathString)
                .Where(x => hiddenFiles.All(hidden => !x.EndsWith(hidden)))
                .Select(x =>
                {
                    var absolutePath = new AbsolutePath(
                        x,
                        true,
                        cSharpProjectTreeView.EnvironmentProvider);

                    var namespaceString = cSharpProjectTreeView.Item.Namespace +
                                          NAMESPACE_DELIMITER +
                                          absolutePath.NameNoExtension;

                    return new TreeViewNamespacePath(
                        new NamespacePath(
                            namespaceString,
                            absolutePath),
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

        var uniqueDirectories = UniqueFileFacts
            .GetUniqueFilesByContainerFileExtension(
                ExtensionNoPeriodFacts.C_SHARP_PROJECT);

        var foundUniqueDirectories = new List<TreeViewNamespacePath>();
        var foundDefaultDirectories = new List<TreeViewNamespacePath>();

        foreach (var directoryTreeViewModel in childDirectoryTreeViewModels)
        {
            if (uniqueDirectories.Any(unique => directoryTreeViewModel
                    .Item.AbsolutePath.NameNoExtension == unique))
            {
                foundUniqueDirectories.Add(directoryTreeViewModel);
            }
            else
            {
                foundDefaultDirectories.Add(directoryTreeViewModel);
            }
        }

        foundUniqueDirectories = foundUniqueDirectories
            .OrderBy(x => x.Item.AbsolutePath.NameNoExtension)
            .ToList();

        foundDefaultDirectories = foundDefaultDirectories
            .OrderBy(x => x.Item.AbsolutePath.NameNoExtension)
            .ToList();

        var childFileTreeViewModels =
            (await cSharpProjectTreeView.FileSystemProvider
                .Directory.GetFilesAsync(parentAbsolutePathString))
                .OrderBy(pathString => pathString)
                .Where(x => !x.EndsWith(ExtensionNoPeriodFacts.C_SHARP_PROJECT))
                .Select(x =>
                {
                    var absolutePath = new AbsolutePath(
                        x,
                        false,
                        cSharpProjectTreeView.EnvironmentProvider);

                    var namespaceString = cSharpProjectTreeView.Item.Namespace;

                    return (TreeViewNoType)new TreeViewNamespacePath(
                        new NamespacePath(
                            namespaceString,
                            absolutePath),
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

        return
            new TreeViewNoType[] { cSharpProjectDependenciesTreeViewNode }
            .Union(foundUniqueDirectories)
            .Union(foundDefaultDirectories)
            .Union(childFileTreeViewModels)
            .ToList();
    }
}