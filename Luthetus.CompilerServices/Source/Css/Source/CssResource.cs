using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.Css;

public class CssResource : ILuthCompilerServiceResource
{
    public CssResource(
        ResourceUri resourceUri,
        CssCompilerService textEditorCssCompilerService)
    {
        ResourceUri = resourceUri;
        TextEditorCssCompilerService = textEditorCssCompilerService;
    }

    public ResourceUri ResourceUri { get; }
    public CssCompilerService TextEditorCssCompilerService { get; }
    public ImmutableArray<TextEditorTextSpan>? SyntacticTextSpans { get; internal set; }

    CompilationUnit? ILuthCompilerServiceResource.CompilationUnit { get; set; }

    ILuthCompilerService ILuthCompilerServiceResource.CompilerService => TextEditorCssCompilerService;
    ImmutableArray<ISyntaxToken> ILuthCompilerServiceResource.SyntaxTokenList { get; set; } = ImmutableArray<ISyntaxToken>.Empty;

    public ImmutableArray<TextEditorTextSpan> TokenTextSpanList { get; set; }

    public ImmutableArray<TextEditorTextSpan> GetTokenTextSpans()
    {
        return TokenTextSpanList;
    }

    public ImmutableArray<ITextEditorSymbol> GetSymbols()
    {
        return ImmutableArray<ITextEditorSymbol>.Empty;
    }

    public ImmutableArray<TextEditorDiagnostic> GetDiagnostics()
    {
        return ImmutableArray<TextEditorDiagnostic>.Empty;
    }
}