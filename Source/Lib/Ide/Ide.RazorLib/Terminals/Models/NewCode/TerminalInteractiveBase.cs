namespace Luthetus.Ide.RazorLib.Terminals.Models.NewCode;

/// <summary>Aaa</summary>
public class TerminalInteractiveBase : ITerminalInteractive
{
	private string? _workingDirectoryAbsolutePathString;
	public string? WorkingDirectoryAbsolutePathString => _workingDirectoryAbsolutePathString;
	
	public void SetWorkingDirectory();
}
