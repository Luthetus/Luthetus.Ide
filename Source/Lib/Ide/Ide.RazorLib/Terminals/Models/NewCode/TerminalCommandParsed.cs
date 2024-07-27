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
	public TerminalCommandRequest SourceTerminalCommandRequest { get; }
}
