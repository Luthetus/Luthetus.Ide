using System.Collections.Immutable;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax.SyntaxTokens;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.General;

public class TokenWalker
{
    private readonly ImmutableArray<ISyntaxToken> _tokens;
    private readonly LuthetusIdeDiagnosticBag _diagnosticBag;

    public TokenWalker(
        ImmutableArray<ISyntaxToken> tokens,
        LuthetusIdeDiagnosticBag diagnosticBag)
    {
        _tokens = tokens;
        _diagnosticBag = diagnosticBag;
    }

    public ImmutableArray<ISyntaxToken> Tokens => _tokens;
    public ISyntaxToken Current => Peek(0);
    public ISyntaxToken Next => Peek(1);
    public ISyntaxToken Previous => Peek(-1);
    public bool IsEof => Current.SyntaxKind == SyntaxKind.EndOfFileToken;

    /// <summary>If there are any tokens, then assume the final token is the end of file token. Otherwise, fabricate an end of file token.</summary>
    private ISyntaxToken EOF =>
        _tokens.Length > 0
            ? _tokens[_tokens.Length - 1]
            : new EndOfFileToken(new(0, 0, 0, new(string.Empty), string.Empty));

    private int _index;

    /// <summary>The input to this method can be positive OR negative.<br/><br/>Returns <see cref="BadToken"/> when an index out of bounds error would've occurred.</summary>
    public ISyntaxToken Peek(int offset)
    {
        var index = _index + offset;

        if (index < 0)
        {
            return GetBadToken();
        }
        else if (index >= _tokens.Length)
        {
            // Return the end of file token (the last token)
            return EOF;
        }

        return _tokens[index];
    }

    public ISyntaxToken Consume()
    {
        if (_index >= _tokens.Length)
        {
            // Return the end of file token (the last token)
            return EOF;
        }

        var consumedToken = _tokens[_index++];

        return consumedToken;
    }

    public ISyntaxToken Backtrack()
    {
        if (_index > 0)
            _index--;

        // TODO: (2023-05-30) Should 'StatementDelimiterTokenRecent' be set here? See logic for tracking whether the current statement is completed. This is used for determining the contextual var keyword.

        return Peek(_index);
    }

    /// <summary>If the syntaxKind passed in does not match the current token, then a syntax token with that syntax kind will be fabricated and then returned instead.</summary>
    public ISyntaxToken Match(
        SyntaxKind expectedSyntaxKind)
    {
        var currentToken = Peek(0);

        if (currentToken.SyntaxKind == expectedSyntaxKind)
            return Consume();

        var fabricatedToken = this.FabricateToken(expectedSyntaxKind);

        _diagnosticBag.ReportUnexpectedToken(
            fabricatedToken.TextSpan,
            currentToken.SyntaxKind.ToString(),
            expectedSyntaxKind.ToString());

        return fabricatedToken;
    }

    /// <summary>If the syntaxKind passed in does not match the current token, then a syntax token with that syntax kind will be fabricated and then returned instead.</summary>
    public ISyntaxToken MatchRange(
        IEnumerable<SyntaxKind> validSyntaxKinds,
        SyntaxKind fabricationKind)
    {
        var currentToken = Peek(0);

        if (validSyntaxKinds.Contains(currentToken.SyntaxKind))
            return Consume();

        var fabricatedToken = this.FabricateToken(fabricationKind);

        _diagnosticBag.ReportUnexpectedToken(
            fabricatedToken.TextSpan,
            currentToken.SyntaxKind.ToString(),
            fabricationKind.ToString());

        return fabricatedToken;
    }

    private BadToken GetBadToken() => new BadToken(new(0, 0, 0, new(string.Empty), string.Empty));
}