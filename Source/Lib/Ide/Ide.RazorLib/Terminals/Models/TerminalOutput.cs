using System.Collections.Immutable;
using CliWrap.EventStream;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

public class TerminalOutput : ITerminalOutput
{
    private readonly ITerminal _terminal;
	
	private readonly List<TerminalCommandParsed> _parsedCommandList = new();
	private readonly object _listLock = new();

	public TerminalOutput(ITerminal terminal)
	{
		_terminal = terminal;
	}

	public TerminalOutput(
			ITerminal terminal,
			ITerminalOutputFormatter outputFormatter)
		: this(terminal)
	{
		OutputFormatterList = new ITerminalOutputFormatter[]
		{
			outputFormatter
		}.ToImmutableList();
	}
	
	public TerminalOutput(
			ITerminal terminal,
			ImmutableList<ITerminalOutputFormatter> outputFormatterList)
		: this(terminal)
	{
		OutputFormatterList = outputFormatterList;
	}
	
	public ImmutableList<ITerminalOutputFormatter> OutputFormatterList { get; private set; }
	
	public event Action? OnWriteOutput;
	
	public ITerminalOutputFormatted? GetOutputFormatted(string terminalOutputFormatterName)
	{
		var outputFormatter = OutputFormatterList.FirstOrDefault(x =>
			x.Name == terminalOutputFormatterName);
			
		if (outputFormatter is null)
			return null;
			
		return outputFormatter.Format();
	}
	
	public TerminalCommandParsed? GetParsedCommandOrDefault(Key<TerminalCommandRequest> terminalCommandRequestKey)
	{
		lock (_listLock)
		{
			return _parsedCommandList.FirstOrDefault(x =>
				x.SourceTerminalCommandRequest.Key == terminalCommandRequestKey);
		}
	}
	
	public ImmutableList<TerminalCommandParsed> GetParsedCommandList()
	{
		lock (_listLock)
		{
			return _parsedCommandList.ToImmutableList();
		}
	}
	
	public int GetParsedCommandListCount()
	{
		lock (_listLock)
		{
			return _parsedCommandList.Count;
		}
	}
	
	public void RegisterOutputFormatterCustom(ITerminalOutputFormatter outputFormatter)
	{
		lock (_listLock)
		{
			OutputFormatterList = OutputFormatterList.Add(outputFormatter);
		}
	}
	
	public void WriteOutput(TerminalCommandParsed terminalCommandParsed, CommandEvent commandEvent)
	{
		var output = (string?)null;

		switch (commandEvent)
		{
			case StartedCommandEvent started:
				
				// Delete any output of the previous invocation.
				lock (_listLock)
				{
					var indexPreviousOutput = _parsedCommandList.FindIndex(x =>
						x.SourceTerminalCommandRequest.Key ==
							terminalCommandParsed.SourceTerminalCommandRequest.Key);
							
					if (indexPreviousOutput != -1)
						_parsedCommandList.RemoveAt(indexPreviousOutput);
						
					_parsedCommandList.Add(terminalCommandParsed);
				}
				
				break;
			case StandardOutputCommandEvent stdOut:
				terminalCommandParsed.OutputCache.AppendTwo(stdOut.Text, "\n");
				break;
			case StandardErrorCommandEvent stdErr:
				terminalCommandParsed.OutputCache.AppendTwo(stdErr.Text, "\n");
				break;
			case ExitedCommandEvent exited:
				break;
		}
		
		OnWriteOutput?.Invoke();
	}
	
	public void ClearOutput()
	{
		lock (_listLock)
		{
			_parsedCommandList.Clear();
		}

		OnWriteOutput?.Invoke();
	}
	
	public void ClearOutputExceptMostRecentCommand()
	{
		lock (_listLock)
		{
			var rememberLastCommand = _parsedCommandList.LastOrDefault();
			
			_parsedCommandList.Clear();
			
			if (rememberLastCommand is not null &&
				rememberLastCommand.OutputCache.GetLength() < TerminalOutputFacts.MAX_OUTPUT_LENGTH)
			{
				_parsedCommandList.Add(rememberLastCommand);
			}
		}

		OnWriteOutput?.Invoke();
	}
	
	public void ClearHistoryWhenExistingOutputTooLong()
	{
		lock (_listLock)
		{
			var sumOutputLength = _parsedCommandList.Sum(x => x.OutputCache.GetLength());

			if (sumOutputLength > TerminalOutputFacts.MAX_OUTPUT_LENGTH ||
				_parsedCommandList.Count > TerminalOutputFacts.MAX_COMMAND_COUNT)
			{
				var rememberLastCommand = _parsedCommandList.LastOrDefault();
			
				_parsedCommandList.Clear();
				
				if (rememberLastCommand is not null &&
					rememberLastCommand.OutputCache.GetLength() < TerminalOutputFacts.OUTPUT_LENGTH_PADDING)
				{
					// It feels odd to clear the entire terminal when there is too much text output
					// that has accumulated.
					//
					// So, keep the most recent command's output,
					// unless its output length is greater than or equal to the TerminalOutputFacts.OUTPUT_LENGTH_PADDING.
					_parsedCommandList.Add(rememberLastCommand);
				}
			}
		}

		OnWriteOutput?.Invoke();
	}
	
    public void Dispose()
    {
    	foreach (var outputFormatter in OutputFormatterList)
		{
			outputFormatter.Dispose();
		}
    }
}
