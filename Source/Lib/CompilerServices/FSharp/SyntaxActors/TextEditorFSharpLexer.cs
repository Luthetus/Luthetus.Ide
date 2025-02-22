using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.SyntaxActors;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.CompilerServices.FSharp.Facts;

namespace Luthetus.CompilerServices.FSharp.SyntaxActors;

public class TextEditorFSharpLexer : Lexer
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

    public TextEditorFSharpLexer(
            ResourceUri resourceUri, string sourceText)
        : base(
            resourceUri,
            sourceText,
            new LexerKeywords(FSharpKeywords.ALL, Array.Empty<string>(), Array.Empty<string>()))
    {
        _fSharpSyntaxTree = new GenericSyntaxTree(FSharpLanguageDefinition);
    }

    public Key<RenderState> ModelRenderStateKey { get; private set; } = Key<RenderState>.Empty;

    public override void Lex()
    {
        var fSharpSyntaxUnit = _fSharpSyntaxTree.ParseText(
            ResourceUri,
            SourceText);

        var syntaxWalker = new GenericSyntaxWalker();
        syntaxWalker.Visit(fSharpSyntaxUnit.GenericDocumentSyntax);

        _syntaxTokenList.AddRange(
            syntaxWalker.StringSyntaxList.Select(x => new SyntaxToken(SyntaxKind.BadToken, x.TextSpan)));

        _syntaxTokenList.AddRange(
            syntaxWalker.CommentSingleLineSyntaxList.Select(x => new SyntaxToken(SyntaxKind.BadToken, x.TextSpan)));

        _syntaxTokenList.AddRange(
            syntaxWalker.CommentMultiLineSyntaxList.Select(x => new SyntaxToken(SyntaxKind.BadToken, x.TextSpan)));

        _syntaxTokenList.AddRange(
            syntaxWalker.KeywordSyntaxList.Select(x => new SyntaxToken(SyntaxKind.BadToken, x.TextSpan)));

        _syntaxTokenList.AddRange(
            syntaxWalker.FunctionSyntaxList.Select(x => new SyntaxToken(SyntaxKind.BadToken, x.TextSpan)));
    }
}