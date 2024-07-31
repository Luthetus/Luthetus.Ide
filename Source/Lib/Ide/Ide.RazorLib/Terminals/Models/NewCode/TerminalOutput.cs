using System.Collections.Immutable;
using System.Text;
using Microsoft.AspNetCore.Components.Web;
using Fluxor;
using CliWrap.EventStream;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Dialogs.States;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Symbols;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.Ide.RazorLib.Terminals.Displays.NewCode;

namespace Luthetus.Ide.RazorLib.Terminals.Models.NewCode;

public class TerminalOutput : ITerminalOutput
{
    private readonly ITerminal _terminal;
	private readonly ITextEditorService _textEditorService;
	private readonly ICompilerServiceRegistry _compilerServiceRegistry;
	private readonly IDispatcher _dispatcher;
	
	private readonly List<ITextEditorSymbol> _symbolList = new();
	private readonly List<TextEditorTextSpan> _textSpanList = new();
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
	
    public void Dispose()
    {
    	foreach (var outputFormatter in OutputFormatterList)
		{
			outputFormatter.Dispose();
		}
    }
}
