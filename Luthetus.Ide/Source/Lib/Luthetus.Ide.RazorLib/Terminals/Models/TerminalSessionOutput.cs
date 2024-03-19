using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Symbols;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

public class TerminalSessionOutput
{
	public List<string> TextLineList { get; private set; } = new();
	public List<ISymbol> SymbolList { get; private set; } = new();
	public List<ISyntaxToken> TokenList { get; private set; } = new();

	public void Clear()
	{
		TextLineList = new();
		SymbolList = new();
		TokenList = new();
	}
}
