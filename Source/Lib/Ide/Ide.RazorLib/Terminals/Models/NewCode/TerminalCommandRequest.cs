namespace Luthetus.Ide.RazorLib.Terminals.Models.NewCode;

public class TerminalCommandRequest
{
	/// <summary>
	/// 'workingDirectory' is intended to be an absolute path in string form.
	///
	/// If 'workingDirectory' is null, then the WorkingDirectory of the terminal
	/// will not be changed.
	/// </summary>
	public TerminalCommandRequest(
		string commandText,
		string? workingDirectory)
	{
		CommandText = commandText;
		WorkingDirectory = workingDirectory;
	}
	
	public string CommandText { get; set; }
	
	/// <summary>
	/// This property is intended to be an absolute path in string form.
	///
	/// If this property is null, then the WorkingDirectory of the terminal
	/// will not be changed.
	/// </summary>
	public string? WorkingDirectory { get; set; }
}
