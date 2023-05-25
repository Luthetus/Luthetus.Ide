using System.Collections.Immutable;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;

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
    public bool IsEof => Current.SyntaxKind == SyntaxKind.EndOfFileToken;

    private int _index;

    public ISyntaxToken Peek(int offset)
    {
        var index = _index + offset;

        if (index >= _tokens.Length)
        {
            // Return the end of file token (the last token)
            return _tokens[_tokens.Length - 1];
        }

        return _tokens[index];
    }

    public ISyntaxToken Consume()
    {
        if (_index >= _tokens.Length)
        {
            // Return the end of file token (the last token)
            return _tokens[_tokens.Length - 1];
        }

        return _tokens[_index++];
    }

    public ISyntaxToken Backtrack()
    {
        if (_index > 0)
            _index--;

        return Peek(_index);
    }
}