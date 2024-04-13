using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.CompilerServices.Lang.FSharp.FSharp.Facts;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.SyntaxActors;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.CompilerServices.Lang.FSharp.FSharp.SyntaxActors;

public class TextEditorFSharpLexer : LuthLexer
{
    public static readonly GenericPreprocessorDefinition FSharpPreprocessorDefinition = new(
        "#",
        ImmutableArray<DeliminationExtendedSyntaxDefinition>.Empty);

    public static readonly GenericLanguageDefinition FSharpLanguageDefinition = new GenericLanguageDefinition(
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
            new LuthLexerKeywords(FSharpKeywords.ALL, ImmutableArray<string>.Empty, ImmutableArray<string>.Empty))
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