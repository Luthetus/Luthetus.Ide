using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.CompilerServices.CSharp.CompilerServiceCase;

public sealed class CSharpResource : ICompilerServiceResource
{
    public CSharpResource(ResourceUri resourceUri, CSharpCompilerService cSharpCompilerService)
    {
    	ResourceUri = resourceUri;
        CompilerService = cSharpCompilerService;
    }
	
	public ResourceUri ResourceUri { get; }
    public ICompilerService CompilerService { get; }
	public CSharpCompilationUnit? CompilationUnit { get; set; }
	public IReadOnlyList<TextEditorTextSpan>? TokenTextSpanList { get; set; }
	public IReadOnlyList<ITextEditorSymbol>? SymbolList { get; set; }
	
	ICompilationUnit? ICompilerServiceResource.CompilationUnit => CompilationUnit;
    
    public IReadOnlyList<ISyntaxToken> GetTokens()
    {
        return Array.Empty<ISyntaxToken>();
    }
    
    public IReadOnlyList<TextEditorTextSpan> GetTokenTextSpans()
    {
    	return TokenTextSpanList ?? Array.Empty<TextEditorTextSpan>();
    }

    public IReadOnlyList<ITextEditorSymbol> GetSymbols()
    {
    	return SymbolList ?? Array.Empty<ITextEditorSymbol>();
    }

    public IReadOnlyList<TextEditorDiagnostic> GetDiagnostics()
    {
        var localCompilationUnit = CompilationUnit;

        if (localCompilationUnit is null)
            return Array.Empty<TextEditorDiagnostic>();

		// TODO: (2024-12-12)
        /*return localCompilationUnit.DiagnosticsList
            .Where(s => s.TextSpan.ResourceUri == ResourceUri)
            .ToArray();*/
        return Array.Empty<TextEditorDiagnostic>();
    }
}