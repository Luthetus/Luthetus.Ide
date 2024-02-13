using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.JavaScript;

public class JavaScriptResource : ILuthCompilerServiceResource
{
    public JavaScriptResource(
        ResourceUri resourceUri,
        JavaScriptCompilerService jsCompilerService)
    {
        ResourceUri = resourceUri;
        JsCompilerService = jsCompilerService;
    }

    public ResourceUri ResourceUri { get; }
    public JavaScriptCompilerService JsCompilerService { get; }
    public CompilationUnit? CompilationUnit { get; set; }

    public ILuthCompilerService CompilerService => JsCompilerService;
    public ImmutableArray<ISyntaxToken> SyntaxTokenList { get; set; } = ImmutableArray<ISyntaxToken>.Empty;

    public ImmutableArray<TextEditorTextSpan> TokenTextSpanList { get; set; }

    public ImmutableArray<TextEditorDiagnostic> GetDiagnostics()
    {
        return ImmutableArray<TextEditorDiagnostic>.Empty;
    }

    public ImmutableArray<ITextEditorSymbol> GetSymbols()
    {
        return ImmutableArray<ITextEditorSymbol>.Empty;
    }

    public ImmutableArray<TextEditorTextSpan> GetTokenTextSpans()
    {
        return TokenTextSpanList;
    }
}