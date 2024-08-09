namespace Luthetus.Ide.RazorLib.Terminals.Models;

public class TerminalInputStringBuilder : ITerminalInput
{
	private readonly ITerminal _terminal;

	public TerminalInputStringBuilder(ITerminal terminal)
	{
		_terminal = terminal;
	}
	
	public void SendCommand(string commandText)
	{
	}
	
	public void Dispose()
	{
	}
}
