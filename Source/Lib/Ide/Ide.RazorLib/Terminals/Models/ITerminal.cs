using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

/// <summary>input -> CliWrap -> output</summary>
public interface ITerminal : IDisposable
{
	public Key<ITerminal> Key { get; }
	public string DisplayName { get; }
	public ITerminalInteractive TerminalInteractive { get; }
	public ITerminalInput TerminalInput { get; }
	public ITerminalOutput TerminalOutput { get; }
	public bool HasExecutingProcess { get; }
	
	public void EnqueueCommand(TerminalCommandRequest terminalCommandRequest);
	public void EnqueueClear();
	
	public void KillProcess();
}
