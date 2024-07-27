namespace Luthetus.Ide.RazorLib.Terminals.Models.NewCode;

/// <summary>
/// This implementation of <see cref="ITerminal"/> is a "blank slate".
/// </summary>
public class Terminal : ITerminal
{
	public Terminal(
		string displayName,
		ITerminalInteractive terminalInteractive,
		ITerminalInput terminalInput,
		ITerminalOutput terminalOutput)
	{
		DisplayName = displayName;
		TerminalInteractive = terminalInteractive;
		TerminalInput = terminalInput;
		TerminalOutput = terminalOutput;
	}

	public string DisplayName { get; }
	public ITerminalInteractive TerminalInteractive { get; }
	public ITerminalInput TerminalInput { get; }
	public ITerminalOutput TerminalOutput { get; }
}
