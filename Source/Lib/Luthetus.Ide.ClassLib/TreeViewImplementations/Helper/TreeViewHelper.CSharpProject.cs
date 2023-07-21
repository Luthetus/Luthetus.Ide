using Luthetus.Ide.ClassLib.DotNet.CSharp;
using Luthetus.Ide.ClassLib.FileSystem.Classes.FilePath;
using Luthetus.Ide.ClassLib.Namespaces;
using Luthetus.Ide.ClassLib.FileConstants;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;

namespace Luthetus.Ide.ClassLib.TreeViewImplementations.Helper;

public partial class TreeViewHelper
{
    public static async Task<List<TreeViewNoType>> LoadChildrenForCSharpProjectAsync(
        TreeViewNamespacePath cSharpProjectTreeView)
    {
        if (cSharpProjectTreeView.Item is null)
            return new();

        var parentDirectoryOfCSharpProject = (IAbsoluteFilePath)
            cSharpProjectTreeView.Item.AbsoluteFilePath.Directories
                .Last();

        var parentAbsoluteFilePathString = parentDirectoryOfCSharpProject
            .GetAbsoluteFilePathString();

        var hiddenFiles = HiddenFileFacts
            .GetHiddenFilesByContainerFileExtension(ExtensionNoPeriodFacts.C_SHARP_PROJECT);

        var childDirectoryTreeViewModels =
            (await cSharpProjectTreeView.FileSystemProvider
                .Directory.GetDirectoriesAsync(parentAbsoluteFilePathString))
                .OrderBy(filePathString => filePathString)
                .Where(x => hiddenFiles.All(hidden => !x.EndsWith(hidden)))
                .Select(x =>
                {
                    var absoluteFilePath = new AbsoluteFilePath(
                        x,
                        true,
                        cSharpProjectTreeView.EnvironmentProvider);

                    var namespaceString = cSharpProjectTreeView.Item.Namespace +
                                          NAMESPACE_DELIMITER +
                                          absoluteFilePath.FileNameNoExtension;

                    return new TreeViewNamespacePath(
                        new NamespacePath(
                            namespaceString,
                            absoluteFilePath),
                        cSharpProjectTreeView.LuthetusIdeComponentRenderers,
                        cSharpProjectTreeView.FileSystemProvider,
                        cSharpProjectTreeView.EnvironmentProvider,
                        true,
                        false)
                    {
                        TreeViewChangedKey = TreeViewChangedKey.NewTreeViewChangedKey()
                    };
                });

        var uniqueDirectories = UniqueFileFacts
            .GetUniqueFilesByContainerFileExtension(
                ExtensionNoPeriodFacts.C_SHARP_PROJECT);

        var foundUniqueDirectories = new List<TreeViewNamespacePath>();
        var foundDefaultDirectories = new List<TreeViewNamespacePath>();

        foreach (var directoryTreeViewModel in childDirectoryTreeViewModels)
        {
            if (directoryTreeViewModel.Item is null)
                continue;

            if (uniqueDirectories.Any(unique => directoryTreeViewModel
                    .Item.AbsoluteFilePath.FileNameNoExtension == unique))
            {
                foundUniqueDirectories.Add(directoryTreeViewModel);
            }
            else
            {
                foundDefaultDirectories.Add(directoryTreeViewModel);
            }
        }

        foundUniqueDirectories = foundUniqueDirectories
            .OrderBy(x => x.Item?.AbsoluteFilePath.FileNameNoExtension ?? string.Empty)
            .ToList();

        foundDefaultDirectories = foundDefaultDirectories
            .OrderBy(x => x.Item?.AbsoluteFilePath.FileNameNoExtension ?? string.Empty)
            .ToList();

        var childFileTreeViewModels =
            (await cSharpProjectTreeView.FileSystemProvider
                .Directory.GetFilesAsync(parentAbsoluteFilePathString))
                .OrderBy(filePathString => filePathString)
                .Where(x => !x.EndsWith(ExtensionNoPeriodFacts.C_SHARP_PROJECT))
                .Select(x =>
                {
                    var absoluteFilePath = new AbsoluteFilePath(
                        x,
                        false,
                        cSharpProjectTreeView.EnvironmentProvider);

                    var namespaceString = cSharpProjectTreeView.Item.Namespace;

                    return (TreeViewNoType)new TreeViewNamespacePath(
                        new NamespacePath(
                            namespaceString,
                            absoluteFilePath),
                        cSharpProjectTreeView.LuthetusIdeComponentRenderers,
                        cSharpProjectTreeView.FileSystemProvider,
                        cSharpProjectTreeView.EnvironmentProvider,
                        false,
                        false)
                    {
                        TreeViewChangedKey = TreeViewChangedKey.NewTreeViewChangedKey()
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
            TreeViewChangedKey = TreeViewChangedKey.NewTreeViewChangedKey()
        };

        return
            new TreeViewNoType[] { cSharpProjectDependenciesTreeViewNode }
            .Union(foundUniqueDirectories)
            .Union(foundDefaultDirectories)
            .Union(childFileTreeViewModels)
            .ToList();
    }
}