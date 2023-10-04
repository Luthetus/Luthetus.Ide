using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.CSharp;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;

namespace Luthetus.Ide.RazorLib.TreeViewImplementations.Models;

public class TreeViewCSharpProjectToProjectReference : TreeViewWithType<CSharpProjectToProjectReference>
{
    public TreeViewCSharpProjectToProjectReference(
            CSharpProjectToProjectReference cSharpProjectToProjectReference,
            ILuthetusIdeComponentRenderers ideComponentRenderers,
            IFileSystemProvider fileSystemProvider,
            IEnvironmentProvider environmentProvider,
            bool isExpandable,
            bool isExpanded)
        : base(cSharpProjectToProjectReference, isExpandable, isExpanded)
    {
        IdeComponentRenderers = ideComponentRenderers;
        FileSystemProvider = fileSystemProvider;
        EnvironmentProvider = environmentProvider;
    }

    public ILuthetusIdeComponentRenderers IdeComponentRenderers { get; }
    public IFileSystemProvider FileSystemProvider { get; }
    public IEnvironmentProvider EnvironmentProvider { get; }

    public override bool Equals(object? obj)
    {
        if (obj is not TreeViewCSharpProjectToProjectReference otherTreeView)
            return false;

        return otherTreeView.GetHashCode() == GetHashCode();
    }

    public override int GetHashCode()
    {
        var modifyProjectAbsolutePathString = Item.ModifyProjectNamespacePath.AbsolutePath.FormattedInput;
        var referenceProjectAbsolutePathString = Item.ReferenceProjectAbsolutePath.FormattedInput;
        
        var uniqueAbsolutePathString = modifyProjectAbsolutePathString + referenceProjectAbsolutePathString;
        return uniqueAbsolutePathString.GetHashCode();
    }

    public override TreeViewRenderer GetTreeViewRenderer()
    {
        return new TreeViewRenderer(
            IdeComponentRenderers.LuthetusIdeTreeViews.TreeViewCSharpProjectToProjectReferenceRendererType,
            new Dictionary<string, object?>
            {
                {
                    nameof(ITreeViewCSharpProjectToProjectReferenceRendererType.CSharpProjectToProjectReference),
                    Item
                },
            });
    }

    public override Task LoadChildBagAsync()
    {
        TreeViewChangedKey = Key<TreeViewChanged>.NewKey();
        return Task.CompletedTask;
    }

    public override void RemoveRelatedFilesFromParent(List<TreeViewNoType> siblingsAndSelfTreeViews)
    {
        return;
    }
}