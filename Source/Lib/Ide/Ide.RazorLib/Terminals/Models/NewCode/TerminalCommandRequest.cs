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
}
