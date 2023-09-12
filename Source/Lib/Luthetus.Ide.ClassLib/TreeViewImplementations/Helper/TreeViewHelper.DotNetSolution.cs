using Luthetus.Common.RazorLib.Namespaces;
using Luthetus.Common.RazorLib.TreeView.TreeViewClasses;
using Luthetus.CompilerServices.Lang.DotNetSolution;

namespace Luthetus.Ide.ClassLib.TreeViewImplementations.Helper;

public partial class TreeViewHelper
{
    public static Task<List<TreeViewNoType>> DotNetSolutionLoadChildrenAsync(
        this TreeViewSolution treeViewSolution)
    {
        var childSolutionFolders = treeViewSolution.Item.SolutionFolders
            .Select(x => (TreeViewNoType)new TreeViewSolutionFolder(
                x,
                treeViewSolution.LuthetusIdeComponentRenderers,
                treeViewSolution.LuthetusCommonComponentRenderers,
                treeViewSolution.FileSystemProvider,
                treeViewSolution.EnvironmentProvider,
                true,
                false)
            {
                TreeViewChangedKey = TreeViewChangedKey.NewKey()
            })
            .OrderBy(x => ((TreeViewSolutionFolder)x).Item.AbsoluteFilePath.FileNameNoExtension)
            .ToList();

        var childProjects = treeViewSolution.Item.DotNetProjects
            .Where(x => x.ProjectTypeGuid != DotNetSolutionFolder.SolutionFolderProjectTypeGuid)
            .Select(x =>
            {
                var namespacePath = new NamespacePath(
                    x.AbsoluteFilePath.FileNameNoExtension,
                    x.AbsoluteFilePath);

                return (TreeViewNoType)new TreeViewNamespacePath(
                    namespacePath,
                    treeViewSolution.LuthetusIdeComponentRenderers,
                    treeViewSolution.LuthetusCommonComponentRenderers,
                    treeViewSolution.FileSystemProvider,
                    treeViewSolution.EnvironmentProvider,
                    true,
                    false)
                {
                    TreeViewChangedKey = TreeViewChangedKey.NewKey()
                };
            })
            .OrderBy(x => ((TreeViewNamespacePath)x).Item.AbsoluteFilePath.FileNameNoExtension)
            .ToList();

        var children = childSolutionFolders
            .Union(childProjects)
            .ToList();

        var copyOfChildrenToFindRelatedFiles = new List<TreeViewNoType>(children);

        // The foreach for child.Parent and the
        // foreach for child.RemoveRelatedFilesFromParent(...)
        // cannot be combined.
        foreach (var child in children)
            child.Parent = treeViewSolution;

        foreach (var child in children)
        {
            child.RemoveRelatedFilesFromParent(
                copyOfChildrenToFindRelatedFiles);
        }

        // The parent directory gets what is left over after the
        // children take their respective 'code behinds'
        return Task.FromResult(copyOfChildrenToFindRelatedFiles);
    }
}