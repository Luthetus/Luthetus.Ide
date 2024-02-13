using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

public interface ILuthCompilerServiceResource
{
    public CompilationUnit? CompilationUnit { get; set; }

    public ResourceUri ResourceUri { get; }
    public ILuthCompilerService CompilerService { get; }
    public ImmutableArray<ISyntaxToken> SyntaxTokenList { get; set; }

    public ImmutableArray<TextEditorTextSpan> GetTokenTextSpans();
    public ImmutableArray<ITextEditorSymbol> GetSymbols();
    public ImmutableArray<TextEditorDiagnostic> GetDiagnostics();
}
