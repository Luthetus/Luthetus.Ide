using CliWrap.EventStream;

namespace Luthetus.Ide.RazorLib.Terminals.Models.NewCode;

/// <summary>Output Data</summary>
public interface ITerminalOutput : ITerminalPipe
{
	public string OutputRaw { get; }

	/// <summary>
	/// TODO: Make this 'Action<Key<TerminalCommandParsed>>?' so one can
	///       track the output of a specific command as it is being executed?
	/// </summary>
	public event Action? OnWriteOutput;
	
	public void WriteOutput(TerminalCommandParsed terminalCommandParsed, CommandEvent commandEvent);
	public string GetCommandOutput(TerminalCommandParsed terminalCommandParsed);
	
	/// <summary>
	/// If the command is: "dir", and the command output is the list of directories.
	/// Then this method returns "dir".
	///
	/// Returns a string which is the result of joining
	/// all the individual commands' CommandText.
	///
	/// This value returned can change rapidly,
	/// depending on how often a terminal command is started.
	///
	/// As a result all the caching involved is done internally to this method.
	/// Instead of possibly constructing a new string every invocation,
	/// due to the data being stored as a list of objects (not one large string).
	/// </summary>
	public string AllCommandTextToString();
	
	/// <summary>
	/// If the command is: "dir", and the command output is the list of directories.
	/// Then this method returns the list of directories.
	///
	/// Returns a string which is the result of joining
	/// all the individual commands' CommandText.
	///
	/// This value returned can change rapidly,
	/// depending on how often a terminal command is started.
	///
	/// As a result all the caching involved is done internally to this method.
	/// Instead of possibly constructing a new string every invocation,
	/// due to the data being stored as a list of objects (not one large string).
	/// </summary>
	public string AllCommandOutputToString();
	
	/// <summary>
	/// If the command is: "dir", and the command output is the list of directories.
	/// Then this method returns "dir" + the list of directories.
	///
	/// Returns a string which is the result of joining
	/// all the individual commands' CommandText.
	///
	/// This value returned can change rapidly,
	/// depending on how often a terminal command is started.
	///
	/// As a result all the caching involved is done internally to this method.
	/// Instead of possibly constructing a new string every invocation,
	/// due to the data being stored as a list of objects (not one large string).
	/// </summary>
	public string AllCommandTextAndCommandOutputToString();
}
