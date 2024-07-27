namespace Luthetus.Ide.RazorLib.Terminals.Models.NewCode;

public class TerminalInputTextEditor : ITerminalInput
{
	private readonly ITerminal _terminal;

	public TerminalInputTextEditor(ITerminal terminal)
	{
		_terminal = terminal;
		ResourceUri = new(ResourceUriFacts.Terminal_ReservedResourceUri_Prefix + Key.Guid.ToString());
		CreateTextEditor();
	}

	public void OnAfterWorkingDirectoryChanged(string workingDirectoryAbsolutePathString)
	{
	}
	
	public void OnHandleCommandStarting()
	{
	}
}
