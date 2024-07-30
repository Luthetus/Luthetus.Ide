namespace Luthetus.Ide.RazorLib.Terminals.Models.NewCode;

public class TerminalOutputFormatterAll : ITerminalOutputFormatter
{
	public const string NAME = nameof(TerminalOutputFormatterAll);
	
	public string Name { get; }
	
	public string Format(ITerminal terminal)
	{
	}
}
