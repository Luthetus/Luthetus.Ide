using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;

public class Lexer : ILexer
{
    protected readonly StringWalker _stringWalker;
    protected readonly List<ISyntaxToken> _syntaxTokenList = new();
    protected readonly DiagnosticBag _diagnosticBag = new();

    public Lexer(
        ResourceUri resourceUri,
        string sourceText,
        LexerKeywords lexerKeywords)
    {
        ResourceUri = resourceUri;
        SourceText = sourceText;
        LexerKeywords = lexerKeywords;

        _stringWalker = new(resourceUri, sourceText);
    }

    public ResourceUri ResourceUri { get; }
    public string SourceText { get; }
    public LexerKeywords LexerKeywords { get; }

    public ImmutableArray<ISyntaxToken> SyntaxTokenList => _syntaxTokenList.ToImmutableArray();
    public ImmutableArray<TextEditorDiagnostic> DiagnosticList => _diagnosticBag.ToImmutableArray();

    public virtual void Lex()
    {
    }
}
