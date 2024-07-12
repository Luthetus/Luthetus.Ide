using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.Models.Project;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;

namespace Luthetus.Ide.RazorLib.CSharpProjects.Models;

public class TreeViewCSharpProjectDependencies : TreeViewWithType<CSharpProjectDependencies>
{
    public TreeViewCSharpProjectDependencies(
            CSharpProjectDependencies cSharpProjectDependencies,
            IIdeComponentRenderers ideComponentRenderers,
            IFileSystemProvider fileSystemProvider,
            IEnvironmentProvider environmentProvider,
            bool isExpandable,
            bool isExpanded)
        : base(cSharpProjectDependencies, isExpandable, isExpanded)
    {
        IdeComponentRenderers = ideComponentRenderers;
        FileSystemProvider = fileSystemProvider;
        EnvironmentProvider = environmentProvider;
    }

    public IIdeComponentRenderers IdeComponentRenderers { get; }
    public IFileSystemProvider FileSystemProvider { get; }
    public IEnvironmentProvider EnvironmentProvider { get; }

    public override bool Equals(object? obj)
    {
        if (obj is not TreeViewCSharpProjectDependencies otherTreeView)
            return false;

        return otherTreeView.GetHashCode() == GetHashCode();
    }

    public override int GetHashCode() => Item.CSharpProjectNamespacePath.AbsolutePath.Value.GetHashCode();

    public override TreeViewRenderer GetTreeViewRenderer()
    {
        return new TreeViewRenderer(
            IdeComponentRenderers.IdeTreeViews.TreeViewCSharpProjectDependenciesRendererType,
            null);
    }

    public override Task LoadChildListAsync()
    {
        var previousChildren = new List<TreeViewNoType>(ChildList);

        var treeViewCSharpProjectNugetPackageReferences = new TreeViewCSharpProjectNugetPackageReferences(
            new CSharpProjectNugetPackageReferences(Item.CSharpProjectNamespacePath),
            IdeComponentRenderers,
            FileSystemProvider,
            EnvironmentProvider,
            true,
            false)
        {
            TreeViewChangedKey = Key<TreeViewChanged>.NewKey()
        };

        var treeViewCSharpProjectToProjectReferences = new TreeViewCSharpProjectToProjectReferences(
            new CSharpProjectToProjectReferences(Item.CSharpProjectNamespacePath),
            IdeComponentRenderers,
            FileSystemProvider,
            EnvironmentProvider,
            true,
            false)
        {
            TreeViewChangedKey = Key<TreeViewChanged>.NewKey()
        };

        var newChildList = new List<TreeViewNoType>
        {
            treeViewCSharpProjectNugetPackageReferences,
            treeViewCSharpProjectToProjectReferences
        };

        ChildList = newChildList;
        LinkChildren(previousChildren, ChildList);

        TreeViewChangedKey = Key<TreeViewChanged>.NewKey();
        return Task.CompletedTask;
    }

    public override void RemoveRelatedFilesFromParent(List<TreeViewNoType> siblingsAndSelfTreeViews)
    {
        return;
    }
}