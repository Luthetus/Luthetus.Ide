using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.FSharp;

public class FSharpResource : ILuthCompilerServiceResource
{
    public FSharpResource(
        ResourceUri resourceUri,
        FSharpCompilerService fSharpCompilerService)
    {
        ResourceUri = resourceUri;
        FSharpCompilerService = fSharpCompilerService;
    }

    public ResourceUri ResourceUri { get; }
    public FSharpCompilerService FSharpCompilerService { get; }
    public ImmutableArray<TextEditorTextSpan>? SyntacticTextSpans { get; internal set; }
    CompilationUnit? ILuthCompilerServiceResource.CompilationUnit { get; set; }

    ILuthCompilerService ILuthCompilerServiceResource.CompilerService => FSharpCompilerService;
    ImmutableArray<ISyntaxToken> ILuthCompilerServiceResource.SyntaxTokenList { get; set; } = ImmutableArray<ISyntaxToken>.Empty;

    public ImmutableArray<TextEditorTextSpan> TokenTextSpanList { get; set; }

    public ImmutableArray<TextEditorTextSpan> GetTokenTextSpans()
    {
        return TokenTextSpanList;
    }

    public ImmutableArray<TextEditorDiagnostic> GetDiagnostics()
    {
        return ImmutableArray<TextEditorDiagnostic>.Empty;
    }

    public ImmutableArray<ITextEditorSymbol> GetSymbols()
    {
        return ImmutableArray<ITextEditorSymbol>.Empty;
    }
}