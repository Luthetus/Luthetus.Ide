using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Namespaces.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.CompilerServices.DotNetSolution.Models.Project;
using Luthetus.Extensions.DotNet.Namespaces.Models;

namespace Luthetus.Extensions.DotNet.DotNetSolutions.Models;

public class TreeViewHelperDotNetSolution
{
	public static Task<List<TreeViewNoType>> LoadChildrenAsync(TreeViewSolution treeViewSolution)
	{
		var childSolutionFolders = treeViewSolution.Item.SolutionFolderList.Select(
			x => (TreeViewNoType)new TreeViewSolutionFolder(
				x,
				treeViewSolution.DotNetComponentRenderers,
				treeViewSolution.IdeComponentRenderers,
				treeViewSolution.CommonComponentRenderers,
				treeViewSolution.FileSystemProvider,
				treeViewSolution.EnvironmentProvider,
				true,
				false)
			{
				TreeViewChangedKey = Key<TreeViewChanged>.NewKey()
			})
			.OrderBy(x => ((TreeViewSolutionFolder)x).Item.DisplayName)
			.ToList();

		var childProjects = treeViewSolution.Item.DotNetProjectList
			.Where(x => x.ProjectTypeGuid != SolutionFolder.SolutionFolderProjectTypeGuid)
			.Select(x =>
			{
				return (TreeViewNoType)new TreeViewNamespacePath(
					new NamespacePath(x.AbsolutePath.NameNoExtension, x.AbsolutePath),
					treeViewSolution.DotNetComponentRenderers,
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

		var children = childSolutionFolders.Concat(childProjects).ToList();

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