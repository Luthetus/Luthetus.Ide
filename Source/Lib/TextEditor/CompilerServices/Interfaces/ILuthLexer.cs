using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

public interface ILuthLexer
{
    public ImmutableArray<TextEditorDiagnostic> DiagnosticList { get; }
    public ImmutableArray<ISyntaxToken> SyntaxTokenList { get; }
    public ResourceUri ResourceUri { get; }
    public string SourceText { get; }
    public LuthLexerKeywords LexerKeywords { get; }

    public void Lex();
}
