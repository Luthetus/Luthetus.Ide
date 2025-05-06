namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

public struct TextEditorLineIndexCacheEntry
{
	public TextEditorLineIndexCacheEntry(
		string topCssValue,
		string lineNumberString,
		int collapsedLineCount)
	{
		TopCssValue = topCssValue;
		LineNumberString = lineNumberString;
		CollapsedLineCount = collapsedLineCount;
	}

    public string TopCssValue { get; set; }
    public string LineNumberString { get; set; }
    public int CollapsedLineCount { get; set; }
}
