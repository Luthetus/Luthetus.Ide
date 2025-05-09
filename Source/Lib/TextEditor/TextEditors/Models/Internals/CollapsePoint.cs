namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

public struct CollapsePoint
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

	public int AppendToLineIndex;
	public bool IsCollapsed;
	public string Identifier;
	public int EndExclusiveLineIndex;
}
