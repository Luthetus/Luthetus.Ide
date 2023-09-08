using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using Luthetus.CompilerServices.Lang.DotNetSolution.CSharp;
using Luthetus.Ide.ClassLib.ComponentRenderers;
using Luthetus.Ide.ClassLib.ComponentRenderers.Types.TreeViews;

namespace Luthetus.Ide.ClassLib.TreeViewImplementations;

public class TreeViewCSharpProjectToProjectReference : TreeViewWithType<CSharpProjectToProjectReference>
{
    public TreeViewCSharpProjectToProjectReference(
        CSharpProjectToProjectReference cSharpProjectToProjectReference,
        ILuthetusIdeComponentRenderers luthetusIdeComponentRenderers,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider,
        bool isExpandable,
        bool isExpanded)
            : base(
                cSharpProjectToProjectReference,
                isExpandable,
                isExpanded)
    {
        LuthetusIdeComponentRenderers = luthetusIdeComponentRenderers;
        FileSystemProvider = fileSystemProvider;
        EnvironmentProvider = environmentProvider;
    }

    public ILuthetusIdeComponentRenderers LuthetusIdeComponentRenderers { get; }
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
        var modifyProjectAbsoluteFilePathString = Item.ModifyProjectNamespacePath.AbsoluteFilePath
            .FormattedInput;
        
        var referenceProjectAbsoluteFilePathString = Item.ReferenceProjectAbsoluteFilePath
            .FormattedInput;

        var uniqueAbsoluteFilePathString = modifyProjectAbsoluteFilePathString +
            referenceProjectAbsoluteFilePathString;

        return uniqueAbsoluteFilePathString.GetHashCode();
    }

    public override TreeViewRenderer GetTreeViewRenderer()
    {
        return new TreeViewRenderer(
            LuthetusIdeComponentRenderers.LuthetusIdeTreeViews.TreeViewCSharpProjectToProjectReferenceRendererType!,
            new Dictionary<string, object?>
            {
            {
                nameof(ITreeViewCSharpProjectToProjectReferenceRendererType.CSharpProjectToProjectReference),
                Item
            },
            });
    }

    public override Task LoadChildrenAsync()
    {
        TreeViewChangedKey = TreeViewChangedKey.NewTreeViewChangedKey();
        return Task.CompletedTask;
    }

    public override void RemoveRelatedFilesFromParent(List<TreeViewNoType> siblingsAndSelfTreeViews)
    {
        return;
    }
}