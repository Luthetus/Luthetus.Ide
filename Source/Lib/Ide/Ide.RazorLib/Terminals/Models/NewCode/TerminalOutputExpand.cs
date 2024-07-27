using System.Text;
using System.Collections.Immutable;
using CliWrap.EventStream;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Ide.RazorLib.Terminals.Models.NewCode;

public class TerminalOutputExpand : ITerminalOutput
{
	private readonly ITerminal _terminal;
	
	// TODO: This property is horrific to look at its defined over 4 lines? Don't do this?
	private readonly Dictionary<
			Key<TerminalCommandRequest>,
			(TerminalCommandParsed terminalCommandParsed, StringBuilder outputBuilder)>
		_commandOutputMap = new(); 
		
	private readonly object _commandOutputMapLock = new();

	public TerminalOutputExpand(ITerminal terminal)
	{
		_terminal = terminal;
		
		_terminal.TerminalInteractive.WorkingDirectoryChanged += OnWorkingDirectoryChanged;
	}
	
	public string OutputRaw { get; } = null!;
	
	public event Action? OnWriteOutput;
	
	public ImmutableDictionary<Key<TerminalCommandRequest>, (TerminalCommandParsed terminalCommandParsed, StringBuilder outputBuilder)> GetCommandOutputMap()
	{
		lock (_commandOutputMapLock)
		{
			return _commandOutputMap.ToImmutableDictionary();
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
			
				lock (_commandOutputMapLock)
				{
					// Delete any output of the previous invocation.
					if (_commandOutputMap.ContainsKey(terminalCommandParsed.SourceTerminalCommandRequest.Key))
						_commandOutputMap.Remove(terminalCommandParsed.SourceTerminalCommandRequest.Key);
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
		
		lock (_commandOutputMapLock)
		{
			if (_commandOutputMap.TryGetValue(
					terminalCommandParsed.SourceTerminalCommandRequest.Key,
					out var commandTuple))
			{
				if (commandTuple.outputBuilder is null)
					commandTuple.outputBuilder = new();
					
				commandTuple.outputBuilder.Append(output);
			}
			else
			{
				_commandOutputMap.Add(
					terminalCommandParsed.SourceTerminalCommandRequest.Key,
					(terminalCommandParsed, new StringBuilder(output)));
			}
		}
		
		OnWriteOutput?.Invoke();
	}
	
	public void Dispose()
	{
		_terminal.TerminalInteractive.WorkingDirectoryChanged -= OnWorkingDirectoryChanged;
	}
}
