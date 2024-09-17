namespace Luthetus.Ide.RazorLib.Terminals.Models;

public class TerminalOutputFacts
{
	public const int MAX_COMMAND_COUNT = 100;
	public const int MAX_OUTPUT_LENGTH = 100_000;
	
	/// <summary>MAX_OUTPUT_LENGTH / 2 + 1; so that two terminal commands can sum and cause a clear</summary>
	public const int OUTPUT_LENGTH_PADDING = 50_001;
}
