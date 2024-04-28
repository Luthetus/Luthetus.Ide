using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.CompilerServices.Lang.JavaScript.JavaScript.Facts;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.SyntaxActors;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.CompilerServices.Lang.JavaScript.JavaScript.SyntaxActors;

public class TextEditorJavaScriptLexer : LuthLexer
{
    public static readonly GenericPreprocessorDefinition JavaScriptPreprocessorDefinition = new(
        "#",
        ImmutableArray<DeliminationExtendedSyntaxDefinition>.Empty);

    public static readonly GenericLanguageDefinition JavaScriptLanguageDefinition = new GenericLanguageDefinition(
        "\"",
        "\"",
        "(",
        ")",
        ".",
        "//",
        new[]
        {
            WhitespaceFacts.CARRIAGE_RETURN.ToString(),
            WhitespaceFacts.LINE_FEED.ToString()
        }.ToImmutableArray(),
        "/*",
        "*/",
        JavaScriptKeywords.ALL,
        JavaScriptPreprocessorDefinition);

    private readonly GenericSyntaxTree _javaScriptSyntaxTree;

    public TextEditorJavaScriptLexer(ResourceUri resourceUri, string sourceText)
        : base(
            resourceUri,
            sourceText,
            new LuthLexerKeywords(JavaScriptKeywords.ALL, ImmutableArray<string>.Empty, ImmutableArray<string>.Empty))
    {
        _javaScriptSyntaxTree = new GenericSyntaxTree(JavaScriptLanguageDefinition);
    }

    public Key<RenderState> ModelRenderStateKey { get; private set; } = Key<RenderState>.Empty;

    public override void Lex()
    {
        var javaScriptSyntaxUnit = _javaScriptSyntaxTree.ParseText(
            ResourceUri,
            SourceText);

        var syntaxWalker = new GenericSyntaxWalker();
        syntaxWalker.Visit(javaScriptSyntaxUnit.GenericDocumentSyntax);

        _syntaxTokenList.AddRange(
            syntaxWalker.StringSyntaxList.Select(x => new BadToken(x.TextSpan)));

        _syntaxTokenList.AddRange(
            syntaxWalker.CommentSingleLineSyntaxList.Select(x => new BadToken(x.TextSpan)));

        _syntaxTokenList.AddRange(
            syntaxWalker.CommentMultiLineSyntaxList.Select(x => new BadToken(x.TextSpan)));

        _syntaxTokenList.AddRange(
            syntaxWalker.KeywordSyntaxList.Select(x => new BadToken(x.TextSpan)));

        _syntaxTokenList.AddRange(
            syntaxWalker.FunctionSyntaxList.Select(x => new BadToken(x.TextSpan)));
    }
}