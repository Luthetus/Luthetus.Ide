using CliWrap.EventStream;

namespace Luthetus.Ide.RazorLib.Terminals.Models.NewCode;

public class TerminalOutputStringBuilder : ITerminalOutput
{
	private readonly ITerminal _terminal;

	public TerminalOutputStringBuilder(ITerminal terminal)
	{
		_terminal = terminal;
		
		_terminal.TerminalInteractive.WorkingDirectoryChanged += OnWorkingDirectoryChanged;
	}
	
	public event Action? OnWriteOutput;

	public void OnWorkingDirectoryChanged()
	{
	}
	
	public void OnHandleCommandStarting()
	{
	}
	
	public void WriteOutput(TerminalCommandParsed terminalCommandParsed, CommandEvent commandEvent)
	{
		OnWriteOutput?.Invoke();
	
		var output = (string?)null;

		switch (commandEvent)
		{
			case StartedCommandEvent started:
				// TODO: If the source of the terminal command is a user having...
				//       ...typed themselves, then hitting enter, do not write this out.
				//       |
				//       This is here for when the command was started programmatically
				//       without a user typing into the terminal.
				///////output = $"{terminalCommand.FormattedCommand.Value}\n";
				break;
			case StandardOutputCommandEvent stdOut:
				output = $"{stdOut.Text}\n";
				break;
			case StandardErrorCommandEvent stdErr:
				output = $"{stdErr.Text}\n";
				break;
			case ExitedCommandEvent exited:
				output = $"Process exited; Code: {exited.ExitCode}\n";
				break;
		}

		/*
		if (output is not null)
		{
			var outputTextSpanList = new List<TextEditorTextSpan>();

			if (terminalCommand.OutputParser is not null)
			{
				outputTextSpanList = terminalCommand.OutputParser.OnAfterOutputLine(
					terminalCommand,
					output);
			}
			
			if (terminalCommand.OutputBuilder is null)
			{
				TerminalOnOutput(
					outputOffset,
					output,
					outputTextSpanList,
					terminalCommand,
					terminalCommandBoundary);

				outputOffset += output.Length;
			}
			else
			{
				terminalCommand.OutputBuilder.Append(output);
				terminalCommand.TextSpanList = outputTextSpanList;
			}
		}
		*/
	}
	
	public void Dispose()
	{
		_terminal.TerminalInteractive.WorkingDirectoryChanged -= OnWorkingDirectoryChanged;
	}
}
