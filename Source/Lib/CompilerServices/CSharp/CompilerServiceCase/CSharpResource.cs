using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.Extensions.CompilerServices.Syntax;
using Luthetus.CompilerServices.CSharp.BinderCase;

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
	public IReadOnlyList<TextEditorTextSpan> MiscTextSpanList { get; set; } = Array.Empty<TextEditorTextSpan>();
	
	ICompilationUnit? ICompilerServiceResource.CompilationUnit { get => CompilationUnit; set => _ = value; }
    
    public IReadOnlyList<SyntaxToken> GetTokens()
    {
        return SyntaxTokenList;
    }
    
    public IReadOnlyList<TextEditorTextSpan> GetMiscTextSpans()
    {
		return MiscTextSpanList;
    }

    public IReadOnlyList<Symbol> GetSymbols()
    {
        var localCompilationUnit = CompilationUnit;

        if (localCompilationUnit is null)
            return Array.Empty<Symbol>();

        return localCompilationUnit.SymbolList;
    }

    public IReadOnlyList<TextEditorDiagnostic> GetDiagnostics()
    {
        var localCompilationUnit = CompilationUnit;

        if (localCompilationUnit?.DiagnosticList is null)
            return Array.Empty<TextEditorDiagnostic>();

        return localCompilationUnit.DiagnosticList;
    }
}