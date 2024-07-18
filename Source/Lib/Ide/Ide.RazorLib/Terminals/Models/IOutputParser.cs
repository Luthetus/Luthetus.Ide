using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

/// <summary>
/// This type defines a method that can be invoked each time output is written to the terminal.
/// Furthermore, <see cref="IDisposable.Dispose"/> is invoked when the terminal command finished
/// execution, and blocks any further terminal commands from executing until the <see cref="IDisposable.Dispose"/>
/// is finished.<br/><br/>
/// </summary>
public interface IOutputParser
{
    /// <summary>
    /// This method is invoked foreach line of output, while the CLI is executing.
    /// For batching logic, one can have the implementation handle that,
    /// for example by tracking all the parsed lines, until the invocation of <see cref="OnAfterCommandFinished"/>.
    /// At this point, one could then as a batch operation apply the syntax highlighting to all lines of the
    /// commands output.
    /// </summary>
    public List<TextEditorTextSpan> OnAfterOutputLine(TerminalCommand terminalCommand, string outputLine);

	/// <summary>
	/// This method will block the terminal from executing further commands until this method finishes.
	/// </summary>
	public Task OnAfterCommandStarted(TerminalCommand terminalCommand);

	/// <summary>
	/// This method will block the terminal from executing further commands until this method finishes.
	/// </summary>
	public Task OnAfterCommandFinished(TerminalCommand terminalCommand);
}
