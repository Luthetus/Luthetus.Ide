using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.Extensions.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax;
using Luthetus.Extensions.CompilerServices.GenericLexer;
using Luthetus.Extensions.CompilerServices.GenericLexer.SyntaxActors;
using Luthetus.CompilerServices.Python.Facts;

namespace Luthetus.CompilerServices.Python;

public class PythonLexer
{
	private static readonly LexerKeywords _lexerKeywords = new LexerKeywords(PythonLanguageFacts.Keywords.ALL_LIST, PythonLanguageFacts.Keywords.CONTROL_KEYWORDS, Array.Empty<string>());

    public PythonLexer(ResourceUri resourceUri, string sourceText)
    {
    	ResourceUri = resourceUri;
    	SourceText = sourceText;
        _pythonSyntaxTree = new GenericSyntaxTree(PythonLanguageDefinition);
    }
    
    public ResourceUri ResourceUri { get; set; }
    public string SourceText { get; set; }
    
    public List<SyntaxToken> SyntaxTokenList { get; } = new();

    public static readonly GenericPreprocessorDefinition PythonPreprocessorDefinition = new(
        "\0",
		Array.Empty<DeliminationExtendedSyntaxDefinition>());
		
    public static readonly GenericLanguageDefinition PythonLanguageDefinition = new GenericLanguageDefinition(
        "\"",
        "\"",
        "(",
        ")",
        ".",
        "#",
        new()
        {
            WhitespaceFacts.CARRIAGE_RETURN.ToString(),
            WhitespaceFacts.LINE_FEED.ToString()
        },
        "/*",
        "*/",
        PythonLanguageFacts.Keywords.ALL_LIST,
        PythonPreprocessorDefinition);

    private readonly GenericSyntaxTree _pythonSyntaxTree;

    public Key<RenderState> ModelRenderStateKey { get; private set; } = Key<RenderState>.Empty;

    public void Lex()
    {
        var pythonSyntaxUnit = _pythonSyntaxTree.ParseText(
            ResourceUri,
            SourceText);

        var pythonSyntaxWalker = new GenericSyntaxWalker();
        pythonSyntaxWalker.Visit(pythonSyntaxUnit.GenericDocumentSyntax);

        SyntaxTokenList.AddRange(
            pythonSyntaxWalker.StringSyntaxList.Select(x => new SyntaxToken(SyntaxKind.BadToken, x.TextSpan)));

        SyntaxTokenList.AddRange(
            pythonSyntaxWalker.CommentSingleLineSyntaxList.Select(x => new SyntaxToken(SyntaxKind.BadToken, x.TextSpan)));

        SyntaxTokenList.AddRange(
            pythonSyntaxWalker.CommentMultiLineSyntaxList.Select(x => new SyntaxToken(SyntaxKind.BadToken, x.TextSpan)));

        SyntaxTokenList.AddRange(
            pythonSyntaxWalker.KeywordSyntaxList.Select(x => new SyntaxToken(SyntaxKind.BadToken, x.TextSpan)));

        SyntaxTokenList.AddRange(
            pythonSyntaxWalker.FunctionSyntaxList.Select(x => new SyntaxToken(SyntaxKind.BadToken, x.TextSpan)));
    }
}