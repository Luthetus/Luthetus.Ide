using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.Ide.RazorLib.CodeSearches.Displays;

namespace Luthetus.Ide.RazorLib.CodeSearches.Models;

public class TreeViewCodeSearchTextSpan : TreeViewWithType<TextEditorTextSpan>
{
	public TreeViewCodeSearchTextSpan(
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
	public AbsolutePath AbsolutePath { get; }
	
	public override bool Equals(object? obj)
	{
		if (obj is not TreeViewCodeSearchTextSpan otherTreeView)
			return false;

		return otherTreeView.GetHashCode() == GetHashCode();
	}

	public override int GetHashCode() => Item.ResourceUri.Value.GetHashCode();
	
	public override TreeViewRenderer GetTreeViewRenderer()
	{
		return new TreeViewRenderer(
			typeof(TreeViewCodeSearchTextSpanDisplay),
			new Dictionary<string, object?>
			{
				{
					nameof(TreeViewCodeSearchTextSpanDisplay.TreeViewCodeSearchTextSpan),
					this
				}
			});
	}
	
	public override Task LoadChildListAsync()
	{
		return Task.CompletedTask;
	}
}

