using Luthetus.TextEditor.RazorLib.Lexing;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax.SyntaxTokens;

public sealed record EqualsEqualsToken : ISyntaxToken
{
    public EqualsEqualsToken(
        TextEditorTextSpan textEditorTextSpan)
    {
        TextSpan = textEditorTextSpan;
    }

    public TextEditorTextSpan TextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.EqualsEqualsToken;
}
