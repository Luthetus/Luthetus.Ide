using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.Extensions.CompilerServices;
using Luthetus.Extensions.CompilerServices.GenericLexer;
using Luthetus.Extensions.CompilerServices.GenericLexer.SyntaxActors;
using Luthetus.Extensions.CompilerServices.Syntax;
using Luthetus.CompilerServices.FSharp.Facts;

namespace Luthetus.CompilerServices.FSharp.SyntaxActors;

public class TextEditorFSharpLexer
{
    public static readonly GenericPreprocessorDefinition FSharpPreprocessorDefinition = new(
        "#",
        Array.Empty<DeliminationExtendedSyntaxDefinition>());

    public static readonly GenericLanguageDefinition FSharpLanguageDefinition = new GenericLanguageDefinition(
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
        "(*",
        "*)",
        FSharpKeywords.ALL,
        FSharpPreprocessorDefinition);

    private readonly GenericSyntaxTree _fSharpSyntaxTree;
    
    private static readonly LexerKeywords _lexerKeywords = new LexerKeywords(FSharpKeywords.ALL, Array.Empty<string>(), Array.Empty<string>());
    
    public List<SyntaxToken> SyntaxTokenList { get; } = new();

    public TextEditorFSharpLexer(ResourceUri resourceUri, string sourceText)
    {
    	ResourceUri = resourceUri;
    	SourceText = sourceText;
        _fSharpSyntaxTree = new GenericSyntaxTree(FSharpLanguageDefinition);
    }

	public ResourceUri ResourceUri { get; set; }
	public string SourceText { get; set; }

    public Key<RenderState> ModelRenderStateKey { get; private set; } = Key<RenderState>.Empty;

    public void Lex()
    {
        var fSharpSyntaxUnit = _fSharpSyntaxTree.ParseText(
            ResourceUri,
            SourceText);

        var syntaxWalker = new GenericSyntaxWalker();
        syntaxWalker.Visit(fSharpSyntaxUnit.GenericDocumentSyntax);

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