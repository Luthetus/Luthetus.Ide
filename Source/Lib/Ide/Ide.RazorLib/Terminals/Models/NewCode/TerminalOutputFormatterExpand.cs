using System.Text;

namespace Luthetus.Ide.RazorLib.Terminals.Models.NewCode;

public class TerminalOutputFormatterExpand : ITerminalOutputFormatter
{
	public const string NAME = nameof(TerminalOutputFormatterExpand);

	public string Name { get; } = NAME;
	
	public string Format(ITerminal terminal)
	{
		var symbolList = terminal.TerminalOutput.GetSymbolList();
		var textSpanList = terminal.TerminalOutput.GetTextSpanList();
		var parsedCommandList = terminal.TerminalOutput.GetParsedCommandList();
		
		var outputBuilder = new StringBuilder();
		
		foreach (var parsedCommand in parsedCommandList)
		{
			outputBuilder
				.Append(parsedCommand.SourceTerminalCommandRequest.CommandText)
				.Append('\n')
				.Append(parsedCommand.OutputCache.ToString());
		}
		
		/*
		var workingDirectoryTextSpan = new TextEditorTextSpan(
			_inputBuilder.Length,
	        _inputBuilder.Length + workingDirectoryText.Length,
	        (byte)TerminalDecorationKind.Keyword,
	        ResourceUri.Empty,
	        string.Empty,
	        workingDirectoryText);
	    _textEditorTextSpanList.Add(workingDirectoryTextSpan);
		
		_inputBuilder.Append(workingDirectoryText);
		
		var commandTextTextSpan = new TextEditorTextSpan(
			_inputBuilder.Length,
	        _inputBuilder.Length + terminalCommandParsed.SourceTerminalCommandRequest.CommandText.Length,
	        (byte)0,
	        ResourceUri.Empty,
	        string.Empty,
	        terminalCommandParsed.SourceTerminalCommandRequest.CommandText);
	        
		var commandTextSymbol = new OnClickSymbol(
			commandTextTextSpan,
			"View Output",
			() => OpenInEditor(terminalCommandParsed));
			
		_textEditorSymbolList.Add(commandTextSymbol);
		
		var targetFileNameTextSpan = new TextEditorTextSpan(
			_inputBuilder.Length,
	        _inputBuilder.Length + terminalCommandParsed.TargetFileName.Length,
	        (byte)TerminalDecorationKind.TargetFilePath,
	        ResourceUri.Empty,
	        string.Empty,
	        terminalCommandParsed.TargetFileName);
	    _textEditorTextSpanList.Add(targetFileNameTextSpan);
		
		_inputBuilder.Append($"{terminalCommandParsed.SourceTerminalCommandRequest.CommandText}\n");
		*/
		
		return outputBuilder.ToString();
	}
}
