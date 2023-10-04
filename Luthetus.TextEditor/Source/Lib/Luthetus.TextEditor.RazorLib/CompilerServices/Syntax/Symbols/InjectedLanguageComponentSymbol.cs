using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Symbols;

public sealed record InjectedLanguageComponentSymbol : ISymbol
{
    public InjectedLanguageComponentSymbol(TextEditorTextSpan textSpan)
    {
        TextSpan = textSpan;
    }

    public TextEditorTextSpan TextSpan { get; init; }
    public string SymbolKindString => SyntaxKind.ToString();

    public SyntaxKind SyntaxKind => SyntaxKind.InjectedLanguageComponentSymbol;
}