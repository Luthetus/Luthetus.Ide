using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Extensions.DotNet.Nugets.Models;
using Luthetus.Extensions.DotNet.ComponentRenderers.Models;

namespace Luthetus.Extensions.DotNet.CSharpProjects.Models;

public class TreeViewCSharpProjectNugetPackageReference : TreeViewWithType<CSharpProjectNugetPackageReference>
{
	public TreeViewCSharpProjectNugetPackageReference(
			CSharpProjectNugetPackageReference cSharpProjectNugetPackageReference,
			ICompilerServicesComponentRenderers compilerServicesComponentRenderers,
			IIdeComponentRenderers ideComponentRenderers,
			IFileSystemProvider fileSystemProvider,
			IEnvironmentProvider environmentProvider,
			bool isExpandable,
			bool isExpanded)
		: base(cSharpProjectNugetPackageReference, isExpandable, isExpanded)
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
		if (obj is not TreeViewCSharpProjectNugetPackageReference otherTreeView)
			return false;

		return otherTreeView.GetHashCode() == GetHashCode();
	}

	public override int GetHashCode()
	{
		var uniqueString = Item.CSharpProjectAbsolutePathString + Item.LightWeightNugetPackageRecord.Id;
		return uniqueString.GetHashCode();
	}

	public override TreeViewRenderer GetTreeViewRenderer()
	{
		return new TreeViewRenderer(
			CompilerServicesComponentRenderers.CompilerServicesTreeViews.TreeViewCSharpProjectNugetPackageReferenceRendererType,
			new Dictionary<string, object?>
			{
				{
					nameof(ITreeViewCSharpProjectNugetPackageReferenceRendererType.CSharpProjectNugetPackageReference),
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