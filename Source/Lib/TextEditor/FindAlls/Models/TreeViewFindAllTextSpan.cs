using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.TextEditor.RazorLib.FindAlls.Displays;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.FindAlls.Models;

public class TreeViewFindAllTextSpan : TreeViewWithType<TextEditorTextSpan>
{
	public TreeViewFindAllTextSpan(
			TextEditorTextSpan textSpan,
			IEnvironmentProvider environmentProvider,
			IFileSystemProvider fileSystemProvider,
			bool isExpandable,
			bool isExpanded)
		: base(textSpan, isExpandable, isExpanded)
	{
		EnvironmentProvider = environmentProvider;
		FileSystemProvider = fileSystemProvider;
		AbsolutePath = EnvironmentProvider.AbsolutePathFactory(textSpan.ResourceUri.Value, false);
	}
	
	public IEnvironmentProvider EnvironmentProvider { get; }
	public IFileSystemProvider FileSystemProvider { get; }
	public IAbsolutePath AbsolutePath { get; }
	
	public override bool Equals(object? obj)
	{
		if (obj is not TreeViewFindAllTextSpan otherTreeView)
			return false;

		return otherTreeView.GetHashCode() == GetHashCode();
	}

	public override int GetHashCode() => Item.ResourceUri.Value.GetHashCode();
	
	public override TreeViewRenderer GetTreeViewRenderer()
	{
		return new TreeViewRenderer(
			typeof(TreeViewFindAllTextSpanDisplay),
			new Dictionary<string, object?>
			{
				{
					nameof(TreeViewFindAllTextSpanDisplay.TreeViewFindAllTextSpan),
					this
				}
			});
	}
	
	public override Task LoadChildListAsync()
	{
		return Task.CompletedTask;
	}
}
