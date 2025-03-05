using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;

public class CompilationUnit : ICompilationUnit
{
	public IReadOnlyList<SyntaxToken> TokenList { get; init; } = Array.Empty<SyntaxToken>();
	public IReadOnlyList<TextEditorDiagnostic> DiagnosticList { get; init; } = Array.Empty<TextEditorDiagnostic>();
	
	public IEnumerable<TextEditorTextSpan> GetTextTextSpans()
	{
		return TokenList.Select(x => x.TextSpan);
	}
    
    public IEnumerable<TextEditorTextSpan> GetDiagnosticTextSpans()
    {
    	return DiagnosticList.Select(x => x.TextSpan);
    }
}
