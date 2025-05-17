namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

public struct TextEditorLineIndexCacheEntry
{
	public TextEditorLineIndexCacheEntry(
		string topCssValue,
		string leftCssValue,
		string lineNumberString,
		int hiddenLineCount)
	{
		TopCssValue = topCssValue;
		LeftCssValue = leftCssValue;
		LineNumberString = lineNumberString;
		HiddenLineCount = hiddenLineCount;
	}

    public string TopCssValue { get; set; }
    public string LeftCssValue { get; set; }
    public string LineNumberString { get; set; }
    public int HiddenLineCount { get; set; }
}
