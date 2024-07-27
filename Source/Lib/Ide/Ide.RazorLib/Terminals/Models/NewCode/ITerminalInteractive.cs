namespace Luthetus.Ide.RazorLib.Terminals.Models.NewCode;

/// <summary>State (i.e.: working directory)</summary>
public interface ITerminalInteractive : IDisposable
{
	public string? WorkingDirectoryAbsolutePathString { get; }
	
	public event Action? WorkingDirectoryChanged;
	
	public void SetWorkingDirectory(string workingDirectoryAbsolutePathString);
}


