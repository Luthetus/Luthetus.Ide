using CliWrap.EventStream;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Ide.RazorLib.Terminals.Models.NewCode;

/// <summary>Output Data</summary>
public interface ITerminalOutput : ITerminalPipe
{
	/// <summary>
	/// TODO: Make this 'Action<Key<TerminalCommandParsed>>?' so one can
	///       track the output of a specific command as it is being executed?
	/// </summary>
	public event Action? OnWriteOutput;
	
	public void WriteOutput(TerminalCommandParsed terminalCommandParsed, CommandEvent commandEvent);
	
	public string GetOutput(string terminalOutputFormatterName);
	
	public TerminalCommandParsed? GetParsedCommandOrDefault(Key<TerminalCommandRequest> terminalCommandRequestKey);
}
