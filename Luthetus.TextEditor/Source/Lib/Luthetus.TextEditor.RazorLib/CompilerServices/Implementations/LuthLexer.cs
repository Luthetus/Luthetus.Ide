using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;

public class LuthLexer : ILuthLexer
{
    protected readonly StringWalker _stringWalker;
    protected readonly List<ISyntaxToken> _syntaxTokenList = new();
    protected readonly LuthDiagnosticBag _diagnosticBag = new();

    public LuthLexer(
        ResourceUri resourceUri,
        string sourceText,
        LuthLexerKeywords lexerKeywords)
    {
        ResourceUri = resourceUri;
        SourceText = sourceText;
        LexerKeywords = lexerKeywords;

        _stringWalker = new(resourceUri, sourceText);
    }

    public ResourceUri ResourceUri { get; }
    public string SourceText { get; }
    public LuthLexerKeywords LexerKeywords { get; }

    public ImmutableArray<ISyntaxToken> SyntaxTokenList => _syntaxTokenList.ToImmutableArray();
    public ImmutableArray<TextEditorDiagnostic> DiagnosticList => _diagnosticBag.ToImmutableArray();

    public virtual void Lex()
    {
    }
}
