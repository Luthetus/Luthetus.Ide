namespace Luthetus.Ide.RazorLib.Terminals.Models.NewCode;

public class TerminalInputStringBuilder : ITerminalInput
{
	private readonly ITerminal _terminal;

	public TerminalInputStringBuilder(ITerminal terminal)
	{
		_terminal = terminal;
		
		_terminal.TerminalInteractive.WorkingDirectoryChanged += OnWorkingDirectoryChanged;
	}

	public void OnWorkingDirectoryChanged()
	{
	}
	
	public void OnHandleCommandStarting()
	{
	}
	
	public void SendCommand(string commandText)
	{
		//_terminal.EnqueueCommand(commandText);
	}
	
	public void Dispose()
	{
		_terminal.TerminalInteractive.WorkingDirectoryChanged -= OnWorkingDirectoryChanged;
	}
}
