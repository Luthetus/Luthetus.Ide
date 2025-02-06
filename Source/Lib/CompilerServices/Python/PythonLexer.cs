using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.RenderStates.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.SyntaxActors;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.CompilerServices.Python.Facts;

namespace Luthetus.CompilerServices.Python;

public class PythonLexer : Lexer
{
    public PythonLexer(ResourceUri resourceUri, string sourceText)
        : base(
            resourceUri,
            sourceText,
            new LexerKeywords(PythonLanguageFacts.Keywords.ALL_LIST, PythonLanguageFacts.Keywords.CONTROL_KEYWORDS, Array.Empty<string>()))
    {
        _pythonSyntaxTree = new GenericSyntaxTree(PythonLanguageDefinition);
    }

    public static readonly GenericPreprocessorDefinition PythonPreprocessorDefinition = new(
        "\0",
        ImmutableArray<DeliminationExtendedSyntaxDefinition>.Empty);

    public static readonly GenericLanguageDefinition PythonLanguageDefinition = new GenericLanguageDefinition(
        "\"",
        "\"",
        "(",
        ")",
        ".",
        "#",
        new[]
        {
            WhitespaceFacts.CARRIAGE_RETURN.ToString(),
            WhitespaceFacts.LINE_FEED.ToString()
        }.ToImmutableArray(),
        "/*",
        "*/",
        PythonLanguageFacts.Keywords.ALL_LIST,
        PythonPreprocessorDefinition);

    private readonly GenericSyntaxTree _pythonSyntaxTree;

    public Key<RenderState> ModelRenderStateKey { get; private set; } = Key<RenderState>.Empty;

    public override void Lex()
    {
        var pythonSyntaxUnit = _pythonSyntaxTree.ParseText(
            ResourceUri,
            SourceText);

        var pythonSyntaxWalker = new GenericSyntaxWalker();
        pythonSyntaxWalker.Visit(pythonSyntaxUnit.GenericDocumentSyntax);

        _syntaxTokenList.AddRange(
            pythonSyntaxWalker.StringSyntaxList.Select(x => (ISyntaxToken)new BadToken(x.TextSpan)));

        _syntaxTokenList.AddRange(
            pythonSyntaxWalker.CommentSingleLineSyntaxList.Select(x => (ISyntaxToken)new BadToken(x.TextSpan)));

        _syntaxTokenList.AddRange(
            pythonSyntaxWalker.CommentMultiLineSyntaxList.Select(x => (ISyntaxToken)new BadToken(x.TextSpan)));

        _syntaxTokenList.AddRange(
            pythonSyntaxWalker.KeywordSyntaxList.Select(x => (ISyntaxToken)new BadToken(x.TextSpan)));

        _syntaxTokenList.AddRange(
            pythonSyntaxWalker.FunctionSyntaxList.Select(x => (ISyntaxToken)new BadToken(x.TextSpan)));
    }
}