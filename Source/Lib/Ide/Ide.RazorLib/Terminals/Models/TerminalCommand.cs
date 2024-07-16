using System.Text;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
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
	
	/// <summary>
	/// (2024-07-15)
	/// If an instance of this class has this property as non-null,
	/// then it is to mean that any terminal output be appended to this StringBuilder.
	///
	/// If an instance of this class has this property as null,
	/// then it is to mean that the terminal's text should be cleared,
	/// and then set write out this instance's FormattedCommand
	/// as a string, and any output.
	///
	/// Idea: Should one always output to the 'OutputBuilder',
	///       and then consider a 'ReadOnlyTextEditor'
	///       which accepts as input a stream?
	///
	/// I really like the idea of the 'ReadOnlyTextEditor' accepting
	/// a stream.
	///
	/// I also have a very hacky 'TextEditorKeymapTerminal.cs' class
	/// at the moment. And I can get rid of it if I use the
	/// 'WidgetBlock.cs' in the namespace
	/// 'Luthetus.TextEditor.RazorLib.TextEditors.Models;'
	///
	/// The 'WidgetBlock.cs' could be a text in and of itself,
	/// but its sort of embedded as a widget within a readonly text editor.
	///
	/// So, the integrated terminal would be two text editor models,
	/// the readonly one, and then the final line is a widget for
	/// the input.
	///
	/// Also, the execution terminal shouldn't have an editable final line,
	/// it should be purely readonly for use by the IDE to show the program's output
	/// when it runs.
	///
	/// I think I want the execution terminal to eventually use this StringBuilder too,
	/// but for now I should keep the execution terminal writing to the text editor model,
	/// and focus on getting the unit tests to use this StringBuilder, so I don't do too much
	/// all at once.
	/// </summary>
	public StringBuilder? OutputBuilder { get; set; } = new();
	public List<TextEditorTextSpan>? TextSpanList { get; set; }

	public Task InvokeStateChangedCallbackFunc()
	{
		var terminalCommandStateChangedCallbackFunc = StateChangedCallbackFunc;

		if (terminalCommandStateChangedCallbackFunc is not null)
			return terminalCommandStateChangedCallbackFunc.Invoke();

		return Task.CompletedTask;
	}
}
