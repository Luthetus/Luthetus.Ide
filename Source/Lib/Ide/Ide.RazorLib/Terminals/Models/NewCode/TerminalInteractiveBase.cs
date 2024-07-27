namespace Luthetus.Ide.RazorLib.Terminals.Models.NewCode;

/// <summary>Aaa</summary>
public class TerminalInteractiveBase : ITerminalInteractive
{
	private readonly ITerminal _terminal;

	public TerminalInteractiveBase(ITerminal terminal)
	{
		_terminal = terminal;
	}

	private string? _previousWorkingDirectoryAbsolutePathString;
	private string? _workingDirectoryAbsolutePathString;
	
	public string? WorkingDirectoryAbsolutePathString => _workingDirectoryAbsolutePathString;

	public event Action? WorkingDirectoryChanged;
	
	/// <summary>
	/// Some terminal commands will map to "interactive" commands
	/// that are meant to modify the shell session rather
	/// than to start a process.
	///
	/// Ex: 'cd ../..' is an "interactive" command,
	///     where as 'dotnet run' will start a process.
	///
	/// This method gives the <see cref="ITerminalInteractive"/> an
	/// opportunity to handle the command, before it is executed
	/// at a more general level.
	///
	/// Presumably, 'dotnet run ../MyProject.csproj' where there is a
	/// ".." in a path, would need to be taken by the
	/// <see cref="ITerminalInteractive"/>, and then modified
	/// to replace "../MyProject.csproj" with the result of traversing
	/// this relative path from the working directory path.
	///
	/// The final result would then be returned for the <see cref="ITerminal"/>
	/// to execute, "dotnet run C:/User/MyProject/MyProject.cs"
	/// if the working directory were to be "C:/User/MyProject/wwwroot/".
	///
	/// If null is returned, then the <see cref="ITerminal"/>
	/// should return (do nothing more).
	/// </summary>
	public string? TryHandleCommand(string commandText)
	{
		if (terminalCommand.ChangeWorkingDirectoryTo is not null)
			TerminalInteractive.SetWorkingDirectory(terminalCommand.ChangeWorkingDirectoryTo);

		if (terminalCommand.FormattedCommand.TargetFileName == "cd")
		{
			// TODO: Don't keep this logic as it is hacky. I'm trying to set myself up to be able to run "gcc" to compile ".c" files. Then I can work on adding symbol related logic like "go to definition" or etc.
			if (terminalCommand.FormattedCommand.HACK_ArgumentsString is not null)
				SetWorkingDirectoryAbsolutePathString(terminalCommand.FormattedCommand.HACK_ArgumentsString);
			else if (terminalCommand.FormattedCommand.ArgumentsList.Any())
				SetWorkingDirectoryAbsolutePathString(terminalCommand.FormattedCommand.ArgumentsList.ElementAt(0));

			return;
		}

		if (terminalCommand.FormattedCommand.TargetFileName == "clear")
		{
			ClearTerminal();
			WriteWorkingDirectory();
			return;
		}
	}
	
	public void SetWorkingDirectory(string workingDirectoryAbsolutePathString)
	{
		_previousWorkingDirectoryAbsolutePathString = _workingDirectoryAbsolutePathString;
        _workingDirectoryAbsolutePathString = workingDirectoryAbsolutePathString;

        if (_previousWorkingDirectoryAbsolutePathString != _workingDirectoryAbsolutePathString)
            WorkingDirectoryChanged?.Invoke();
	}
	
	public void Dispose()
	{
		return;
	}
}
