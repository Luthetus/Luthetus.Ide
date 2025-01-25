using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Symbols;

public record struct EnumMemberSymbol : ISymbol
{
    public EnumMemberSymbol(int symbolId, TextEditorTextSpan textSpan)
    {
    	SymbolId = symbolId;
        TextSpan = textSpan;
    }

    public int SymbolId { get; }
    public TextEditorTextSpan TextSpan { get; }
    public string SymbolKindString => SyntaxKind.ToString();

    public SyntaxKind SyntaxKind => SyntaxKind.EnumMemberSymbol;
}
