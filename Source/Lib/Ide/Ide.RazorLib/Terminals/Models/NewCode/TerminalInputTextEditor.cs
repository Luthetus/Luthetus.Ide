namespace Luthetus.Ide.RazorLib.Terminals.Models.NewCode;

public class TerminalInputTextEditor : ITerminalInput
{
	private readonly ITerminal _terminal;

	public TerminalInputTextEditor(ITerminal terminal)
	{
		_terminal = terminal;
		ResourceUri = new(ResourceUriFacts.Terminal_ReservedResourceUri_Prefix + Key.Guid.ToString());
		CreateTextEditor();
		
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
		_terminal.EnqueueCommand(commandText);
	}
	
	public void Dispose()
	{
		_terminal.TerminalInteractive.WorkingDirectoryChanged -= OnWorkingDirectoryChanged;
	}
}
