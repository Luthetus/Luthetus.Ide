using Luthetus.CompilerServices.Lang.Xml.Html.Decoration;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Symbols;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Nodes;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices;

namespace Luthetus.CompilerServices.Lang.Razor.CompilerServiceCase;

public class RazorResource : ILuthCompilerServiceResource
{
    private readonly ITextEditorService _textEditorService;

    public RazorResource(
        ResourceUri resourceUri,
        RazorCompilerService razorCompilerService,
        ITextEditorService textEditorService)
    {
        ResourceUri = resourceUri;
        RazorCompilerService = razorCompilerService;
        _textEditorService = textEditorService;
    }

    public ResourceUri ResourceUri { get; }
    public RazorCompilerService RazorCompilerService { get; }

    public RazorSyntaxTree? RazorSyntaxTree { get; internal set; }
    
    CompilationUnit? ILuthCompilerServiceResource.CompilationUnit { get; set; }
    ILuthCompilerService ILuthCompilerServiceResource.CompilerService => RazorCompilerService;
    ImmutableArray<ISyntaxToken> ILuthCompilerServiceResource.SyntaxTokenList { get; set; } = ImmutableArray<ISyntaxToken>.Empty;
    public ImmutableArray<TextEditorTextSpan> TokenTextSpanList { get; internal set; }

    public List<ITextEditorSymbol> HtmlSymbols = new();

    /// <summary>
    /// TODO: It might be a useful optimization to evaluate these linq expressions
    /// and store the result as to not re-evaluate the linq expression over and over.
    /// </summary>
    public ImmutableArray<ITextEditorSymbol> GetSymbols()
    {
        var localRazorSyntaxTree = RazorSyntaxTree;

        if (localRazorSyntaxTree?.SemanticResultRazor is null)
            return ImmutableArray<ITextEditorSymbol>.Empty;

        var symbols = localRazorSyntaxTree
            .SemanticResultRazor
            .CompilationUnit
            .Binder
            .SymbolsList;

        var originalText = _textEditorService.ModelApi.GetAllText(ResourceUri);

        if (originalText is null)
            return ImmutableArray<ITextEditorSymbol>.Empty;

        List<ITextEditorSymbol> mappedSymbols = new();

        foreach (var symbol in symbols)
        {
            var mappedTextSpan = localRazorSyntaxTree.SemanticResultRazor
                .MapAdhocCSharpTextSpanToSource(
                    ResourceUri,
                    originalText,
                    symbol.TextSpan);

            if (mappedTextSpan is null)
                continue;

            ITextEditorSymbol mappedSymbol;

            switch (symbol.SymbolKindString)
            {
                case "TypeSymbol":
                    mappedTextSpan = mappedTextSpan with
                    {
                        DecorationByte = (byte)HtmlDecorationKind.InjectedLanguageType
                    };

                    mappedSymbol = new TypeSymbol(mappedTextSpan);
                    break;
                case "ConstructorSymbol":
                    mappedTextSpan = mappedTextSpan with
                    {
                        DecorationByte = (byte)HtmlDecorationKind.InjectedLanguageType
                    };

                    mappedSymbol = new ConstructorSymbol(mappedTextSpan);
                    break;
                case "FunctionSymbol":
                    mappedTextSpan = mappedTextSpan with
                    {
                        DecorationByte = (byte)HtmlDecorationKind.InjectedLanguageMethod
                    };

                    mappedSymbol = new FunctionSymbol(mappedTextSpan);
                    break;
                case "VariableSymbol":
                    mappedTextSpan = mappedTextSpan with
                    {
                        DecorationByte = (byte)HtmlDecorationKind.InjectedLanguageVariable
                    };

                    mappedSymbol = new VariableSymbol(mappedTextSpan);
                    break;
                case "PropertySymbol":
                    mappedTextSpan = mappedTextSpan with
                    {
                        DecorationByte = (byte)HtmlDecorationKind.None
                    };

                    mappedSymbol = new PropertySymbol(mappedTextSpan);
                    break;
                case "StringInterpolationSymbol":
                    mappedTextSpan = mappedTextSpan with
                    {
                        DecorationByte = (byte)HtmlDecorationKind.InjectedLanguageStringLiteral
                    };

                    mappedSymbol = new StringInterpolationSymbol(mappedTextSpan);
                    break;
                case "NamespaceSymbol":
                    mappedTextSpan = mappedTextSpan with
                    {
                        DecorationByte = (byte)HtmlDecorationKind.None
                    };

                    mappedSymbol = new NamespaceSymbol(mappedTextSpan);
                    break;
                default:
                    mappedTextSpan = mappedTextSpan with
                    {
                        DecorationByte = (byte)HtmlDecorationKind.None
                    };

                    mappedSymbol = symbol;
                    break;
            }

            mappedSymbols.Add(mappedSymbol);
        }
        
        foreach (var symbol in HtmlSymbols)
        {
            switch (symbol.SymbolKindString)
            {
                case "InjectedLanguageComponentSymbol":
                    var injectedLanguageComponentSymbol = (InjectedLanguageComponentSymbol)symbol;
                    
                    mappedSymbols.Add(injectedLanguageComponentSymbol with
                    {
                        TextSpan = injectedLanguageComponentSymbol.TextSpan with
                        {
                            DecorationByte = (byte)HtmlDecorationKind.InjectedLanguageComponent
                        }
                    });
                    break;
            }
        }

        return mappedSymbols.ToImmutableArray();
    }

    public ImmutableArray<TextEditorTextSpan> GetTokenTextSpans()
    {
        return TokenTextSpanList;
    }

    public ImmutableArray<TextEditorDiagnostic> GetDiagnostics()
    {
        return ImmutableArray<TextEditorDiagnostic>.Empty;
    }
}