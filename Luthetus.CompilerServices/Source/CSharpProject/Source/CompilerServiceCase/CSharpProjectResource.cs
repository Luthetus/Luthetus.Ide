using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.CSharpProject.CompilerServiceCase;

public class CSharpProjectResource : ILuthCompilerServiceResource
{
    public CSharpProjectResource(
        ResourceUri resourceUri,
        CSharpProjectCompilerService cSharpProjectCompilerService)
    {
        ResourceUri = resourceUri;
        CSharpProjectCompilerService = cSharpProjectCompilerService;
    }

    public ResourceUri ResourceUri { get; }
    public CSharpProjectCompilerService CSharpProjectCompilerService { get; }
    public CompilationUnit? CompilationUnit { get; set; }
    public ImmutableArray<ISyntaxToken>? SyntaxTokens { get; set; }
    public DotNetSolution.Models.Project.CSharpProject? CSharpProject { get; set; }
    public ImmutableArray<TextEditorTextSpan> TokenTextSpanList { get; internal set; }

    ILuthCompilerService ILuthCompilerServiceResource.CompilerService => CSharpProjectCompilerService;
    ImmutableArray<ISyntaxToken> ILuthCompilerServiceResource.SyntaxTokenList { get; set; } = ImmutableArray<ISyntaxToken>.Empty;

    public ImmutableArray<TextEditorDiagnostic> GetDiagnostics()
    {
        return ImmutableArray<TextEditorDiagnostic>.Empty;
    }

    /// <summary>
    /// TODO: It might be a useful optimization to evaluate these linq expressions
    /// and store the result as to not re-evaluate the linq expression over and over.
    /// </summary>
    public ImmutableArray<ITextEditorSymbol> GetSymbols()
    {
        var localCompilationUnit = CompilationUnit;

        if (localCompilationUnit is null)
            return ImmutableArray<ITextEditorSymbol>.Empty;

        return localCompilationUnit.Binder.SymbolsList;
    }

    public ImmutableArray<TextEditorTextSpan> GetTokenTextSpans()
    {
        return TokenTextSpanList;
    }
}