using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

public interface ICompilerServiceResource
{
    public ResourceUri ResourceUri { get; }
    public ICompilerService CompilerService { get; }
    public CompilationUnit? CompilationUnit { get; set; }
    public ImmutableArray<ISyntaxToken> SyntaxTokenList { get; set; }

    public ImmutableArray<TextEditorTextSpan> GetTokenTextSpans();
    public ImmutableArray<ITextEditorSymbol> GetSymbols();
    public ImmutableArray<TextEditorDiagnostic> GetDiagnostics();
}
