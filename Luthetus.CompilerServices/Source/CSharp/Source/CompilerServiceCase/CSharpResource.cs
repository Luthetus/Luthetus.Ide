using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.CSharp.CompilerServiceCase;

public class CSharpResource : ICompilerServiceResource
{
    public CSharpResource(
        ResourceUri resourceUri,
        CSharpCompilerService cSharpCompilerService)
    {
        ResourceUri = resourceUri;
        CSharpCompilerService = cSharpCompilerService;
    }

    public ResourceUri ResourceUri { get; }
    public CSharpCompilerService CSharpCompilerService { get; }
    public CompilationUnit? CompilationUnit { get; internal set; }
    public ImmutableArray<ISyntaxToken> SyntaxTokens { get; internal set; } = ImmutableArray<ISyntaxToken>.Empty;

    public ImmutableArray<TextEditorTextSpan> SyntacticTextSpans => GetSyntacticTextSpans();
    public ImmutableArray<ITextEditorSymbol> Symbols => GetSymbols();
    public ImmutableArray<TextEditorDiagnostic> Diagnostics => GetDiagnostics();

    /// <summary>
    /// TODO: It might be a useful optimization to evaluate these linq expressions
    /// and store the result as to not re-evaluate the linq expression over and over.
    /// </summary>
    private ImmutableArray<TextEditorTextSpan> GetSyntacticTextSpans()
    {
        return SyntaxTokens.Select(st => st.TextSpan).ToImmutableArray();
    }

    /// <summary>
    /// TODO: It might be a useful optimization to evaluate these linq expressions
    /// and store the result as to not re-evaluate the linq expression over and over.
    /// </summary>
    private ImmutableArray<ITextEditorSymbol> GetSymbols()
    {
        var localCompilationUnit = CompilationUnit;

        if (localCompilationUnit is null)
            return ImmutableArray<ITextEditorSymbol>.Empty;

        return localCompilationUnit.Binder.SymbolsBag
            .Where(s => s.TextSpan.ResourceUri == ResourceUri)
            .ToImmutableArray();
    }
    
    /// <summary>
    /// TODO: It might be a useful optimization to evaluate these linq expressions
    /// and store the result as to not re-evaluate the linq expression over and over.
    /// </summary>
    private ImmutableArray<TextEditorDiagnostic> GetDiagnostics()
    {
        var localCompilationUnit = CompilationUnit;

        if (localCompilationUnit is null)
            return ImmutableArray<TextEditorDiagnostic>.Empty;

        return localCompilationUnit.DiagnosticsBag
            .Where(s => s.TextSpan.ResourceUri == ResourceUri)
            .ToImmutableArray();
    }

    /// <returns>
    /// The <see cref="ISyntaxNode"/>
    /// which represents the resource in the compilation result.
    /// </returns>
    public Task GetRootSyntaxNodeAsync()
    {
        //CSharpCompilerService.Compilation.RootSyntaxNode;
        return Task.CompletedTask;
    }
}