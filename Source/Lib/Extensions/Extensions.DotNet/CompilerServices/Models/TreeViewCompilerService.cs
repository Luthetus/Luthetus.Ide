using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.WatchWindows.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;
using Luthetus.Ide.RazorLib.ComponentRenderers.Models;
using Luthetus.Extensions.DotNet.ComponentRenderers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;

namespace Luthetus.Extensions.DotNet.CompilerServices.Models;

public class TreeViewCompilerService : TreeViewWithType<ICompilerService>
{
	public TreeViewCompilerService(
			ICompilerService compilerService,
			IDotNetComponentRenderers dotNetComponentRenderers,
			IIdeComponentRenderers ideComponentRenderers,
			ICommonComponentRenderers commonComponentRenderers,
			bool isExpandable,
			bool isExpanded)
		: base(compilerService, isExpandable, isExpanded)
	{
		DotNetComponentRenderers = dotNetComponentRenderers;
        IdeComponentRenderers = ideComponentRenderers;
		CommonComponentRenderers = commonComponentRenderers;
	}

	public IDotNetComponentRenderers DotNetComponentRenderers { get; }
	public IIdeComponentRenderers IdeComponentRenderers { get; }
	public ICommonComponentRenderers CommonComponentRenderers { get; }

	public override bool Equals(object? obj)
	{
		if (obj is not TreeViewCompilerService treeViewCompilerService)
			return false;

		return treeViewCompilerService.Item == Item;
	}

	public override int GetHashCode() => Item.GetHashCode();

	public override TreeViewRenderer GetTreeViewRenderer()
	{
		return new TreeViewRenderer(
            DotNetComponentRenderers.CompilerServicesTreeViews.TreeViewCompilerServiceRendererType,
			new Dictionary<string, object?>
			{
				{ nameof(ITreeViewCompilerServiceRendererType.TreeViewCompilerService), this },
			});
	}

	public override Task LoadChildListAsync()
	{
		var previousChildren = new List<TreeViewNoType>(ChildList);

		try
		{
			ChildList.Clear();

			if (Item is CSharpCompilerService cSharpCompilerService)
			{
				ChildList.Add(new TreeViewCompilerService(
					cSharpCompilerService,
					DotNetComponentRenderers,
					IdeComponentRenderers,
					CommonComponentRenderers,
					true,
					false));
			}
		}
		catch (Exception e)
		{
			ChildList.Clear();
			ChildList.Add(new TreeViewException(e, false, false, CommonComponentRenderers));
		}

		LinkChildren(previousChildren, ChildList);

		return Task.CompletedTask;
	}
}