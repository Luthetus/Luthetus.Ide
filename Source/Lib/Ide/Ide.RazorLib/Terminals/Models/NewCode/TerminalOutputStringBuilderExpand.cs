using System.Text;
using System.Collections.Immutable;
using CliWrap.EventStream;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Ide.RazorLib.Terminals.Models.NewCode;

public class TerminalOutputStringBuilderExpand : ITerminalOutput
{
	private readonly ITerminal _terminal;
	
	// TODO: This property is horrific to look at its defined over 3 lines? Don't do this?
	private readonly 
		List<(TerminalCommandParsed terminalCommandParsed, StringBuilder outputBuilder)>
		_commandOutputList = new(); 
		
	private readonly object _commandOutputListLock = new();

	public TerminalOutputStringBuilderExpand(ITerminal terminal)
	{
		_terminal = terminal;
		_terminal.TerminalInteractive.WorkingDirectoryChanged += OnWorkingDirectoryChanged;
	}
	
	public string OutputRaw { get; } = string.Empty;
	
	public event Action? OnWriteOutput;
	
	public ImmutableList<(TerminalCommandParsed terminalCommandParsed, StringBuilder outputBuilder)> GetCommandOutputList()
	{
		lock (_commandOutputListLock)
		{
			return _commandOutputList.ToImmutableList();
		}
	}

	public void OnWorkingDirectoryChanged()
	{
	}
	
	public void OnHandleCommandStarting()
	{
	}
	
	public void WriteOutput(TerminalCommandParsed terminalCommandParsed, CommandEvent commandEvent)
	{
		var output = (string?)null;

		switch (commandEvent)
		{
			case StartedCommandEvent started:
			
				// Delete any output of the previous invocation.
				lock (_commandOutputListLock)
				{
					var indexPreviousOutput = _commandOutputList.FindIndex(x =>
						x.terminalCommandParsed.SourceTerminalCommandRequest.Key ==
							terminalCommandParsed.SourceTerminalCommandRequest.Key);
							
					if (indexPreviousOutput != -1)
						_commandOutputList.RemoveAt(indexPreviousOutput);
				}
				
				output = $"{terminalCommandParsed.SourceTerminalCommandRequest.CommandText}\n";
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
		
		lock (_commandOutputListLock)
		{
			var indexPreviousOutput = _commandOutputList.FindIndex(x =>
				x.terminalCommandParsed.SourceTerminalCommandRequest.Key ==
					terminalCommandParsed.SourceTerminalCommandRequest.Key);
		
			if (indexPreviousOutput == -1)
			{
				_commandOutputList.Add(
					(terminalCommandParsed, new StringBuilder(output)));
			}
			else
			{
				var commandTuple = _commandOutputList[indexPreviousOutput];
				
				if (commandTuple.outputBuilder is null)
					commandTuple.outputBuilder = new(output);
					
				commandTuple.outputBuilder.Append(output);
			}
		}
		
		OnWriteOutput?.Invoke();
	}
	
	public void Dispose()
	{
		_terminal.TerminalInteractive.WorkingDirectoryChanged -= OnWorkingDirectoryChanged;
	}
}
