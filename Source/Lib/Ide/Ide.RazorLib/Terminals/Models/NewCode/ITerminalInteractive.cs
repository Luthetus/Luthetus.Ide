namespace Luthetus.Ide.RazorLib.Terminals.Models.NewCode;

/// <summary>State (i.e.: working directory)</summary>
public interface ITerminalInteractive
{
	private string? _workingDirectoryAbsolutePathString;
	public string? WorkingDirectoryAbsolutePathString => _workingDirectoryAbsolutePathString;
	
	public void SetWorkingDirectory();
}


