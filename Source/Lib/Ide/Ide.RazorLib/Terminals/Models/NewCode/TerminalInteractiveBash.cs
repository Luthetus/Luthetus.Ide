namespace Luthetus.Ide.RazorLib.Terminals.Models.NewCode;

/// <summary>Aaa</summary>
public class TerminalInteractiveBash : ITerminalInteractive
{
	private readonly ITerminal _terminal;

	public TerminalInteractiveBash(ITerminal terminal)
	{
		_terminal = terminal;
	}

	private string? _workingDirectoryAbsolutePathString;
	public string? WorkingDirectoryAbsolutePathString => _workingDirectoryAbsolutePathString;
	
	public void SetWorkingDirectory();
}
