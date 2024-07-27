namespace Luthetus.Ide.RazorLib.Terminals.Models.NewCode;

public class TerminalInputStringBuilder : ITerminalInput
{
	private readonly ITerminal _terminal;

	public TerminalInputTextEditor(ITerminal terminal)
	{
		_terminal = terminal;
	}

	public void OnAfterWorkingDirectoryChanged(string workingDirectoryAbsolutePathString)
	{
	}
	
	public void OnHandleCommandStarting()
	{
	}
	
	public void SendCommand(string commandText)
	{
		_terminal.EnqueueCommand(commandText);
	}
}
