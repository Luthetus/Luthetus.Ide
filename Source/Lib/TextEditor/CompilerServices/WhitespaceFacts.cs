namespace Luthetus.TextEditor.RazorLib.CompilerServices;

public static class WhitespaceFacts
{
	public const char SPACE = ' ';
	public const char TAB = '\t';
	public const char CARRIAGE_RETURN = '\r';
	public const char LINE_FEED = '\n';

	public static readonly List<char> ALL_LIST = new()
	{
		SPACE,
		TAB,
		CARRIAGE_RETURN,
		LINE_FEED,
	};

	public static readonly List<char> LINE_ENDING_CHARACTER_LIST = new()
	{
		CARRIAGE_RETURN,
		LINE_FEED,
	};
}