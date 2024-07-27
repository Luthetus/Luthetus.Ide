namespace Luthetus.Ide.RazorLib.Terminals.Models.NewCode;

/// <summary>
/// This implementation of <see cref="ITerminal"/> is a specific
/// implementation meant for the "Execution" terminal in the IDE.
/// </summary>
public class TerminalExecution : ITerminal
{
	public TerminalExecution()
	{
		DisplayName = "Execution";
		TerminalInteractive = terminalInteractive;
		TerminalInput  = new TerminalInputTextEditor(/*ITerminal*/ this);
		TerminalOutput = new TerminalOutputTextEditor(/*ITerminal*/ this);
	}

	public string DisplayName { get; }
	public ITerminalInteractive TerminalInteractive { get; }
	public ITerminalInput TerminalInput { get; }
	public ITerminalOutput TerminalOutput { get; }
}
