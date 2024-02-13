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

    public ResourceUri ResourceUri { get; }
    public ILuthCompilerService CompilerService { get; }
    public CompilationUnit? CompilationUnit { get; set; }
    public ImmutableArray<ISyntaxToken> SyntaxTokenList { get; set; } = ImmutableArray<ISyntaxToken>.Empty;

    /// <summary>
    /// TODO: It might be a useful optimization to evaluate these linq expressions...
    /// ...and store the result as to not re-evaluate the linq expression over and over.
    /// </summary>
    public ImmutableArray<TextEditorTextSpan> GetTokenTextSpans()
    {
        return SyntaxTokenList.Select(st => st.TextSpan).ToImmutableArray();
    }

    /// <summary>
    /// TODO: It might be a useful optimization to evaluate these linq expressions...
    /// ...and store the result as to not re-evaluate the linq expression over and over.
    /// </summary>
    public ImmutableArray<ITextEditorSymbol> GetSymbols()
    {
        var localCompilationUnit = CompilationUnit;

        if (localCompilationUnit is null)
            return ImmutableArray<ITextEditorSymbol>.Empty;

        return localCompilationUnit.Binder.SymbolsList
            .Where(s => s.TextSpan.ResourceUri == ResourceUri)
            .ToImmutableArray();
    }

    /// <summary>
    /// TODO: It might be a useful optimization to evaluate these linq expressions...
    /// ...and store the result as to not re-evaluate the linq expression over and over.
    /// </summary>
    public ImmutableArray<TextEditorDiagnostic> GetDiagnostics()
    {
        var localCompilationUnit = CompilationUnit;

        if (localCompilationUnit is null)
            return ImmutableArray<TextEditorDiagnostic>.Empty;

        return localCompilationUnit.DiagnosticsList
            .Where(s => s.TextSpan.ResourceUri == ResourceUri)
            .ToImmutableArray();
    }
}
