using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

/// <summary>
/// This type defines a method that can be invoked each time output is written to the terminal.
/// Furthermore, <see cref="IDisposable.Dispose"/> is invoked when the terminal command finished
/// execution, and blocks any further terminal commands from executing until the <see cref="IDisposable.Dispose"/>
/// is finished.<br/><br/>
/// </summary>
public interface IOutputParser : IDisposable
{
    /// <summary>
    /// This method is invoked foreach line of output, while the CLI is executing.
    /// For batching logic, one can have the implementation handle that,
    /// for example by tracking all the parsed lines, until the invocation of <see cref="IDisposable.Dispose"/>.
    /// At this point, one could then as a batch operation apply the syntax highlighting to all lines of the
    /// commands output.
    /// </summary>
    public List<TextEditorTextSpan> ParseLine(string output);
}
