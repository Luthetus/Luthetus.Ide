using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.Ide.RazorLib.Terminals.Models;

public class TerminalLexer : LuthLexer
{
    public TerminalLexer(ResourceUri resourceUri, string sourceText)
        : base(resourceUri, sourceText, LuthLexerKeywords.Empty)
    {
    }

    public override void Lex()
    {
        return;
        while (!_stringWalker.IsEof)
        {
            switch (_stringWalker.CurrentCharacter)
            {
                /* Lowercase Letters */
                case 'a':
                case 'b':
                case 'c':
                case 'd':
                case 'e':
                case 'f':
                case 'g':
                case 'h':
                case 'i':
                case 'j':
                case 'k':
                case 'l':
                case 'm':
                case 'n':
                case 'o':
                case 'p':
                case 'q':
                case 'r':
                case 's':
                case 't':
                case 'u':
                case 'v':
                case 'w':
                case 'x':
                case 'y':
                case 'z':
                /* Uppercase Letters */
                case 'A':
                case 'B':
                case 'C':
                case 'D':
                case 'E':
                case 'F':
                case 'G':
                case 'H':
                case 'I':
                case 'J':
                case 'K':
                case 'L':
                case 'M':
                case 'N':
                case 'O':
                case 'P':
                case 'Q':
                case 'R':
                case 'S':
                case 'T':
                case 'U':
                case 'V':
                case 'W':
                case 'X':
                case 'Y':
                case 'Z':
                /* Underscore */
                case '_':
                    LuthLexerUtils.LexIdentifierOrKeywordOrKeywordContextual(_stringWalker, _syntaxTokenList, LexerKeywords);
                    break;
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    LuthLexerUtils.LexNumericLiteralToken(_stringWalker, _syntaxTokenList);
                    break;
                case '"':
                    LuthLexerUtils.LexStringLiteralToken(_stringWalker, _syntaxTokenList);
                    break;
                case '/':
                    if (_stringWalker.PeekCharacter(1) == '/')
                        LuthLexerUtils.LexCommentSingleLineToken(_stringWalker, _syntaxTokenList);
                    else if (_stringWalker.PeekCharacter(1) == '*')
                        LuthLexerUtils.LexCommentMultiLineToken(_stringWalker, _syntaxTokenList);
                    else
                        LuthLexerUtils.LexDivisionToken(_stringWalker, _syntaxTokenList);

                    break;
                case '+':
                    if (_stringWalker.PeekCharacter(1) == '+')
                        LuthLexerUtils.LexPlusPlusToken(_stringWalker, _syntaxTokenList);
                    else
                        LuthLexerUtils.LexPlusToken(_stringWalker, _syntaxTokenList);

                    break;
                case '-':
                    if (_stringWalker.PeekCharacter(1) == '-')
                        LuthLexerUtils.LexMinusMinusToken(_stringWalker, _syntaxTokenList);
                    else
                        LuthLexerUtils.LexMinusToken(_stringWalker, _syntaxTokenList);

                    break;
                case '=':
                    if (_stringWalker.PeekCharacter(1) == '=')
                        LuthLexerUtils.LexEqualsEqualsToken(_stringWalker, _syntaxTokenList);
                    else
                        LuthLexerUtils.LexEqualsToken(_stringWalker, _syntaxTokenList);

                    break;
                case '?':
                    if (_stringWalker.PeekCharacter(1) == '?')
                        LuthLexerUtils.LexQuestionMarkQuestionMarkToken(_stringWalker, _syntaxTokenList);
                    else
                        LuthLexerUtils.LexQuestionMarkToken(_stringWalker, _syntaxTokenList);

                    break;
                case '*':
                    LuthLexerUtils.LexStarToken(_stringWalker, _syntaxTokenList);
                    break;
                case '!':
                    LuthLexerUtils.LexBangToken(_stringWalker, _syntaxTokenList);
                    break;
                case ';':
                    LuthLexerUtils.LexStatementDelimiterToken(_stringWalker, _syntaxTokenList);
                    break;
                case '(':
                    LuthLexerUtils.LexOpenParenthesisToken(_stringWalker, _syntaxTokenList);
                    break;
                case ')':
                    LuthLexerUtils.LexCloseParenthesisToken(_stringWalker, _syntaxTokenList);
                    break;
                case '{':
                    LuthLexerUtils.LexOpenBraceToken(_stringWalker, _syntaxTokenList);
                    break;
                case '}':
                    LuthLexerUtils.LexCloseBraceToken(_stringWalker, _syntaxTokenList);
                    break;
                case '<':
                    LuthLexerUtils.LexOpenAngleBracketToken(_stringWalker, _syntaxTokenList);
                    break;
                case '>':
                    LuthLexerUtils.LexCloseAngleBracketToken(_stringWalker, _syntaxTokenList);
                    break;
                case '[':
                    LuthLexerUtils.LexOpenSquareBracketToken(_stringWalker, _syntaxTokenList);
                    break;
                case ']':
                    LuthLexerUtils.LexCloseSquareBracketToken(_stringWalker, _syntaxTokenList);
                    break;
                case '$':
                    LuthLexerUtils.LexDollarSignToken(_stringWalker, _syntaxTokenList);
                    break;
                case ':':
                    LuthLexerUtils.LexColonToken(_stringWalker, _syntaxTokenList);
                    break;
                case '.':
                    LuthLexerUtils.LexMemberAccessToken(_stringWalker, _syntaxTokenList);
                    break;
                case ',':
                    LuthLexerUtils.LexCommaToken(_stringWalker, _syntaxTokenList);
                    break;
                case '#':
                    LuthLexerUtils.LexPreprocessorDirectiveToken(_stringWalker, _syntaxTokenList);
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
}
