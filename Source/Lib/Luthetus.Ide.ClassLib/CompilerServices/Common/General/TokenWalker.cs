using System.Collections.Immutable;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax.SyntaxTokens;

namespace Luthetus.Ide.ClassLib.CompilerServices.Common.General;

public class TokenWalker
{
    private readonly ImmutableArray<ISyntaxToken> _tokens;

    public TokenWalker(ImmutableArray<ISyntaxToken> tokens)
    {
        _tokens = tokens;
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

    private BadToken GetBadToken() => new BadToken(new(0, 0, 0, new(string.Empty), string.Empty));
}