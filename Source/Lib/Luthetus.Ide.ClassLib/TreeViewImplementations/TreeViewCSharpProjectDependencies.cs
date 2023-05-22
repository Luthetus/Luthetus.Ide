using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using Luthetus.Ide.ClassLib.DotNet.CSharp;
using Luthetus.Ide.ClassLib.TreeViewImplementations;
using Luthetus.Ide.ClassLib.ComponentRenderers;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;

namespace Luthetus.Ide.ClassLib.TreeViewImplementations;

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
        if (obj is null ||
            obj is not TreeViewCSharpProjectDependencies treeViewCSharpProjectDependencies ||
            Item is null)
        {
            return false;
        }

        return true;
    }

    public override int GetHashCode()
    {
        return Item?
            .GetHashCode()
               ?? default;
    }

    public override TreeViewRenderer GetTreeViewRenderer()
    {
        return new TreeViewRenderer(
            LuthetusIdeComponentRenderers.TreeViewCSharpProjectDependenciesRendererType!,
            null);
    }

    public override async Task LoadChildrenAsync()
    {
        if (Item is null)
            return;

        var treeViewCSharpProjectNugetPackageReferences = new TreeViewCSharpProjectNugetPackageReferences(
            new CSharpProjectNugetPackageReferences(Item.CSharpProjectNamespacePath),
            LuthetusIdeComponentRenderers,
            FileSystemProvider,
            EnvironmentProvider,
            true,
            false)
        {
            TreeViewChangedKey = TreeViewChangedKey.NewTreeViewChangedKey()
        };

        var treeViewCSharpProjectToProjectReferences = new TreeViewCSharpProjectToProjectReferences(
            new CSharpProjectToProjectReferences(Item.CSharpProjectNamespacePath),
            LuthetusIdeComponentRenderers,
            FileSystemProvider,
            EnvironmentProvider,
            true,
            false)
        {
            TreeViewChangedKey = TreeViewChangedKey.NewTreeViewChangedKey()
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
            newChild.TreeViewChangedKey = TreeViewChangedKey.NewTreeViewChangedKey();
        }

        Children = newChildren;
        TreeViewChangedKey = TreeViewChangedKey.NewTreeViewChangedKey();
    }

    public override void RemoveRelatedFilesFromParent(List<TreeViewNoType> siblingsAndSelfTreeViews)
    {
        return;
    }
}