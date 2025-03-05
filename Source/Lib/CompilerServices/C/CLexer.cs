using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.SyntaxActors;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.CompilerServices.C.Facts;

namespace Luthetus.CompilerServices.C;

public class CLexer
{
	private static readonly LexerKeywords _lexerKeywords = new LexerKeywords(CLanguageFacts.Keywords.ALL_LIST, CLanguageFacts.Keywords.CONTROL_KEYWORDS, Array.Empty<string>());

	public List<SyntaxToken> SyntaxTokenList { get; } = new();

    public CLexer(ResourceUri resourceUri, string sourceText)
    {
    	ResourceUri = resourceUri;
    	SourceText = sourceText;
    
        _cSyntaxTree = new GenericSyntaxTree(CLanguageDefinition);
    }
    
    public ResourceUri ResourceUri { get; }
    public string SourceText { get; }

    public static readonly GenericPreprocessorDefinition CPreprocessorDefinition = new(
        "#",
        Array.Empty<DeliminationExtendedSyntaxDefinition>());

    public static readonly GenericLanguageDefinition CLanguageDefinition = new GenericLanguageDefinition(
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
        CLanguageFacts.Keywords.ALL_LIST,
        CPreprocessorDefinition);

    private readonly GenericSyntaxTree _cSyntaxTree;

    public Key<RenderState> ModelRenderStateKey { get; private set; } = Key<RenderState>.Empty;

    public void Lex()
    {
        var cSyntaxUnit = _cSyntaxTree.ParseText(
            ResourceUri,
            SourceText);

        var cSyntaxWalker = new GenericSyntaxWalker();
        cSyntaxWalker.Visit(cSyntaxUnit.GenericDocumentSyntax);

        SyntaxTokenList.AddRange(
            cSyntaxWalker.StringSyntaxList.Select(x => new SyntaxToken(SyntaxKind.BadToken, x.TextSpan)));

        SyntaxTokenList.AddRange(
            cSyntaxWalker.CommentSingleLineSyntaxList.Select(x => new SyntaxToken(SyntaxKind.BadToken, x.TextSpan)));

        SyntaxTokenList.AddRange(
            cSyntaxWalker.CommentMultiLineSyntaxList.Select(x => new SyntaxToken(SyntaxKind.BadToken, x.TextSpan)));

        SyntaxTokenList.AddRange(
            cSyntaxWalker.KeywordSyntaxList.Select(x => new SyntaxToken(SyntaxKind.BadToken, x.TextSpan)));

        SyntaxTokenList.AddRange(
            cSyntaxWalker.FunctionSyntaxList.Select(x => new SyntaxToken(SyntaxKind.BadToken, x.TextSpan)));
    }
}