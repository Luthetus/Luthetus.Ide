using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.Ide.RazorLib.CommandLines.Models;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

public record TerminalCommand(
    Key<TerminalCommand> TerminalCommandKey,
    FormattedCommand FormattedCommand,
    string? ChangeWorkingDirectoryTo = null,
    CancellationToken CancellationToken = default,
    Func<Task>? ContinueWith = null,
	Func<Task>? BeginWith = null,
    IOutputParser? OutputParser = null)
{
	/// <summary>
	/// (2024-05-20)
	/// Looking into adding these 3 properties:<br/>
	/// -TextEditorTextSpan? TextSpan<br/>
	/// -bool WasWrittenTo<br/>
	/// -bool IsCompleted<br/>
	/// <br/><br/>
	/// Issue is whether these properties then make
	/// all instances of this type a '1 usage' scenario?
	/// </summary>
	public TextEditorTextSpan? TextSpan { get; set; }
    public bool WasWrittenTo { get; set; }
    public bool WasStarted { get; set; }
    public bool IsCompleted { get; set; }
	public Func<Task> StateChangedCallbackFunc { get; internal set; }

	public Task InvokeStateChangedCallbackFunc()
	{
		var terminalCommandStateChangedCallbackFunc = StateChangedCallbackFunc;

		if (terminalCommandStateChangedCallbackFunc is not null)
			return terminalCommandStateChangedCallbackFunc.Invoke();

		return Task.CompletedTask;
	}
}
