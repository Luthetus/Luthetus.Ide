using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.Extensions.CompilerServices;
using Luthetus.Extensions.CompilerServices.GenericLexer;
using Luthetus.Extensions.CompilerServices.GenericLexer.SyntaxActors;
using Luthetus.Extensions.CompilerServices.Syntax;
using Luthetus.CompilerServices.TypeScript.Facts;

namespace Luthetus.CompilerServices.TypeScript.SyntaxActors;

public class TextEditorTypeScriptLexer
{
    public static readonly GenericPreprocessorDefinition TypeScriptPreprocessorDefinition = new(
        "#",
        Array.Empty<DeliminationExtendedSyntaxDefinition>());

    public static readonly GenericLanguageDefinition TypeScriptLanguageDefinition = new GenericLanguageDefinition(
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
        TypeScriptKeywords.ALL,
        TypeScriptPreprocessorDefinition);

    private readonly GenericSyntaxTree _typeScriptSyntaxTree;
    
    public List<SyntaxToken> SyntaxTokenList { get; } = new();
    
    public static readonly LexerKeywords LexerKeywords = new LexerKeywords(TypeScriptKeywords.ALL, Array.Empty<string>(), Array.Empty<string>());
    
    public TextEditorTypeScriptLexer(ResourceUri resourceUri, string sourceText)
    {
    	ResourceUri = resourceUri;
    	SourceText = sourceText;
        _typeScriptSyntaxTree = new GenericSyntaxTree(TypeScriptLanguageDefinition);
    }

	public ResourceUri ResourceUri { get; set; }
	public string SourceText { get; set; }

    public Key<RenderState> ModelRenderStateKey { get; private set; } = Key<RenderState>.Empty;

    public void Lex()
    {
        var typeScriptSyntaxUnit = _typeScriptSyntaxTree.ParseText(
            ResourceUri,
            SourceText);

        var syntaxWalker = new GenericSyntaxWalker();
        syntaxWalker.Visit(typeScriptSyntaxUnit.GenericDocumentSyntax);

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