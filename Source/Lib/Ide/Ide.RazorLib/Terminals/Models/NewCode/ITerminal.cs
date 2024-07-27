namespace Luthetus.Ide.RazorLib.Terminals.Models.NewCode;

/// <summary>input -> CliWrap -> output</summary>
public interface ITerminal
{
	public string DisplayName { get; }
	public ITerminalInteractive TerminalInteractive { get; }
	public ITerminalInput TerminalInput { get; }
	public ITerminalOutput TerminalOutput { get; }
}
