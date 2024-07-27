using CliWrap.EventStream;

namespace Luthetus.Ide.RazorLib.Terminals.Models.NewCode;

/// <summary>Output Data</summary>
public interface ITerminalOutput : ITerminalPipe
{
	public string Output { get; }

	public event Action? OnWriteOutput;
	
	public void WriteOutput(TerminalCommandParsed terminalCommandParsed, CommandEvent commandEvent);
}
