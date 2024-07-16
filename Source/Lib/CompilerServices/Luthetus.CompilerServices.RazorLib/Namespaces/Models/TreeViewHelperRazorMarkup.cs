using Luthetus.Common.RazorLib.Namespaces.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.Ide.RazorLib.Namespaces.Models;

namespace Luthetus.CompilerServices.RazorLib.Namespaces.Models;

public class TreeViewHelperRazorMarkup
{
    public static async Task<List<TreeViewNoType>> LoadChildrenAsync(TreeViewNamespacePath razorMarkupTreeView)
    {
        if (razorMarkupTreeView.Item is null)
            return new();

        var parentDirectoryOfRazorMarkup = razorMarkupTreeView.Item.AbsolutePath.AncestorDirectoryList.Last();
        var ancestorDirectory = parentDirectoryOfRazorMarkup;

        var filePathStringsList = await razorMarkupTreeView.FileSystemProvider.Directory
            .GetFilesAsync(ancestorDirectory.Value)
            .ConfigureAwait(false);

        var childFileTreeViewModels = filePathStringsList
            .Select(x =>
            {
                var absolutePath = razorMarkupTreeView.EnvironmentProvider.AbsolutePathFactory(x, false);
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

        FindRelatedFiles(razorMarkupTreeView, childFileTreeViewModels);
        return razorMarkupTreeView.ChildList;
    }

    public static void FindRelatedFiles(
        TreeViewNamespacePath razorMarkupTreeView,
        List<TreeViewNoType> siblingsAndSelfTreeViews)
    {
        razorMarkupTreeView.ChildList.Clear();

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

            razorMarkupTreeView.ChildList.Add(relatedFile);
        }

        razorMarkupTreeView.IsExpandable = true;

        razorMarkupTreeView.TreeViewChangedKey = Key<TreeViewChanged>.NewKey();
    }
}