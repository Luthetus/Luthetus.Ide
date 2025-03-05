using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

public interface ICompilationUnit
{
	public IReadOnlyList<SyntaxToken> TokenList { get; }
	public IReadOnlyList<TextEditorTextSpan> MiscTextSpanList { get; }
	public IReadOnlyList<Symbol> SymbolList { get; }
	public IReadOnlyList<TextEditorDiagnostic> DiagnosticList { get; }
}
