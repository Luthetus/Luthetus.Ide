namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

public class GutterChevron
{
	public GutterChevron(
		int lineIndex,
		bool isExpanded,
		string identifier,
		int startingIndexInclusive,
		int endingIndexExclusive,
		int exclusiveLineIndex)
	{
		LineIndex = lineIndex;
		IsExpanded = isExpanded;
		Identifier = identifier;
		StartingIndexInclusive = startingIndexInclusive;
		EndingIndexExclusive = endingIndexExclusive;
		ExclusiveLineIndex = exclusiveLineIndex;
	}

	public int LineIndex { get; }
	public bool IsExpanded { get; set; }
	public string Identifier { get; }
	public int StartingIndexInclusive { get; }
	public int EndingIndexExclusive { get; }
	public int ExclusiveLineIndex { get; }
}
