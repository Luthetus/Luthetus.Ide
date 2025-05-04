namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

public class CollapsePoint
{
	public CollapsePoint(
		int appendToLineIndex,
		bool isCollapsed,
		string identifier,
		int endExclusiveLineIndex)
	{
		AppendToLineIndex = appendToLineIndex;
		IsCollapsed = isCollapsed;
		Identifier = identifier;
		EndExclusiveLineIndex = endExclusiveLineIndex;
	}

	public int AppendToLineIndex { get; }
	public bool IsCollapsed { get; set; }
	public string Identifier { get; }
	public int EndExclusiveLineIndex { get; }
}
