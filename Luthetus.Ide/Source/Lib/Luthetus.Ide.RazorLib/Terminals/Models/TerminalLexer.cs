using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

public class TerminalLexer : LuthLexer
{
    private readonly TerminalResource _terminalResource;

    public TerminalLexer(TerminalResource terminalResource, string sourceText)
        : base(terminalResource.ResourceUri, sourceText, LuthLexerKeywords.Empty)
    {
        _terminalResource = terminalResource;
    }

    public override void Lex()
    {
        // Skip to the 'editable' position index.
        //     (i.e.): the final line, and the first available column after the working directory text.
        {
            // Hackily look at the last entry of the 'manual decoration list' because every
            //     'working directory text' token is added to this list.
            var workingDirectoryTextSpan = _terminalResource.ManualDecorationTextSpanList.Last();

            for (int i = 0; i < workingDirectoryTextSpan.EndingIndexExclusive; i++)
            {
                if (_stringWalker.IsEof)
                    break;

                _ = _stringWalker.ReadCharacter();
            }
        }

        // Skip and leading whitespace
        _ = _stringWalker.ReadWhitespace();

        // Lex the first 'word' or string-like token
        //
        // Ex:
        //     cd ..                        // 'cd' would be a word.
        //     "/dot net/source/" --version // "/dot net/source/" would be a string deliminated with double quotes.
        //     '/programs/git/' init        // '/programs/git/' would be a string deliminated with single quotes.
        if (_stringWalker.CurrentCharacter == '"')
        {
            LuthLexerUtils.LexStringLiteralToken(_stringWalker, _syntaxTokenList);
        }
        else if (_stringWalker.CurrentCharacter == '\'')
        {
            LexStringLiteralTokenWithSingleQuoteDelimiter(_stringWalker, _syntaxTokenList);
        }
        else
        {
            var wordTuple = _stringWalker.ReadWordTuple();
            _syntaxTokenList.Add(new IdentifierToken(wordTuple.textSpan));
        }

        // Rewrite the token that was read to be an identifier token.
        //
        // This code is a bit odd, and hacky, because the 'LexStringLiteralToken' will construct a string token,
        //     even though in the this context, we are reading the target file path for the terminal command.
        var lastEntry = _syntaxTokenList[^1];
        _syntaxTokenList[^1] = new IdentifierToken(lastEntry.TextSpan with
        {
            DecorationByte = (byte)TerminalDecorationKind.TargetFilePath
        });

        while (!_stringWalker.IsEof)
        {
            switch (_stringWalker.CurrentCharacter)
            {
                case '"':
                    LuthLexerUtils.LexStringLiteralToken(_stringWalker, _syntaxTokenList);
                    _syntaxTokenList[^1] = new IdentifierToken(lastEntry.TextSpan with
                    {
                        DecorationByte = (byte)TerminalDecorationKind.StringLiteral
                    });
                    break;
                case '\'':
                    LexStringLiteralTokenWithSingleQuoteDelimiter(_stringWalker, _syntaxTokenList);
                    _syntaxTokenList[^1] = new IdentifierToken(lastEntry.TextSpan with
                    {
                        DecorationByte = (byte)TerminalDecorationKind.StringLiteral
                    });
                    break;
                default:
                    _ = _stringWalker.ReadCharacter();
                    break;
            }
        }

        var endOfFileTextSpan = new TextEditorTextSpan(
            _stringWalker.PositionIndex,
            _stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            _stringWalker.ResourceUri,
            _stringWalker.SourceText);

        _syntaxTokenList.Add(new EndOfFileToken(endOfFileTextSpan));
    }

    public static void LexStringLiteralTokenWithSingleQuoteDelimiter(
        StringWalker stringWalker, List<ISyntaxToken> syntaxTokens)
    {
        var entryPositionIndex = stringWalker.PositionIndex;

        // Move past the initial opening character
        _ = stringWalker.ReadCharacter();

        // Declare outside the while loop to avoid overhead of redeclaring each loop? not sure
        var wasClosingCharacter = false;

        while (!stringWalker.IsEof)
        {
            switch (stringWalker.CurrentCharacter)
            {
                case '\'':
                    wasClosingCharacter = true;
                    break;
                default:
                    break;
            }

            _ = stringWalker.ReadCharacter();

            if (wasClosingCharacter)
                break;
        }

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            stringWalker.PositionIndex,
            (byte)GenericDecorationKind.StringLiteral,
            stringWalker.ResourceUri,
            stringWalker.SourceText);

        syntaxTokens.Add(new StringLiteralToken(textSpan));
    }
}
