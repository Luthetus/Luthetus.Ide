using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices;

public class CompilerServiceResource : ICompilerServiceResource
{
    public CompilerServiceResource(
        ResourceUri resourceUri,
        ICompilerService compilerService)
    {
        ResourceUri = resourceUri;
        CompilerService = compilerService;
    }

    public virtual ResourceUri ResourceUri { get; }
    public virtual ICompilerService CompilerService { get; }
    public virtual CompilationUnit? CompilationUnit { get; set; }
    public virtual IReadOnlyList<ISyntaxToken> SyntaxTokenList { get; set; } = ImmutableArray<ISyntaxToken>.Empty;

	ICompilationUnit? ICompilerServiceResource.CompilationUnit => CompilationUnit;

    public virtual IReadOnlyList<ISyntaxToken> GetTokens()
    {
        return SyntaxTokenList;
    }
    
    public virtual IReadOnlyList<TextEditorTextSpan> GetTokenTextSpans()
    {
        return SyntaxTokenList.Select(st => st.TextSpan).ToArray();
    }

    public virtual IReadOnlyList<ITextEditorSymbol> GetSymbols()
    {
        var localCompilationUnit = CompilationUnit;

        if (localCompilationUnit is null)
            return Array.Empty<ITextEditorSymbol>();

        return localCompilationUnit.Binder.SymbolsList
            .Where(s => s.TextSpan.ResourceUri == ResourceUri)
            .ToArray();
    }

    public virtual IReadOnlyList<TextEditorDiagnostic> GetDiagnostics()
    {
        var localCompilationUnit = CompilationUnit;

        if (localCompilationUnit is null)
            return Array.Empty<TextEditorDiagnostic>();

        return localCompilationUnit.DiagnosticsList
            .Where(s => s.TextSpan.ResourceUri == ResourceUri)
            .ToArray();
    }
}
