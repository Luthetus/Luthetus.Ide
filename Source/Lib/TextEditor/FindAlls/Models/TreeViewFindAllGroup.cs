using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.TextEditor.RazorLib.FindAlls.Displays;

namespace Luthetus.TextEditor.RazorLib.FindAlls.Models;

public class TreeViewFindAllGroup : TreeViewWithType<List<TreeViewFindAllTextSpan>>
{
	public TreeViewFindAllGroup(
			List<TreeViewFindAllTextSpan> treeViewFindAllTextSpanList,
			AbsolutePath absolutePath,
			bool isExpandable,
			bool isExpanded)
		: base(treeViewFindAllTextSpanList, isExpandable, isExpanded)
	{
		AbsolutePath = absolutePath;
	}
	
	public AbsolutePath AbsolutePath { get; }
	
	public override bool Equals(object? obj)
	{
		if (obj is not TreeViewFindAllGroup otherTreeView)
			return false;

		return otherTreeView.GetHashCode() == GetHashCode();
	}

	public override int GetHashCode() => AbsolutePath.Value.GetHashCode();
	
	public override TreeViewRenderer GetTreeViewRenderer()
	{
		return new TreeViewRenderer(
			typeof(TreeViewFindAllGroupDisplay),
			new Dictionary<string, object?>
			{
				{
					nameof(TreeViewFindAllGroupDisplay.TreeViewFindAllGroup),
					this
				}
			});
	}
	
	public override Task LoadChildListAsync()
	{
		if (ChildList.Count != 0)
			return Task.CompletedTask;
		
		var previousChildList = ChildList;
		ChildList = Item.Select(x => (TreeViewNoType)x).ToList();
		LinkChildren(previousChildList, ChildList);
		
		return Task.CompletedTask;
	}
}
