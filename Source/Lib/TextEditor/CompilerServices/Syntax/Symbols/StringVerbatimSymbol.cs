using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Symbols;

public sealed record StringVerbatimSymbol : ISymbol
{
    public StringVerbatimSymbol(TextEditorTextSpan textSpan)
    {
        TextSpan = textSpan;
    }

    public TextEditorTextSpan TextSpan { get; }
    public string SymbolKindString => SyntaxKind.ToString();

    public SyntaxKind SyntaxKind => SyntaxKind.StringVerbatimSymbol;
}