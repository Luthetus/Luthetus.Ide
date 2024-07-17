using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.CompilerServices.DotNetSolution.Models.Project;
using Luthetus.Extensions.DotNet.ComponentRenderers.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;

namespace Luthetus.Extensions.DotNet.CSharpProjects.Models;

public class TreeViewCSharpProjectToProjectReference : TreeViewWithType<CSharpProjectToProjectReference>
{
	public TreeViewCSharpProjectToProjectReference(
			CSharpProjectToProjectReference cSharpProjectToProjectReference,
			ICompilerServicesComponentRenderers compilerServicesComponentRenderers,
			IIdeComponentRenderers ideComponentRenderers,
			IFileSystemProvider fileSystemProvider,
			IEnvironmentProvider environmentProvider,
			bool isExpandable,
			bool isExpanded)
		: base(cSharpProjectToProjectReference, isExpandable, isExpanded)
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
		if (obj is not TreeViewCSharpProjectToProjectReference otherTreeView)
			return false;

		return otherTreeView.GetHashCode() == GetHashCode();
	}

	public override int GetHashCode()
	{
		var modifyProjectAbsolutePathString = Item.ModifyProjectNamespacePath.AbsolutePath.Value;
		var referenceProjectAbsolutePathString = Item.ReferenceProjectAbsolutePath.Value;

		var uniqueAbsolutePathString = modifyProjectAbsolutePathString + referenceProjectAbsolutePathString;
		return uniqueAbsolutePathString.GetHashCode();
	}

	public override TreeViewRenderer GetTreeViewRenderer()
	{
		return new TreeViewRenderer(
			CompilerServicesComponentRenderers.CompilerServicesTreeViews.TreeViewCSharpProjectToProjectReferenceRendererType,
			new Dictionary<string, object?>
			{
				{
					nameof(ITreeViewCSharpProjectToProjectReferenceRendererType.CSharpProjectToProjectReference),
					Item
				},
			});
	}

	public override Task LoadChildListAsync()
	{
		TreeViewChangedKey = Key<TreeViewChanged>.NewKey();
		return Task.CompletedTask;
	}

	public override void RemoveRelatedFilesFromParent(List<TreeViewNoType> siblingsAndSelfTreeViews)
	{
		return;
	}
}