using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Namespaces.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.FileSystems.Models;

namespace Luthetus.Ide.RazorLib.TreeViewImplementations.Models;

public partial class TreeViewHelper
{
    public static async Task<List<TreeViewNoType>> RazorMarkupLoadChildrenAsync(
        this TreeViewNamespacePath razorMarkupTreeView)
    {
        if (razorMarkupTreeView.Item is null)
            return new();

        var parentDirectoryOfRazorMarkup = razorMarkupTreeView.Item.AbsolutePath.AncestorDirectoryBag.Last();
        var parentAbsolutePathString = parentDirectoryOfRazorMarkup.FormattedInput;

        var filePathStringsBag = await razorMarkupTreeView.FileSystemProvider.Directory
            .GetFilesAsync(parentAbsolutePathString);

        var childFileTreeViewModels = filePathStringsBag
            .Select(x =>
            {
                var absolutePath = new AbsolutePath(x, false, razorMarkupTreeView.EnvironmentProvider);
                var namespaceString = razorMarkupTreeView.Item.Namespace;

                return (TreeViewNoType)new TreeViewNamespacePath(
                    new NamespacePath(namespaceString, absolutePath),
                    razorMarkupTreeView.IdeComponentRenderers,
                    razorMarkupTreeView.CommonComponentRenderers,
                    razorMarkupTreeView.FileSystemProvider,
                    razorMarkupTreeView.EnvironmentProvider,
                    false,
                    false)
                {
                    TreeViewChangedKey = Key<TreeViewChanged>.NewKey()
                };
            }).ToList();

        RazorMarkupFindRelatedFiles(razorMarkupTreeView, childFileTreeViewModels);
        return razorMarkupTreeView.ChildBag;
    }

    public static void RazorMarkupFindRelatedFiles(
        TreeViewNamespacePath razorMarkupTreeView,
        List<TreeViewNoType> siblingsAndSelfTreeViews)
    {
        razorMarkupTreeView.ChildBag.Clear();

        // .razor files look to remove .razor.cs and .razor.css files
        var matches = new[]
        {
            razorMarkupTreeView.Item.AbsolutePath.NameWithExtension + '.' + ExtensionNoPeriodFacts.C_SHARP_CLASS,
            razorMarkupTreeView.Item.AbsolutePath.NameWithExtension + '.' + ExtensionNoPeriodFacts.CSS
        };

        var relatedFiles = siblingsAndSelfTreeViews.Where(x =>
                x.UntypedItem is NamespacePath namespacePath &&
                matches.Contains(namespacePath.AbsolutePath.NameWithExtension))
            .ToArray();

        if (!relatedFiles.Any())
            return;

        for (var index = 0; index < relatedFiles.Length; index++)
        {
            var relatedFile = relatedFiles[index];

            siblingsAndSelfTreeViews.Remove(relatedFile);

            relatedFile.Parent = razorMarkupTreeView;
            relatedFile.IndexAmongSiblings = index;
            relatedFile.TreeViewChangedKey = Key<TreeViewChanged>.NewKey();

            razorMarkupTreeView.ChildBag.Add(relatedFile);
        }

        razorMarkupTreeView.IsExpandable = true;

        razorMarkupTreeView.TreeViewChangedKey = Key<TreeViewChanged>.NewKey();
    }
}