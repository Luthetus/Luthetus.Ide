using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;

namespace Luthetus.CompilerServices.CSharp.LexerCase;

public class CSharpLexer : Lexer
{
	private readonly List<TextEditorTextSpan> _escapeCharacterList = new();

    public CSharpLexer(ResourceUri resourceUri, string sourceText)
        : base(
            resourceUri,
            sourceText,
            new LexerKeywords(CSharpKeywords.NON_CONTEXTUAL_KEYWORDS, CSharpKeywords.CONTROL_KEYWORDS, CSharpKeywords.CONTEXTUAL_KEYWORDS))
    {
    }

    public ImmutableArray<TextEditorTextSpan> EscapeCharacterList => _escapeCharacterList.ToImmutableArray();

    public override void Lex()
    {
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
                    LexerUtils.LexIdentifierOrKeywordOrKeywordContextual(_stringWalker, _syntaxTokenList, LexerKeywords);
                    break;
                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    LexerUtils.LexNumericLiteralToken(_stringWalker, _syntaxTokenList);
                    break;
				case '\'':
                    LexerUtils.LexCharLiteralToken(_stringWalker, _syntaxTokenList, _escapeCharacterList);
                    break;
                case '"':
                    LexerUtils.LexStringLiteralToken(_stringWalker, _syntaxTokenList, _escapeCharacterList);
                    break;
                case '/':
                    if (_stringWalker.PeekCharacter(1) == '/')
                        LexerUtils.LexCommentSingleLineToken(_stringWalker, _syntaxTokenList);
                    else if (_stringWalker.PeekCharacter(1) == '*')
                        LexerUtils.LexCommentMultiLineToken(_stringWalker, _syntaxTokenList);
                    else
                        LexerUtils.LexDivisionToken(_stringWalker, _syntaxTokenList);

                    break;
                case '+':
                    if (_stringWalker.PeekCharacter(1) == '+')
                        LexerUtils.LexPlusPlusToken(_stringWalker, _syntaxTokenList);
                    else
                        LexerUtils.LexPlusToken(_stringWalker, _syntaxTokenList);

                    break;
                case '-':
                    if (_stringWalker.PeekCharacter(1) == '-')
                        LexerUtils.LexMinusMinusToken(_stringWalker, _syntaxTokenList);
                    else
                        LexerUtils.LexMinusToken(_stringWalker, _syntaxTokenList);

                    break;
                case '=':
                    if (_stringWalker.PeekCharacter(1) == '=')
                        LexerUtils.LexEqualsEqualsToken(_stringWalker, _syntaxTokenList);
                    else
                        LexerUtils.LexEqualsToken(_stringWalker, _syntaxTokenList);

                    break;
                case '?':
                    if (_stringWalker.PeekCharacter(1) == '?')
                        LexerUtils.LexQuestionMarkQuestionMarkToken(_stringWalker, _syntaxTokenList);
                    else
                        LexerUtils.LexQuestionMarkToken(_stringWalker, _syntaxTokenList);

                    break;
                case '*':
                    LexerUtils.LexStarToken(_stringWalker, _syntaxTokenList);
                    break;
                case '!':
                    LexerUtils.LexBangToken(_stringWalker, _syntaxTokenList);
                    break;
                case ';':
                    LexerUtils.LexStatementDelimiterToken(_stringWalker, _syntaxTokenList);
                    break;
                case '(':
                    LexerUtils.LexOpenParenthesisToken(_stringWalker, _syntaxTokenList);
                    break;
                case ')':
                    LexerUtils.LexCloseParenthesisToken(_stringWalker, _syntaxTokenList);
                    break;
                case '{':
                    LexerUtils.LexOpenBraceToken(_stringWalker, _syntaxTokenList);
                    break;
                case '}':
                    LexerUtils.LexCloseBraceToken(_stringWalker, _syntaxTokenList);
                    break;
                case '<':
                    LexerUtils.LexOpenAngleBracketToken(_stringWalker, _syntaxTokenList);
                    break;
                case '>':
                    LexerUtils.LexCloseAngleBracketToken(_stringWalker, _syntaxTokenList);
                    break;
                case '[':
                    LexerUtils.LexOpenSquareBracketToken(_stringWalker, _syntaxTokenList);
                    break;
                case ']':
                    LexerUtils.LexCloseSquareBracketToken(_stringWalker, _syntaxTokenList);
                    break;
                case '$':
                	if (_stringWalker.NextCharacter == '"')
                		LexStringInterpolation(_stringWalker, _syntaxTokenList);
                	else
                    	LexerUtils.LexDollarSignToken(_stringWalker, _syntaxTokenList);
                    break;
                case '@':
                	if (_stringWalker.NextCharacter == '"')
                		LexStringVerbatim(_stringWalker, _syntaxTokenList);
                	else
                    	LexerUtils.LexAtToken(_stringWalker, _syntaxTokenList);
                    break;
                case ':':
                    LexerUtils.LexColonToken(_stringWalker, _syntaxTokenList);
                    break;
                case '.':
                    LexerUtils.LexMemberAccessToken(_stringWalker, _syntaxTokenList);
                    break;
                case ',':
                    LexerUtils.LexCommaToken(_stringWalker, _syntaxTokenList);
                    break;
                case '#':
                    LexerUtils.LexPreprocessorDirectiveToken(_stringWalker, _syntaxTokenList);
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
    
    private void LexStringInterpolation(StringWalker stringWalker, List<ISyntaxToken> syntaxTokenList)
    {
    	var entryPositionIndex = stringWalker.PositionIndex;

        _ = stringWalker.ReadCharacter(); // Move past the '$' (dollar sign character)
        _ = stringWalker.ReadCharacter(); // Move past the '"' (double quote character)

        while (!stringWalker.IsEof)
        {
			if (stringWalker.CurrentCharacter == '\"')
			{
				_ = stringWalker.ReadCharacter();
				break;
			}
			else if (stringWalker.CurrentCharacter == '\\')
			{
				if (_escapeCharacterList is not null)
				{
					_escapeCharacterList.Add(new TextEditorTextSpan(
			            stringWalker.PositionIndex,
			            stringWalker.PositionIndex + 2,
			            (byte)GenericDecorationKind.EscapeCharacter,
			            stringWalker.ResourceUri,
			            stringWalker.SourceText));
				}

				// Presuming the escaped text is 2 characters, then read an extra character.
				_ = stringWalker.ReadCharacter();
			}
			else if (stringWalker.CurrentCharacter == '{')
			{
				if (stringWalker.NextCharacter == '{')
				{
					_ = stringWalker.ReadCharacter();
				}
				else
				{
					var innerTextSpan = new TextEditorTextSpan(
			            entryPositionIndex,
			            stringWalker.PositionIndex,
			            (byte)GenericDecorationKind.StringLiteral,
			            stringWalker.ResourceUri,
			            stringWalker.SourceText);
			        _syntaxTokenList.Add(new StringLiteralToken(innerTextSpan));
				
					// 'LexInterpolatedExpression' is expected to consume one more after it is finished.
					// Thus, if this while loop were to consume, it would skip the
					// closing double quotes if the expression were the last thing in the string.
					LexInterpolatedExpression(stringWalker, syntaxTokenList);
					entryPositionIndex = stringWalker.PositionIndex;
					continue;
				}
			}

            _ = stringWalker.ReadCharacter();
        }

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            stringWalker.PositionIndex,
            (byte)GenericDecorationKind.StringLiteral,
            stringWalker.ResourceUri,
            stringWalker.SourceText);

        _syntaxTokenList.Add(new StringLiteralToken(textSpan));
    }
    
    private void LexInterpolatedExpression(StringWalker stringWalker, List<ISyntaxToken> syntaxTokenList)
    {
    	var entryPositionIndex = stringWalker.PositionIndex;

        _ = stringWalker.ReadCharacter(); // Move past the '{' (open brace character)
        
    	var unmatchedBraceCounter = 1;
		
		while (!stringWalker.IsEof)
		{
			if (stringWalker.CurrentCharacter == '{')
			{
				++unmatchedBraceCounter;
			}
			else if (stringWalker.CurrentCharacter == '}')
			{
				if (--unmatchedBraceCounter <= 0)
					break;
			}

			_ = stringWalker.ReadCharacter();
		}

		_ = stringWalker.ReadCharacter();
    }
    
    private void LexStringVerbatim(StringWalker stringWalker, List<ISyntaxToken> syntaxTokenList)
    {
    	var entryPositionIndex = stringWalker.PositionIndex;

        _ = stringWalker.ReadCharacter(); // Move past the '@' (at character)
        _ = stringWalker.ReadCharacter(); // Move past the '"' (double quote character)

        while (!stringWalker.IsEof)
        {
			if (stringWalker.CurrentCharacter == '\"')
			{
				if (stringWalker.NextCharacter == '\"')
				{
					if (_escapeCharacterList is not null)
					{
						_escapeCharacterList.Add(new TextEditorTextSpan(
				            stringWalker.PositionIndex,
				            stringWalker.PositionIndex + 2,
				            (byte)GenericDecorationKind.EscapeCharacter,
				            stringWalker.ResourceUri,
				            stringWalker.SourceText));
					}
	
					_ = stringWalker.ReadCharacter();
				}
				else
				{
					_ = stringWalker.ReadCharacter();
					break;
				}
			}

            _ = stringWalker.ReadCharacter();
        }

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            stringWalker.PositionIndex,
            (byte)GenericDecorationKind.StringLiteral,
            stringWalker.ResourceUri,
            stringWalker.SourceText);

        _syntaxTokenList.Add(new StringLiteralToken(textSpan));
    }
}