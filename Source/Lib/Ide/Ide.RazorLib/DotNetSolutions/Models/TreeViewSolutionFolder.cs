using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.WatchWindows.Models;
using Luthetus.Common.RazorLib.Namespaces.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.Models.Project;
using Luthetus.Ide.RazorLib.TreeViewUtils.Models;

namespace Luthetus.Ide.RazorLib.DotNetSolutions.Models;

public class TreeViewSolutionFolder : TreeViewWithType<SolutionFolder>
{
    public TreeViewSolutionFolder(
            SolutionFolder dotNetSolutionFolder,
            ILuthetusIdeComponentRenderers ideComponentRenderers,
            ILuthetusCommonComponentRenderers commonComponentRenderers,
            IFileSystemProvider fileSystemProvider,
            IEnvironmentProvider environmentProvider,
            bool isExpandable,
            bool isExpanded)
        : base(dotNetSolutionFolder, isExpandable, isExpanded)
    {
        IdeComponentRenderers = ideComponentRenderers;
        CommonComponentRenderers = commonComponentRenderers;
        FileSystemProvider = fileSystemProvider;
        EnvironmentProvider = environmentProvider;
    }

    public ILuthetusIdeComponentRenderers IdeComponentRenderers { get; }
    public ILuthetusCommonComponentRenderers CommonComponentRenderers { get; }
    public IFileSystemProvider FileSystemProvider { get; }
    public IEnvironmentProvider EnvironmentProvider { get; }

    public override bool Equals(object? obj)
    {
        if (obj is not TreeViewSolutionFolder treeViewSolutionFolder)
            return false;

        return treeViewSolutionFolder.Item.AbsolutePath.Value == Item.AbsolutePath.Value;
    }

    public override int GetHashCode() => Item.AbsolutePath.Value.GetHashCode();

    public override TreeViewRenderer GetTreeViewRenderer()
    {
        return new TreeViewRenderer(
            IdeComponentRenderers.LuthetusIdeTreeViews.TreeViewSolutionFolderRendererType,
            new Dictionary<string, object?>
            {
                {
                    nameof(ITreeViewSolutionFolderRendererType.DotNetSolutionFolder),
                    Item
                },
            });
    }

    public override Task LoadChildListAsync()
    {
        if (Item is null)
            return Task.CompletedTask;

        try
        {
            LinkChildren(ChildList, ChildList);
        }
        catch (Exception exception)
        {
            ChildList = new List<TreeViewNoType>
            {
                new TreeViewException(exception, false, false, CommonComponentRenderers)
                {
                    Parent = this,
                    IndexAmongSiblings = 0,
                }
            };
        }

        TreeViewChangedKey = Key<TreeViewChanged>.NewKey();

        return Task.CompletedTask;
    }

    public override void RemoveRelatedFilesFromParent(List<TreeViewNoType> siblingsAndSelfTreeViews)
    {
        var currentNode = (TreeViewNoType)this;

        // First, find the TreeViewSolution
        while (currentNode is not TreeViewSolution && currentNode.Parent is not null)
        {
            currentNode = currentNode.Parent;
        }

        if (currentNode is not TreeViewSolution treeViewSolution)
            return;

        var nestedProjectEntries = treeViewSolution.Item.NestedProjectEntryList
                .Where(x => x.SolutionFolderIdGuid == Item.ProjectIdGuid)
                .ToArray();

        var childProjectIds = nestedProjectEntries.Select(x => x.ChildProjectIdGuid).ToArray();

        var childProjectList = treeViewSolution.Item.DotNetProjectList
            .Where(x => childProjectIds.Contains(x.ProjectIdGuid))
            .ToArray();

        var childTreeViewSolutionFolderList = new List<TreeViewSolutionFolder>();
        var childTreeViewCSharpProjectList = new List<TreeViewNamespacePath>();

        foreach (var project in childProjectList)
        {
            if (project.DotNetProjectKind == DotNetProjectKind.SolutionFolder)
                childTreeViewSolutionFolderList.Add(ConstructTreeViewSolutionFolder((SolutionFolder)project));
            else
                childTreeViewCSharpProjectList.Add(ConstructTreeViewCSharpProject((CSharpProject)project));
        }

        var childTreeViewList =
            childTreeViewSolutionFolderList.OrderBy(x => x.Item.DisplayName).Select(x => (TreeViewNoType)x)
            .Union(childTreeViewCSharpProjectList.OrderBy(x => x.Item.AbsolutePath.NameNoExtension).Select(x => (TreeViewNoType)x))
            .ToList();

        for (int siblingsIndex = siblingsAndSelfTreeViews.Count - 1; siblingsIndex >= 0; siblingsIndex--)
        {
            var siblingOrSelf = siblingsAndSelfTreeViews[siblingsIndex];

            for (var childrensIndex = 0; childrensIndex < childTreeViewList.Count; childrensIndex++)
            {
                var childTreeView = childTreeViewList[childrensIndex];

                if (siblingOrSelf.Equals(childTreeView))
                {
                    // What i'm doing here is super confusing and needs changed.
                    // In lines above I re-created a TreeView node for a second time.
                    //
                    // Now I have to figure out where that re-created TreeView node
                    // existed originally because it will have its
                    // "RemoveRelatedFilesFromParent" invoked.
                    //
                    // Without this logic a:
                    //     solution folder -> solution folder -> project
                    //
                    // Will not render the project.
                    //
                    // TODO: Revisit this logic.
                    var originalTreeView = siblingsAndSelfTreeViews[siblingsIndex];

                    originalTreeView.Parent = this;
                    originalTreeView.IndexAmongSiblings = childrensIndex;
                    originalTreeView.TreeViewChangedKey = Key<TreeViewChanged>.NewKey();

                    siblingsAndSelfTreeViews.RemoveAt(siblingsIndex);

                    childTreeViewList[childrensIndex] = originalTreeView;
                }
                else
                {
                    childTreeView.Parent = this;
                    childTreeView.IndexAmongSiblings = childrensIndex;
                    childTreeView.TreeViewChangedKey = Key<TreeViewChanged>.NewKey();
                }
            }
        }

        ChildList = childTreeViewList;
    }

    private TreeViewSolutionFolder ConstructTreeViewSolutionFolder(SolutionFolder dotNetSolutionFolder)
    {
        return new TreeViewSolutionFolder(
            dotNetSolutionFolder,
            IdeComponentRenderers,
            CommonComponentRenderers,
            FileSystemProvider,
            EnvironmentProvider,
            true,
            false)
        {
            TreeViewChangedKey = Key<TreeViewChanged>.NewKey()
        };
    }

    private TreeViewNamespacePath ConstructTreeViewCSharpProject(CSharpProject cSharpProject)
    {
        var namespacePath = new NamespacePath(
            cSharpProject.AbsolutePath.NameNoExtension,
            cSharpProject.AbsolutePath);

        return new TreeViewNamespacePath(
            namespacePath,
            IdeComponentRenderers,
            CommonComponentRenderers,
            FileSystemProvider,
            EnvironmentProvider,
            true,
            false)
        {
            TreeViewChangedKey = Key<TreeViewChanged>.NewKey()
        };
    }
}