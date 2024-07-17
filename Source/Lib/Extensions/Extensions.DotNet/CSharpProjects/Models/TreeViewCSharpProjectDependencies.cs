using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.CompilerServices.DotNetSolution.Models.Project;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Extensions.DotNet.CSharpProjects.Models;
using Luthetus.Extensions.DotNet.ComponentRenderers.Models;

namespace Luthetus.Extensions.DotNet.CSharpProjects.Models;

public class TreeViewCSharpProjectDependencies : TreeViewWithType<CSharpProjectDependencies>
{
	public TreeViewCSharpProjectDependencies(
			CSharpProjectDependencies cSharpProjectDependencies,
			ICompilerServicesComponentRenderers compilerServicesComponentRenderers,
			IIdeComponentRenderers ideComponentRenderers,
			IFileSystemProvider fileSystemProvider,
			IEnvironmentProvider environmentProvider,
			bool isExpandable,
			bool isExpanded)
		: base(cSharpProjectDependencies, isExpandable, isExpanded)
	{
		CompilerServicesComponentRenderers = compilerServicesComponentRenderers;
		IdeComponentRenderers = ideComponentRenderers;
		FileSystemProvider = fileSystemProvider;
		EnvironmentProvider = environmentProvider;
	}

	public ICompilerServicesComponentRenderers CompilerServicesComponentRenderers { get; }
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
			CompilerServicesComponentRenderers.CompilerServicesTreeViews.TreeViewCSharpProjectDependenciesRendererType,
			null);
	}

	public override Task LoadChildListAsync()
	{
		var previousChildren = new List<TreeViewNoType>(ChildList);

		var treeViewCSharpProjectNugetPackageReferences = new TreeViewCSharpProjectNugetPackageReferences(
			new CSharpProjectNugetPackageReferences(Item.CSharpProjectNamespacePath),
			CompilerServicesComponentRenderers,
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
			CompilerServicesComponentRenderers,
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