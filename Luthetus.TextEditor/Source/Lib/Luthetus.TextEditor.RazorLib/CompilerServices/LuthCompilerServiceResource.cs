using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices;

public class LuthCompilerServiceResource : ILuthCompilerServiceResource
{
    public LuthCompilerServiceResource(
        ResourceUri resourceUri,
        ILuthCompilerService compilerService)
    {
        ResourceUri = resourceUri;
        CompilerService = compilerService;
    }

    public virtual ResourceUri ResourceUri { get; }
    public virtual ILuthCompilerService CompilerService { get; }
    public virtual CompilationUnit? CompilationUnit { get; set; }
    public virtual ImmutableArray<ISyntaxToken> SyntaxTokenList { get; set; } = ImmutableArray<ISyntaxToken>.Empty;

    public virtual ImmutableArray<TextEditorTextSpan> GetTokenTextSpans()
    {
        return SyntaxTokenList.Select(st => st.TextSpan).ToImmutableArray();
    }

    public virtual ImmutableArray<ITextEditorSymbol> GetSymbols()
    {
        var localCompilationUnit = CompilationUnit;

        if (localCompilationUnit is null)
            return ImmutableArray<ITextEditorSymbol>.Empty;

        return localCompilationUnit.Binder.SymbolsList
            .Where(s => s.TextSpan.ResourceUri == ResourceUri)
            .ToImmutableArray();
    }

    public virtual ImmutableArray<TextEditorDiagnostic> GetDiagnostics()
    {
        var localCompilationUnit = CompilationUnit;

        if (localCompilationUnit is null)
            return ImmutableArray<TextEditorDiagnostic>.Empty;

        return localCompilationUnit.DiagnosticsList
            .Where(s => s.TextSpan.ResourceUri == ResourceUri)
            .ToImmutableArray();
    }
}
