namespace Luthetus.Ide.RazorLib.Terminals.Models.NewCode;

/// <summary>State (i.e.: working directory)</summary>
public interface ITerminalInteractive : IDisposable
{
	private string? _workingDirectoryAbsolutePathString;
	public string? WorkingDirectoryAbsolutePathString => _workingDirectoryAbsolutePathString;
	
	public event Action? WorkingDirectoryChanged;
	
	public void SetWorkingDirectory(string workingDirectoryAbsolutePathString);
}


