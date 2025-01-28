using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;

/// <summary>
/// This type is similar to 'StringLiteralToken'.
/// The difference is that this token implies to the consumer
/// that they need to check the 'CSharpLexerOutput.TriviaTextSpanList'
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
/// The 'CSharpLexerOutput.TriviaTextSpanList' will contain a text span:
///     {name}
/// </summary>
public struct StringInterpolatedToken : ISyntaxToken
{
	/// <summary>
	/// 'delimiterLength' is for raw strings that have any amount of '{' or '}' that can be used to deliminate interpolated expressions.
	/// </summary>
    public StringInterpolatedToken(TextEditorTextSpan textSpan, int delimiterLength)
    {
    	ConstructorWasInvoked = true;
        TextSpan = textSpan;
        
        DelimiterLength = delimiterLength;
    }

    public TextEditorTextSpan TextSpan { get; }
    public SyntaxKind SyntaxKind => SyntaxKind.StringInterpolatedToken;
    public bool IsFabricated { get; init; }
    public bool ConstructorWasInvoked { get; }
    /// <summary>
	/// 'delimiterLength' is for raw strings that have any amount of '{' or '}' that can be used to deliminate interpolated expressions.
	/// 
	/// Every entry in the 'CSharpLexerOutput.TriviaTextSpanList' will include the opening and closing delimiters.
	/// So, when parsing the interpolated expression, it is necessary to skip by 'DelimiterLength'.
	/// </summary>
    public int DelimiterLength { get; }
}
