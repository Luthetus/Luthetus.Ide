using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax;
using Luthetus.CompilerServices.Xml.Html.Decoration;

namespace Luthetus.CompilerServices.Razor.CompilerServiceCase;

public class RazorResource : ICompilerServiceResource
{
    private readonly TextEditorService _textEditorService;

    public RazorResource(
        ResourceUri resourceUri,
        RazorCompilerService razorCompilerService,
        TextEditorService textEditorService)
    {
    	ResourceUri = resourceUri;
    	CompilerService = razorCompilerService;
        _textEditorService = textEditorService;
    }
    
    public ResourceUri ResourceUri { get; }
	public ICompilerService CompilerService { get; }
	public ICompilationUnit CompilationUnit { get; set; }

    public RazorSyntaxTree? RazorSyntaxTree { get; internal set; }
    public List<Symbol> HtmlSymbols { get; } = new();
    
    public IReadOnlyList<Symbol> GetSymbols()
    {
        var localRazorSyntaxTree = RazorSyntaxTree;

        if (localRazorSyntaxTree?.SemanticResultRazor is null)
            return Array.Empty<Symbol>();

        var symbols = localRazorSyntaxTree.SemanticResultRazor.CompilationUnit.SymbolList;

        var originalText = _textEditorService.ModelApi.GetAllText(ResourceUri);

        if (originalText is null)
            return Array.Empty<Symbol>();

        List<Symbol> mappedSymbols = new();

        foreach (var symbol in symbols)
        {
            var mappedTextSpan = localRazorSyntaxTree.SemanticResultRazor
                .MapAdhocCSharpTextSpanToSource(
                    ResourceUri,
                    originalText,
                    symbol.TextSpan);

            if (mappedTextSpan is null)
                continue;

            Symbol mappedSymbol;

            switch (symbol.SymbolKindString)
            {
                case "TypeSymbol":
                    mappedTextSpan = mappedTextSpan.Value with
                    {
                        DecorationByte = (byte)HtmlDecorationKind.InjectedLanguageType
                    };

                    mappedSymbol = new Symbol(SyntaxKind.TypeSymbol, symbolId: 0, mappedTextSpan.Value);
                    break;
                case "ConstructorSymbol":
                    mappedTextSpan = mappedTextSpan.Value with
                    {
                        DecorationByte = (byte)HtmlDecorationKind.InjectedLanguageType
                    };

                    mappedSymbol = new Symbol(SyntaxKind.ConstructorSymbol, symbolId: 0, mappedTextSpan.Value);
                    break;
                case "FunctionSymbol":
                    mappedTextSpan = mappedTextSpan.Value with
                    {
                        DecorationByte = (byte)HtmlDecorationKind.InjectedLanguageMethod
                    };

                    mappedSymbol = new Symbol(SyntaxKind.FunctionSymbol, symbolId: 0, mappedTextSpan.Value);
                    break;
                case "VariableSymbol":
                    mappedTextSpan = mappedTextSpan.Value with
                    {
                        DecorationByte = (byte)HtmlDecorationKind.InjectedLanguageVariable
                    };

                    mappedSymbol = new Symbol(SyntaxKind.VariableSymbol, symbolId: 0, mappedTextSpan.Value);
                    break;
                case "PropertySymbol":
                    mappedTextSpan = mappedTextSpan.Value with
                    {
                        DecorationByte = (byte)HtmlDecorationKind.None
                    };

                    mappedSymbol = new Symbol(SyntaxKind.PropertySymbol, symbolId: 0, mappedTextSpan.Value);
                    break;
                case "StringInterpolationSymbol":
                    mappedTextSpan = mappedTextSpan.Value with
                    {
                        DecorationByte = (byte)HtmlDecorationKind.InjectedLanguageStringLiteral
                    };

                    mappedSymbol = new Symbol(SyntaxKind.StringInterpolationSymbol, symbolId: 0, mappedTextSpan.Value);
                    break;
                case "NamespaceSymbol":
                    mappedTextSpan = mappedTextSpan.Value with
                    {
                        DecorationByte = (byte)HtmlDecorationKind.None
                    };

                    mappedSymbol = new Symbol(SyntaxKind.NamespaceSymbol, symbolId: 0, mappedTextSpan.Value);
                    break;
                default:
                    mappedTextSpan = mappedTextSpan.Value with
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
                    var injectedLanguageComponentSymbol = symbol;
                    
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

        return mappedSymbols;
    }
}