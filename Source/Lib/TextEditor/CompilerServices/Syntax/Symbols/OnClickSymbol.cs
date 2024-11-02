using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Symbols;

/// <summary>
/// TODO: This type should be changed. It is being made to permit simple 'onclick' markers...
///       ...but its not quite a 'Symbol'.
///       |
///       Just happens to be the case that a symbol can act as a marker for some
///       "event like thing" so it works to have it here for now.
///       |
///       The main issue is that the Symbol instances will apply
///       syntax highlighting.
///       |
///       For this 'OnClickSymbol' we would preferably not apply
///       any syntax highlighting.
///       |
///       For example: "cd .." I want to syntax highlight 'cd' as the target file path,
///       but NOT syntax highlight the arguments '..'
///       |
///       If I use a symbol to syntax highlight 'cd', then it might be overridden by the
///       'OnClickSymbol' of which is not supposed to alter the syntax highlighting.
///       (it depends on the order that the symbols are applied).
/// </summary>
public record struct OnClickSymbol : ISymbol
{
	public OnClickSymbol(
		TextEditorTextSpan textSpan,
		string displayText,
		Func<Task> onClickFunc)
    {
        TextSpan = textSpan;
        OnClickFunc = onClickFunc;
        DisplayText = displayText;
    }

    public Func<Task> OnClickFunc { get; init; }
    
    public TextEditorTextSpan TextSpan { get; init; }
    public string DisplayText { get; init; }
    public string SymbolKindString => SyntaxKind.ToString();

    public SyntaxKind SyntaxKind => SyntaxKind.OnClickSymbol;
}
