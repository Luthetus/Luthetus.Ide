using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.Xml;

public class XmlResource : ILuthCompilerServiceResource
{
    public XmlResource(
        ResourceUri resourceUri,
        XmlCompilerService xmlCompilerService)
    {
        ResourceUri = resourceUri;
        XmlCompilerService = xmlCompilerService;
    }

    public ResourceUri ResourceUri { get; }
    public XmlCompilerService XmlCompilerService { get; }
    public ImmutableArray<TextEditorTextSpan>? SyntacticTextSpans { get; internal set; }
    public ImmutableArray<TextEditorTextSpan> TokenTextSpanList { get; internal set; }
    public CompilationUnit? CompilationUnit { get; set; }

    public ILuthCompilerService CompilerService => XmlCompilerService;

    public ImmutableArray<ISyntaxToken> SyntaxTokenList { get; set; } = ImmutableArray<ISyntaxToken>.Empty;

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