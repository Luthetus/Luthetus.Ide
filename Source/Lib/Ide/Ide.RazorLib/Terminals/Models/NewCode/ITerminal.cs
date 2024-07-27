using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Ide.RazorLib.Terminals.Models.NewCode;

/// <summary>input -> CliWrap -> output</summary>
public interface ITerminal : IDisposable
{
	public Key<ITerminal> Key { get; }
	public string DisplayName { get; }
	public ITerminalInteractive TerminalInteractive { get; }
	public ITerminalInput TerminalInput { get; }
	public ITerminalOutput TerminalOutput { get; }
	
	public void EnqueueCommand(string commandText);
}
