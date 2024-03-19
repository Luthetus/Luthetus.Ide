using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Symbols;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

public class TerminalSessionOutput
{
	public List<string> TextLines { get; } = new();
	public List<ISymbol> SymbolList { get; } = new();
	public List<ISyntaxToken> TokenList { get; } = new();
}
