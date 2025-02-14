using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

public class TerminalOutputFormattedTextEditor : ITerminalOutputFormatted
{
	public TerminalOutputFormattedTextEditor(
		string text,
		ImmutableList<TerminalCommandParsed> parsedCommandList,
		ImmutableList<TextEditorTextSpan> textSpanList,
		ImmutableList<Symbol> symbolList)
	{
		Text = text;
		ParsedCommandList = parsedCommandList;
		TextSpanList = textSpanList;
		SymbolList = symbolList;
	}

	public string Text { get; }
	public ImmutableList<TerminalCommandParsed> ParsedCommandList { get; }
	public ImmutableList<TextEditorTextSpan> TextSpanList { get; }
	public ImmutableList<Symbol> SymbolList { get; }
}
