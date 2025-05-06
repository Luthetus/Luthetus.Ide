namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

public struct TextEditorLineIndexCacheEntry
{
	public TextEditorLineIndexCacheEntry(
		string topCssValue,
		string lineNumberString,
		int hiddenLineCount)
	{
		TopCssValue = topCssValue;
		LineNumberString = lineNumberString;
		HiddenLineCount = hiddenLineCount;
	}

    public string TopCssValue { get; set; }
    public string LineNumberString { get; set; }
    public int HiddenLineCount { get; set; }
}
