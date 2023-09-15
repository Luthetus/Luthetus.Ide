using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.CompilerServices.Lang.DotNetSolution.CSharp;
using Luthetus.Ide.RazorLib.ComponentRenderersCase.Models;

namespace Luthetus.Ide.RazorLib.TreeViewImplementationsCase.Models;

public class TreeViewCSharpProjectDependencies : TreeViewWithType<CSharpProjectDependencies>
{
    public TreeViewCSharpProjectDependencies(
        CSharpProjectDependencies cSharpProjectDependencies,
        ILuthetusIdeComponentRenderers luthetusIdeComponentRenderers,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider,
        bool isExpandable,
        bool isExpanded)
            : base(
                cSharpProjectDependencies,
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
        if (obj is not TreeViewCSharpProjectDependencies otherTreeView)
            return false;

        return otherTreeView.GetHashCode() == GetHashCode();
    }

    public override int GetHashCode()
    {
        return Item.CSharpProjectNamespacePath.AbsolutePath
            .FormattedInput
            .GetHashCode();
    }

    public override TreeViewRenderer GetTreeViewRenderer()
    {
        return new TreeViewRenderer(
            LuthetusIdeComponentRenderers.LuthetusIdeTreeViews.TreeViewCSharpProjectDependenciesRendererType!,
            null);
    }

    public override Task LoadChildrenAsync()
    {
        var treeViewCSharpProjectNugetPackageReferences = new TreeViewCSharpProjectNugetPackageReferences(
            new CSharpProjectNugetPackageReferences(Item.CSharpProjectNamespacePath),
            LuthetusIdeComponentRenderers,
            FileSystemProvider,
            EnvironmentProvider,
            true,
            false)
        {
            TreeViewChangedKey = TreeViewChangedKey.NewKey()
        };

        var treeViewCSharpProjectToProjectReferences = new TreeViewCSharpProjectToProjectReferences(
            new CSharpProjectToProjectReferences(Item.CSharpProjectNamespacePath),
            LuthetusIdeComponentRenderers,
            FileSystemProvider,
            EnvironmentProvider,
            true,
            false)
        {
            TreeViewChangedKey = TreeViewChangedKey.NewKey()
        };

        var newChildren = new List<TreeViewNoType>
    {
        treeViewCSharpProjectNugetPackageReferences,
        treeViewCSharpProjectToProjectReferences
    };

        for (int i = 0; i < newChildren.Count; i++)
        {
            var newChild = newChildren[i];

            newChild.IndexAmongSiblings = i;
            newChild.Parent = this;
            newChild.TreeViewChangedKey = TreeViewChangedKey.NewKey();
        }

        Children = newChildren;
        TreeViewChangedKey = TreeViewChangedKey.NewKey();
        return Task.CompletedTask;
    }

    public override void RemoveRelatedFilesFromParent(List<TreeViewNoType> siblingsAndSelfTreeViews)
    {
        return;
    }
}