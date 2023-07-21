using Luthetus.Ide.ClassLib.FileSystem.Classes.FilePath;
using Luthetus.Ide.ClassLib.FileConstants;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;
using Luthetus.Ide.ClassLib.Namespaces;

namespace Luthetus.Ide.ClassLib.TreeViewImplementations.Helper;

public partial class TreeViewHelper
{
    public static async Task<List<TreeViewNoType>> LoadChildrenForRazorMarkupAsync(
        TreeViewNamespacePath razorMarkupTreeView)
    {
        if (razorMarkupTreeView.Item is null)
            return new();

        var parentDirectoryOfRazorMarkup = (IAbsoluteFilePath)
            razorMarkupTreeView.Item.AbsoluteFilePath.Directories
                .Last();

        var parentAbsoluteFilePathString = parentDirectoryOfRazorMarkup
            .GetAbsoluteFilePathString();

        var childFileTreeViewModels =
            (await razorMarkupTreeView.FileSystemProvider
                .Directory.GetFilesAsync(parentAbsoluteFilePathString))
                .Select(x =>
                {
                    var absoluteFilePath = new AbsoluteFilePath(
                        x,
                        false,
                        razorMarkupTreeView.EnvironmentProvider);

                    var namespaceString = razorMarkupTreeView.Item.Namespace;

                    return (TreeViewNoType)new TreeViewNamespacePath(
                        new NamespacePath(
                            namespaceString,
                            absoluteFilePath),
                        razorMarkupTreeView.LuthetusIdeComponentRenderers,
                        razorMarkupTreeView.FileSystemProvider,
                        razorMarkupTreeView.EnvironmentProvider,
                        false,
                        false)
                    {
                        TreeViewChangedKey = TreeViewChangedKey.NewTreeViewChangedKey()
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
        if (razorMarkupTreeView.Item is null)
            return;

        razorMarkupTreeView.Children.Clear();

        // .razor files look to remove .razor.cs and .razor.css files

        var matches = new[]
        {
        razorMarkupTreeView.Item.AbsoluteFilePath.FilenameWithExtension +
            '.' +
            ExtensionNoPeriodFacts.C_SHARP_CLASS,
        razorMarkupTreeView.Item.AbsoluteFilePath.FilenameWithExtension +
            '.' +
            ExtensionNoPeriodFacts.CSS
    };

        var relatedFiles = siblingsAndSelfTreeViews.Where(x =>
                x.UntypedItem is NamespacePath namespacePath &&
                matches.Contains(namespacePath.AbsoluteFilePath.FilenameWithExtension))
            .ToArray();

        if (!relatedFiles.Any())
            return;

        for (var index = 0; index < relatedFiles.Length; index++)
        {
            var relatedFile = relatedFiles[index];

            siblingsAndSelfTreeViews.Remove(relatedFile);

            relatedFile.Parent = razorMarkupTreeView;
            relatedFile.IndexAmongSiblings = index;
            relatedFile.TreeViewChangedKey = TreeViewChangedKey.NewTreeViewChangedKey();

            razorMarkupTreeView.Children.Add(relatedFile);
        }

        razorMarkupTreeView.IsExpandable = true;

        razorMarkupTreeView.TreeViewChangedKey = TreeViewChangedKey.NewTreeViewChangedKey();
    }
}