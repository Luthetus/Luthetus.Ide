using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.SyntaxActors;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.CompilerServices.C.Facts;

namespace Luthetus.CompilerServices.C;

public class CLexer : Lexer
{
    public CLexer(ResourceUri resourceUri, string sourceText)
        : base(
            resourceUri,
            sourceText,
            new LexerKeywords(CLanguageFacts.Keywords.ALL_LIST, CLanguageFacts.Keywords.CONTROL_KEYWORDS, ImmutableArray<string>.Empty))
    {
        _cSyntaxTree = new GenericSyntaxTree(CLanguageDefinition);
    }

    public static readonly GenericPreprocessorDefinition CPreprocessorDefinition = new(
        "#",
        ImmutableArray<DeliminationExtendedSyntaxDefinition>.Empty);

    public static readonly GenericLanguageDefinition CLanguageDefinition = new GenericLanguageDefinition(
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
        CLanguageFacts.Keywords.ALL_LIST,
        CPreprocessorDefinition);

    private readonly GenericSyntaxTree _cSyntaxTree;

    public Key<RenderState> ModelRenderStateKey { get; private set; } = Key<RenderState>.Empty;

    public override void Lex()
    {
        var cSyntaxUnit = _cSyntaxTree.ParseText(
            ResourceUri,
            SourceText);

        var cSyntaxWalker = new GenericSyntaxWalker();
        cSyntaxWalker.Visit(cSyntaxUnit.GenericDocumentSyntax);

        _syntaxTokenList.AddRange(
            cSyntaxWalker.StringSyntaxList.Select(x => (ISyntaxToken)new BadToken(x.TextSpan)));

        _syntaxTokenList.AddRange(
            cSyntaxWalker.CommentSingleLineSyntaxList.Select(x => (ISyntaxToken)new BadToken(x.TextSpan)));

        _syntaxTokenList.AddRange(
            cSyntaxWalker.CommentMultiLineSyntaxList.Select(x => (ISyntaxToken)new BadToken(x.TextSpan)));

        _syntaxTokenList.AddRange(
            cSyntaxWalker.KeywordSyntaxList.Select(x => (ISyntaxToken)new BadToken(x.TextSpan)));

        _syntaxTokenList.AddRange(
            cSyntaxWalker.FunctionSyntaxList.Select(x => (ISyntaxToken)new BadToken(x.TextSpan)));
    }
}