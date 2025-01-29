using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

/// <summary>
/// This type is similar to 'StringLiteralToken'.
/// The difference is that this token implies to the consumer
/// that they need to consume the next tokens until 'SyntaxKind.StringInterpolatedEnd'
/// for any interpolated expressions that need to be parsed.
///
/// Preferably, this type wouldn't "implies to the consumer",
/// but instead automatically handle this once consumed,
/// but for now it likely needs to be an implication
/// because there is no "OnConsumed" kind of logic.
///
/// This token stores:
///     $"Hello, {name}"
/// 
/// In the above example the DollarSignToken is included, up to (and including) the closing double quote.
///
/// The individual interpolated expressions will be added to the syntax token list at the next index after the
/// 'StringInterpolatedToken'.
///     {name}
/// will add the IdentifierToken of 'name'.
/// 
/// The "list" ends once 'SyntaxKind.StringInterpolatedEnd' is read on a token.
/// </summary>
public struct StringInterpolatedStartToken : ISyntaxToken
{
    public StringInterpolatedStartToken(TextEditorTextSpan textSpan)
    {
    	ConstructorWasInvoked = true;
        TextSpan = textSpan;
    }

    public TextEditorTextSpan TextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.StringInterpolatedStartToken;
    public bool IsFabricated { get; init; }
    public bool ConstructorWasInvoked { get; }
}
