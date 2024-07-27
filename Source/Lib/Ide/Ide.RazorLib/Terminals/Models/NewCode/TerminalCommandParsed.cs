namespace Luthetus.Ide.RazorLib.Terminals.Models.NewCode;

public class TerminalCommandParsed
{
	public TerminalCommandParsed(string targetFileName, string arguments)
	{
		TargetFileName = targetFileName;
		Arguments = arguments;
	}

	public string TargetFileName { get; }
	public string Arguments { get; }
}
