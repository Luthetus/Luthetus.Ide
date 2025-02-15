using CliWrap.EventStream;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

/// <summary>Output Data</summary>
public interface ITerminalOutput : IDisposable
{
	public List<ITerminalOutputFormatter> OutputFormatterList { get; }

	/// <summary>
	/// TODO: Make this 'Action<Key<TerminalCommandParsed>>?' so one can
	///       track the output of a specific command as it is being executed?
	/// </summary>
	public event Action? OnWriteOutput;
	
	public void WriteOutput(TerminalCommandParsed terminalCommandParsed, CommandEvent commandEvent);
	
	/// <summary>Guaranteed to clear the output</summary>
	public void ClearOutput();
	
	/// <summary>Guaranteed to clear the output, but will keep the most recent command</summary>
	public void ClearOutputExceptMostRecentCommand();
	
	/// <summary>Conditionally clear the output</summary>
	public void ClearHistoryWhenExistingOutputTooLong();
	
	public ITerminalOutputFormatted GetOutputFormatted(string terminalOutputFormatterName);
	public TerminalCommandParsed? GetParsedCommandOrDefault(Key<TerminalCommandRequest> terminalCommandRequestKey);
	public List<TerminalCommandParsed> GetParsedCommandList();
	public int GetParsedCommandListCount();
}
