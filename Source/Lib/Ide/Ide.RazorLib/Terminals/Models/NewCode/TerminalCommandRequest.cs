using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Ide.RazorLib.Terminals.Models.NewCode;

/// <summary>
/// 'workingDirectory' is intended to be an absolute path in string form.
///
/// If 'workingDirectory' is null, then the WorkingDirectory of the terminal
/// will not be changed.
/// </summary>
public class TerminalCommandRequest
{
	/// <inheritdoc cref="TerminalCommandRequest"/>
	public TerminalCommandRequest(
			string commandText,
			string? workingDirectory)
		: this(
			commandText,
			workingDirectory,
			Key<TerminalCommandRequest>.NewKey())
	{
	}

	/// <inheritdoc cref="TerminalCommandRequest"/>
	public TerminalCommandRequest(
		string commandText,
		string? workingDirectory,
		Key<TerminalCommandRequest> key)
	{
		CommandText = commandText;
		WorkingDirectory = workingDirectory;
		Key = key;
	}
	
	public Key<TerminalCommandRequest> Key { get; }
	
	public string CommandText { get; }
	
	/// <inheritdoc cref="TerminalCommandRequest"/>
	public string? WorkingDirectory { get; }
	
	/// <summary>
	/// Input: the parsed terminal command that is about to begin execution.
	/// 
	/// This Func will block the terminal from executing further terminal
	/// commands in the queue until it has returned.
	/// </summary>
	public Func<TerminalCommandParsed, Task> BeginWith { get; init; }
	
	/// <summary>
	/// Input: the parsed terminal command that just finished.
	/// 
	/// This Func will block the terminal from executing further terminal
	/// commands in the queue until it has returned.
	/// </summary>
	public Func<TerminalCommandParsed, Task> ContinueWith { get; init; }
}
