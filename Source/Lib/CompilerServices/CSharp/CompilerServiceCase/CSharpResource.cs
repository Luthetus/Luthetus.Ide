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
	public IReadOnlyList<SyntaxToken> SyntaxTokenList { get; set; } = Array.Empty<SyntaxToken>();
	public IReadOnlyList<TextEditorTextSpan> MiscTextSpanList { get; internal set; }
	
	ICompilationUnit? ICompilerServiceResource.CompilationUnit => CompilationUnit;
    
    public IReadOnlyList<SyntaxToken> GetTokens()
    {
        return SyntaxTokenList;
    }
    
    public IReadOnlyList<TextEditorTextSpan> GetTokenTextSpans()
    {
		var tokenTextSpanList = new List<TextEditorTextSpan>();

        tokenTextSpanList.AddRange(SyntaxTokenList.Select(st => st.TextSpan));
		tokenTextSpanList.AddRange(MiscTextSpanList);

		return tokenTextSpanList;
    }

    public IReadOnlyList<Symbol> GetSymbols()
    {
        var localCompilationUnit = CompilationUnit;

        if (localCompilationUnit is null)
            return Array.Empty<Symbol>();

        return localCompilationUnit.Binder.Symbols
            .Where(s => s.TextSpan.ResourceUri == ResourceUri)
            .ToArray();
    }

    public IReadOnlyList<TextEditorDiagnostic> GetDiagnostics()
    {
        var localCompilationUnit = CompilationUnit;

        if (localCompilationUnit?.DiagnosticsList is null)
            return Array.Empty<TextEditorDiagnostic>();

        return localCompilationUnit.DiagnosticsList
            .Where(s => s.TextSpan.ResourceUri == ResourceUri)
            .ToArray();
    }
}