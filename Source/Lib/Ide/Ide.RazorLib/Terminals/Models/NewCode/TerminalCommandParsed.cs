using Luthetus.Common.RazorLib.Reactives.Models;

namespace Luthetus.Ide.RazorLib.Terminals.Models.NewCode;

public class TerminalCommandParsed
{
	public TerminalCommandParsed(
		string targetFileName,
		string arguments,
		TerminalCommandRequest sourceTerminalCommandRequest)
	{
		TargetFileName = targetFileName;
		Arguments = arguments;
		SourceTerminalCommandRequest = sourceTerminalCommandRequest;
	}

	public string TargetFileName { get; }
	public string Arguments { get; }
	public StringBuilderCache OutputCache { get; } = new();
	public TerminalCommandRequest SourceTerminalCommandRequest { get; }
}
