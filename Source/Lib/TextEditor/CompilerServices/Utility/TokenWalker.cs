using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Utility;

public class TokenWalker
{
    private readonly ImmutableArray<ISyntaxToken> _tokenList;
    private readonly LuthDiagnosticBag _diagnosticBag;

    private int _index;

    public TokenWalker(ImmutableArray<ISyntaxToken> tokenList, LuthDiagnosticBag diagnosticBag)
    {
        _tokenList = tokenList;
        _diagnosticBag = diagnosticBag;
    }

    public ImmutableArray<ISyntaxToken> TokenList => _tokenList;
    public ISyntaxToken Current => Peek(0);
    public ISyntaxToken Next => Peek(1);
    public ISyntaxToken Previous => Peek(-1);
    public bool IsEof => Current.SyntaxKind == SyntaxKind.EndOfFileToken;

    /// <summary>If there are any tokens, then assume the final token is the end of file token. Otherwise, fabricate an end of file token.</summary>
    private ISyntaxToken EOF => _tokenList.Length > 0
        ? _tokenList[_tokenList.Length - 1]
        : new EndOfFileToken(new(0, 0, 0, new(string.Empty), string.Empty));

    /// <summary>The input to this method can be positive OR negative.<br/><br/>Returns <see cref="BadToken"/> when an index out of bounds error would've occurred.</summary>
    public ISyntaxToken Peek(int offset)
    {
        var index = _index + offset;

        if (index < 0)
            return GetBadToken();
        else if (index >= _tokenList.Length)
            return EOF; // Return the end of file token (the last token)

        return _tokenList[index];
    }

    public ISyntaxToken Consume()
    {
        if (_index >= _tokenList.Length)
            return EOF; // Return the end of file token (the last token)

        var consumedToken = _tokenList[_index++];

        return consumedToken;
    }

    public ISyntaxToken Backtrack()
    {
        if (_index > 0)
            _index--;

        return Peek(_index);
    }

    /// <summary>If the syntaxKind passed in does not match the current token, then a syntax token with that syntax kind will be fabricated and then returned instead.</summary>
    public ISyntaxToken Match(SyntaxKind expectedSyntaxKind)
    {
        var currentToken = Peek(0);

        // TODO: Checking for the text 'args' is likely not a good solution. When parsing a main method, it might have the method arguments: 'string[] args'. The issue here is that 'args' comes up as a keyword while being the identifier for that method argument.
        if (currentToken.TextSpan.GetText() == "args" && expectedSyntaxKind == SyntaxKind.IdentifierToken)
        {
            _ = Consume();
            return new IdentifierToken(currentToken.TextSpan);
        }

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
    public ISyntaxToken MatchRange(IEnumerable<SyntaxKind> validSyntaxKinds, SyntaxKind fabricationKind)
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