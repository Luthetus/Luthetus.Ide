using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.SyntaxActors;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.CompilerServices.JavaScript.Facts;

namespace Luthetus.CompilerServices.JavaScript.SyntaxActors;

public class TextEditorJavaScriptLexer
{
    public static readonly GenericPreprocessorDefinition JavaScriptPreprocessorDefinition = new(
        "#",
        Array.Empty<DeliminationExtendedSyntaxDefinition>());

    public static readonly GenericLanguageDefinition JavaScriptLanguageDefinition = new GenericLanguageDefinition(
        "\"",
        "\"",
        "(",
        ")",
        ".",
        "//",
        new()
        {
            WhitespaceFacts.CARRIAGE_RETURN.ToString(),
            WhitespaceFacts.LINE_FEED.ToString()
        },
        "/*",
        "*/",
        JavaScriptKeywords.ALL,
        JavaScriptPreprocessorDefinition);

    private readonly GenericSyntaxTree _javaScriptSyntaxTree;

    public TextEditorJavaScriptLexer(ResourceUri resourceUri, string sourceText)
    {
    	ResourceUri = resourceUri;
        SourceText = sourceText;
        _javaScriptSyntaxTree = new GenericSyntaxTree(JavaScriptLanguageDefinition);
    }
    
    private static readonly LexerKeywords LexerKeywords = new LexerKeywords(JavaScriptKeywords.ALL, Array.Empty<string>(), Array.Empty<string>());
    
    public List<SyntaxToken> SyntaxTokenList { get; } = new();
    
    public ResourceUri ResourceUri { get; set; }
    public string SourceText { get; set; }

    public Key<RenderState> ModelRenderStateKey { get; private set; } = Key<RenderState>.Empty;

    public void Lex()
    {
        var javaScriptSyntaxUnit = _javaScriptSyntaxTree.ParseText(
            ResourceUri,
            SourceText);

        var syntaxWalker = new GenericSyntaxWalker();
        syntaxWalker.Visit(javaScriptSyntaxUnit.GenericDocumentSyntax);

        SyntaxTokenList.AddRange(
            syntaxWalker.StringSyntaxList.Select(x => new SyntaxToken(SyntaxKind.BadToken, x.TextSpan)));

        SyntaxTokenList.AddRange(
            syntaxWalker.CommentSingleLineSyntaxList.Select(x => new SyntaxToken(SyntaxKind.BadToken, x.TextSpan)));

        SyntaxTokenList.AddRange(
            syntaxWalker.CommentMultiLineSyntaxList.Select(x => new SyntaxToken(SyntaxKind.BadToken, x.TextSpan)));

        SyntaxTokenList.AddRange(
            syntaxWalker.KeywordSyntaxList.Select(x => new SyntaxToken(SyntaxKind.BadToken, x.TextSpan)));

        SyntaxTokenList.AddRange(
            syntaxWalker.FunctionSyntaxList.Select(x => new SyntaxToken(SyntaxKind.BadToken, x.TextSpan)));
    }
}