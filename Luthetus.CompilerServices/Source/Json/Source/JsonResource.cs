using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.Json;

public class JsonResource : ILuthCompilerServiceResource
{
    public JsonResource(
        ResourceUri resourceUri,
        JsonCompilerService jsonCompilerService)
    {
        ResourceUri = resourceUri;
        JsonCompilerService = jsonCompilerService;
    }

    public ResourceUri ResourceUri { get; }
    public JsonCompilerService JsonCompilerService { get; }
    public ImmutableArray<TextEditorTextSpan>? SyntacticTextSpans { get; internal set; }
    public CompilationUnit? CompilationUnit { get; set; }

    public ImmutableArray<ISyntaxToken> SyntaxTokenList { get; set; } = ImmutableArray<ISyntaxToken>.Empty;

    public ImmutableArray<TextEditorTextSpan> TokenTextSpanList { get; set; }

    public ILuthCompilerService CompilerService => JsonCompilerService;

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