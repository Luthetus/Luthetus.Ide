using Luthetus.Ide.ClassLib.FileConstants;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Common.RazorLib.FileSystem.Classes.FilePath;
using Luthetus.Common.RazorLib.Namespaces;
using Luthetus.CompilerServices.Lang.DotNetSolution.CSharp;

namespace Luthetus.Ide.ClassLib.TreeViewImplementations.Helper;

public partial class TreeViewHelper
{
    public static async Task<List<TreeViewNoType>> CSharpProjectLoadChildrenAsync(
        this TreeViewNamespacePath cSharpProjectTreeView)
    {
        var parentDirectoryOfCSharpProject = (IAbsoluteFilePath)
            cSharpProjectTreeView.Item.AbsoluteFilePath.AncestorDirectories
                .Last();

        var parentAbsoluteFilePathString = parentDirectoryOfCSharpProject.FormattedInput;

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
                        cSharpProjectTreeView.LuthetusCommonComponentRenderers,
                        cSharpProjectTreeView.FileSystemProvider,
                        cSharpProjectTreeView.EnvironmentProvider,
                        true,
                        false)
                    {
                        TreeViewChangedKey = TreeViewChangedKey.NewKey()
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
            .OrderBy(x => x.Item.AbsoluteFilePath.FileNameNoExtension)
            .ToList();

        foundDefaultDirectories = foundDefaultDirectories
            .OrderBy(x => x.Item.AbsoluteFilePath.FileNameNoExtension)
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
                        cSharpProjectTreeView.LuthetusCommonComponentRenderers,
                        cSharpProjectTreeView.FileSystemProvider,
                        cSharpProjectTreeView.EnvironmentProvider,
                        false,
                        false)
                    {
                        TreeViewChangedKey = TreeViewChangedKey.NewKey()
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
            TreeViewChangedKey = TreeViewChangedKey.NewKey()
        };

        return
            new TreeViewNoType[] { cSharpProjectDependenciesTreeViewNode }
            .Union(foundUniqueDirectories)
            .Union(foundDefaultDirectories)
            .Union(childFileTreeViewModels)
            .ToList();
    }
}