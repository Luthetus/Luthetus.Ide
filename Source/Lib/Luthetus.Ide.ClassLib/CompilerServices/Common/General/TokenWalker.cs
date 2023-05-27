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
    public bool IsEof => Current.SyntaxKind == SyntaxKind.EndOfFileToken;

    /// <summary>If there are any tokens, then assume the final token is the end of file token. Otherwise, fabricate an end of file token.</summary>
    private ISyntaxToken EOF => _tokens.Length > 0
        ? _tokens[_tokens.Length - 1]
        : new EndOfFileToken(
            new TextEditor.RazorLib.Lexing.TextEditorTextSpan(
                -1, -1, 0));

    private int _index;

    public ISyntaxToken Peek(int offset)
    {
        var index = _index + offset;

        if (index >= _tokens.Length)
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

        return _tokens[_index++];
    }

    public ISyntaxToken Backtrack()
    {
        if (_index > 0)
            _index--;

        return Peek(_index);
    }
}