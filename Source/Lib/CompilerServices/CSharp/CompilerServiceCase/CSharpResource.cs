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
	public IReadOnlyList<ISyntaxToken> SyntaxTokenList { get; set; } = ImmutableArray<ISyntaxToken>.Empty;
	public IReadOnlyList<TextEditorTextSpan> EscapeCharacterList { get; internal set; }
	
	ICompilationUnit? ICompilerServiceResource.CompilationUnit => CompilationUnit;
    
    public IReadOnlyList<ISyntaxToken> GetTokens()
    {
        return SyntaxTokenList;
    }
    
    public IReadOnlyList<TextEditorTextSpan> GetTokenTextSpans()
    {
		var tokenTextSpanList = new List<TextEditorTextSpan>();

        tokenTextSpanList.AddRange(SyntaxTokenList.Select(st => st.TextSpan));
		tokenTextSpanList.AddRange(EscapeCharacterList);

		return tokenTextSpanList;
    }

    public IReadOnlyList<ITextEditorSymbol> GetSymbols()
    {
        var localCompilationUnit = CompilationUnit;

        if (localCompilationUnit is null)
            return Array.Empty<ITextEditorSymbol>();

        return localCompilationUnit.Binder.Symbols
            .Where(s => s.TextSpan.ResourceUri == ResourceUri)
            .ToArray();
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