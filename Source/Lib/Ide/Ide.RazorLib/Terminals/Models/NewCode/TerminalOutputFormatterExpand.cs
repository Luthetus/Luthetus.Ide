using System.Text;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.Ide.RazorLib.Terminals.Models.NewCode;

public class TerminalOutputFormatterExpand : ITerminalOutputFormatter
{
	public const string NAME = nameof(TerminalOutputFormatterExpand);

	public string Name { get; } = NAME;
	
	public string Format(ITerminal terminal)
	{
		var outSymbolList = new List<ITextEditorSymbol>();
		var outTextSpanList = new List<TextEditorTextSpan>();
		
		var parsedCommandList = terminal.TerminalOutput.GetParsedCommandList();
		
		var outputBuilder = new StringBuilder();
		
		foreach (var parsedCommand in parsedCommandList)
		{
			var workingDirectoryText = parsedCommand.SourceTerminalCommandRequest.WorkingDirectory + "> ";
		
			var workingDirectoryTextSpan = new TextEditorTextSpan(
				outputBuilder.Length,
		        outputBuilder.Length + workingDirectoryText.Length,
		        (byte)TerminalDecorationKind.Keyword,
		        ResourceUri.Empty,
		        string.Empty,
		        workingDirectoryText);
		    outTextSpanList.Add(workingDirectoryTextSpan);
			
			outputBuilder
				.Append(workingDirectoryText)
				.Append(parsedCommand.SourceTerminalCommandRequest.CommandText)
				.Append('\n')
				.Append(parsedCommand.OutputCache.ToString());
		}
		
		terminal.TerminalOutput.SetSymbolList(outSymbolList);
		
		/*
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
