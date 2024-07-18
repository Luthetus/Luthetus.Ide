using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

public interface ILexer
{
    public ImmutableArray<TextEditorDiagnostic> DiagnosticList { get; }
    public ImmutableArray<ISyntaxToken> SyntaxTokenList { get; }
    public ResourceUri ResourceUri { get; }
    public string SourceText { get; }
    public LexerKeywords LexerKeywords { get; }

    public void Lex();
}
