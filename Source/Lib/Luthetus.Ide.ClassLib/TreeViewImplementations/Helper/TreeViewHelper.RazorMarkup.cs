using Luthetus.Ide.ClassLib.FileConstants;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using Luthetus.Common.RazorLib.Namespaces;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Common.RazorLib.FileSystem.Classes.FilePath;

namespace Luthetus.Ide.ClassLib.TreeViewImplementations.Helper;

public partial class TreeViewHelper
{
    public static async Task<List<TreeViewNoType>> RazorMarkupLoadChildrenAsync(
        this TreeViewNamespacePath razorMarkupTreeView)
    {
        if (razorMarkupTreeView.Item is null)
            return new();

        var parentDirectoryOfRazorMarkup = (IAbsolutePath)
            razorMarkupTreeView.Item.AbsoluteFilePath.AncestorDirectories
                .Last();

        var parentAbsoluteFilePathString = parentDirectoryOfRazorMarkup.FormattedInput;

        var childFileTreeViewModels =
            (await razorMarkupTreeView.FileSystemProvider
                .Directory.GetFilesAsync(parentAbsoluteFilePathString))
                .Select(x =>
                {
                    var absoluteFilePath = new AbsolutePath(
                        x,
                        false,
                        razorMarkupTreeView.EnvironmentProvider);

                    var namespaceString = razorMarkupTreeView.Item.Namespace;

                    return (TreeViewNoType)new TreeViewNamespacePath(
                        new NamespacePath(
                            namespaceString,
                            absoluteFilePath),
                        razorMarkupTreeView.LuthetusIdeComponentRenderers,
                        razorMarkupTreeView.LuthetusCommonComponentRenderers,
                        razorMarkupTreeView.FileSystemProvider,
                        razorMarkupTreeView.EnvironmentProvider,
                        false,
                        false)
                    {
                        TreeViewChangedKey = TreeViewChangedKey.NewKey()
                    };
                }).ToList();

        RazorMarkupFindRelatedFiles(
            razorMarkupTreeView,
            childFileTreeViewModels);

        return razorMarkupTreeView.Children;
    }

    public static void RazorMarkupFindRelatedFiles(
        TreeViewNamespacePath razorMarkupTreeView,
        List<TreeViewNoType> siblingsAndSelfTreeViews)
    {
        razorMarkupTreeView.Children.Clear();

        // .razor files look to remove .razor.cs and .razor.css files

        var matches = new[]
        {
        razorMarkupTreeView.Item.AbsoluteFilePath.NameWithExtension +
            '.' +
            ExtensionNoPeriodFacts.C_SHARP_CLASS,
        razorMarkupTreeView.Item.AbsoluteFilePath.NameWithExtension +
            '.' +
            ExtensionNoPeriodFacts.CSS
    };

        var relatedFiles = siblingsAndSelfTreeViews.Where(x =>
                x.UntypedItem is NamespacePath namespacePath &&
                matches.Contains(namespacePath.AbsoluteFilePath.NameWithExtension))
            .ToArray();

        if (!relatedFiles.Any())
            return;

        for (var index = 0; index < relatedFiles.Length; index++)
        {
            var relatedFile = relatedFiles[index];

            siblingsAndSelfTreeViews.Remove(relatedFile);

            relatedFile.Parent = razorMarkupTreeView;
            relatedFile.IndexAmongSiblings = index;
            relatedFile.TreeViewChangedKey = TreeViewChangedKey.NewKey();

            razorMarkupTreeView.Children.Add(relatedFile);
        }

        razorMarkupTreeView.IsExpandable = true;

        razorMarkupTreeView.TreeViewChangedKey = TreeViewChangedKey.NewKey();
    }
}