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
	public Task EnqueueCommandAsync(TerminalCommandRequest terminalCommandRequest);
	
	/// <summary>
	/// This will enqueue the command with text "clear".
	/// Thus, it will only execute when it is its turn in the queue.
	/// </summary>
	public void ClearEnqueue();
	
	/// <summary>
	/// This will invoke the <see cref="ITerminalOutput.ClearOutput"/> method,
	/// by using '_ = Task.Run(...)'.
	///
	/// This will execute EVEN IF a command in the queue is currently being executed.
	///
	/// The fire and forget is because the terminal <see cref="ITerminalOutput"/> wraps
	/// any state mutation in a 'lock(...) { }'. So, the fire and forget is to avoid
	/// freezing the UI.
	/// </summary>
	public void ClearFireAndForget();
	
	public void KillProcess();
}
