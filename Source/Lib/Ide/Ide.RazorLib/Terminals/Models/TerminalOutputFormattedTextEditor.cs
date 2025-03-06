using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.Extensions.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

public class TerminalOutputFormattedTextEditor : ITerminalOutputFormatted
{
	public TerminalOutputFormattedTextEditor(
		string text,
		List<TerminalCommandParsed> parsedCommandList,
		List<TextEditorTextSpan> textSpanList,
		List<Symbol> symbolList)
	{
		Text = text;
		ParsedCommandList = parsedCommandList;
		TextSpanList = textSpanList;
		SymbolList = symbolList;
	}

	public string Text { get; }
	public List<TerminalCommandParsed> ParsedCommandList { get; }
	public List<TextEditorTextSpan> TextSpanList { get; }
	public List<Symbol> SymbolList { get; }
}
