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

    public string TopCssValue;
    public string LineNumberString;
    public int HiddenLineCount;
}
