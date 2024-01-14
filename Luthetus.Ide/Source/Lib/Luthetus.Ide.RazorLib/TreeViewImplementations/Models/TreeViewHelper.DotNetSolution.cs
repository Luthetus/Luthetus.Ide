using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Namespaces.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.Models.Project;

namespace Luthetus.Ide.RazorLib.TreeViewImplementations.Models;

public partial class TreeViewHelper
{
    public static Task<List<TreeViewNoType>> DotNetSolutionLoadChildrenAsync(
        this TreeViewSolution treeViewSolution)
    {
        var childSolutionFolders = treeViewSolution.Item.SolutionFolderList.Select(
            x => (TreeViewNoType)new TreeViewSolutionFolder(
                x,
                treeViewSolution.IdeComponentRenderers,
                treeViewSolution.CommonComponentRenderers,
                treeViewSolution.FileSystemProvider,
                treeViewSolution.EnvironmentProvider,
                true,
                false)
            {
                TreeViewChangedKey = Key<TreeViewChanged>.NewKey()
            })
            .OrderBy(x => ((TreeViewSolutionFolder)x).Item.AbsolutePath.NameNoExtension)
            .ToList();

        var childProjects = treeViewSolution.Item.DotNetProjectList
            .Where(x => x.ProjectTypeGuid != SolutionFolder.SolutionFolderProjectTypeGuid)
            .Select(x =>
            {
                return (TreeViewNoType)new TreeViewNamespacePath(
                    new NamespacePath(x.AbsolutePath.NameNoExtension, x.AbsolutePath),
                    treeViewSolution.IdeComponentRenderers,
                    treeViewSolution.CommonComponentRenderers,
                    treeViewSolution.FileSystemProvider,
                    treeViewSolution.EnvironmentProvider,
                    true,
                    false)
                {
                    TreeViewChangedKey = Key<TreeViewChanged>.NewKey()
                };
            })
            .OrderBy(x => ((TreeViewNamespacePath)x).Item.AbsolutePath.NameNoExtension)
            .ToList();

        var children = childSolutionFolders.Union(childProjects).ToList();

        var copyOfChildrenToFindRelatedFiles = new List<TreeViewNoType>(children);

        // The foreach for child.Parent and the
        // foreach for child.RemoveRelatedFilesFromParent(...)
        // cannot be combined.
        foreach (var child in children)
        {
            child.Parent = treeViewSolution;
        }

        // The foreach for child.Parent and the
        // foreach for child.RemoveRelatedFilesFromParent(...)
        // cannot be combined.
        foreach (var child in children)
        {
            child.RemoveRelatedFilesFromParent(copyOfChildrenToFindRelatedFiles);
        }

        // The parent directory gets what is left over after the
        // children take their respective 'code behinds'
        return Task.FromResult(copyOfChildrenToFindRelatedFiles);
    }
}