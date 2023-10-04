using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.CSharpProject.CompilerServiceCase;

public class CSharpProjectResource : ICompilerServiceResource
{
    public CSharpProjectResource(
        ResourceUri resourceUri,
        CSharpProjectCompilerService cSharpProjectCompilerService)
    {
        ResourceUri = resourceUri;
        DotNetCompilerService = cSharpProjectCompilerService;
    }

    public ResourceUri ResourceUri { get; }
    public CSharpProjectCompilerService DotNetCompilerService { get; }
    public CompilationUnit? CompilationUnit { get; internal set; }
    public ImmutableArray<ISyntaxToken>? SyntaxTokens { get; internal set; }
    public DotNetSolution.CSharp.CSharpProject? CSharpProject { get; set; }

    public ImmutableArray<TextEditorTextSpan> SyntacticTextSpans { get; set; } = ImmutableArray<TextEditorTextSpan>.Empty;
    public ImmutableArray<ITextEditorSymbol> Symbols => GetSymbols();

    /// <summary>
    /// TODO: It might be a useful optimization to evaluate these linq expressions
    /// and store the result as to not re-evaluate the linq expression over and over.
    /// </summary>
    private ImmutableArray<ITextEditorSymbol> GetSymbols()
    {
        var localCompilationUnit = CompilationUnit;

        if (localCompilationUnit is null)
            return ImmutableArray<ITextEditorSymbol>.Empty;

        return localCompilationUnit.Binder.SymbolsBag;
    }

    /// <returns>
    /// The <see cref="ISyntaxNode"/>
    /// which represents the resource in the compilation result.
    /// </returns>
    public Task GetRootSyntaxNodeAsync()
    {
        //DotNetCompilerService.Compilation.RootSyntaxNode;
        return Task.CompletedTask;
    }
}