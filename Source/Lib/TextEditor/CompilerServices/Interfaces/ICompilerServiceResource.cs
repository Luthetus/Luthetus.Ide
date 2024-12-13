using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

public interface ICompilerServiceResource
{
    public ResourceUri ResourceUri { get; }
    public ICompilerService CompilerService { get; }
    public ICompilationUnit? CompilationUnit { get; }

	public IReadOnlyList<ISyntaxToken> GetTokens();
    public IReadOnlyList<TextEditorTextSpan> GetTokenTextSpans();
    public IReadOnlyList<ITextEditorSymbol> GetSymbols();
    public IReadOnlyList<TextEditorDiagnostic> GetDiagnostics();
}
