namespace Luthetus.Ide.RazorLib.Terminals.Models;

/// <summary>State (i.e.: working directory)</summary>
public interface ITerminalInteractive : IDisposable
{
	/// <summary>This property is intended to be an absolute path in string form</summary>
	public string? WorkingDirectory { get; }
	
	public event Action? WorkingDirectoryChanged;
	
	public void SetWorkingDirectory(string workingDirectoryAbsolutePathString);
	
	/// <summary>
	/// Some terminal commands will map to "interactive" commands that are
	/// meant to modify the shell session rather than to start a process.
	///
	/// Ex: 'cd ../..' is an "interactive" command,
	///     where as 'dotnet run' will start a process.
	///
	/// This method gives the <see cref="ITerminalInteractive"/> an opportunity to
	/// handle the command, before it is executed at a more general level.
	///
	/// Presumably, 'dotnet run ../MyProject.csproj' where there is a ".." in a path,
	/// would need to be taken by the <see cref="ITerminalInteractive"/>, and then
	/// modified to replace "../MyProject.csproj" with the result of traversing
	/// this relative path from the working directory path.
	///
	/// The final result would then be returned for the <see cref="ITerminal"/>
	/// to execute, "dotnet run C:/User/MyProject/MyProject.cs"
	/// if the working directory were to be "C:/User/MyProject/wwwroot/".
	///
	/// If null is returned, then the <see cref="ITerminal"/>
	/// should return (do nothing more).
	/// </summary>
	public Task<TerminalCommandParsed?> TryHandleCommand(TerminalCommandRequest terminalCommandRequest);
}


