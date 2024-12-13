using System.Collections.Immutable;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;
using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;

namespace Luthetus.CompilerServices.CSharp.LexerCase;

public struct CSharpLexer
{
	private readonly StringWalker _stringWalker;
    private readonly List<ISyntaxToken> _syntaxTokenList = new();
    private readonly DiagnosticBag _diagnosticBag = new();

    public CSharpLexer(ResourceUri resourceUri, string sourceText)
    {
    	#if DEBUG
    	++LuthetusDebugSomething.Lexer_ConstructorInvocationCount;
    	#endif
    	
    	ResourceUri = resourceUri;
        SourceText = sourceText;
    	LexerKeywords = CSharpKeywords.LexerKeywords;
    	
    	_stringWalker = new(resourceUri, sourceText);
    }
    
    private byte _decorationByteLastEscapeCharacter = (byte)GenericDecorationKind.None;

    public List<TextEditorTextSpan> EscapeCharacterList { get; } = new();
    
    public ResourceUri ResourceUri { get; }
    public string SourceText { get; }
    public LexerKeywords LexerKeywords { get; }
    
    public List<ISyntaxToken> SyntaxTokenList => _syntaxTokenList;
    public List<TextEditorDiagnostic> DiagnosticList => _diagnosticBag.ToList();

    public void Lex()
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
                    LexIdentifierOrKeywordOrKeywordContextual();
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
                    LexerUtils.LexCharLiteralToken(_stringWalker, _syntaxTokenList, EscapeCharacterList);
                    break;
                case '"':
                	LexString(countDollarSign: 0, useVerbatim: false);
                    break;
                case '/':
                    if (_stringWalker.PeekCharacter(1) == '/')
                    {
                        LexerUtils.LexCommentSingleLineToken(_stringWalker, _syntaxTokenList);
                    }
                    else if (_stringWalker.PeekCharacter(1) == '*')
                    {
                        LexerUtils.LexCommentMultiLineToken(_stringWalker, _syntaxTokenList);
                    }
                    else
                    {
                        var entryPositionIndex = _stringWalker.PositionIndex;
				        _stringWalker.ReadCharacter();
				        var textSpan = new TextEditorTextSpan(entryPositionIndex, _stringWalker.PositionIndex, (byte)GenericDecorationKind.None, _stringWalker.ResourceUri, _stringWalker.SourceText);
				        _syntaxTokenList.Add(new DivisionToken(textSpan));
                    }
                    break;
                case '+':
                    if (_stringWalker.PeekCharacter(1) == '+')
                    {
                        LexerUtils.LexPlusPlusToken(_stringWalker, _syntaxTokenList);
                    }
                    else
                    {
                        var entryPositionIndex = _stringWalker.PositionIndex;
				        _stringWalker.ReadCharacter();
				        var textSpan = new TextEditorTextSpan(entryPositionIndex, _stringWalker.PositionIndex, (byte)GenericDecorationKind.None, _stringWalker.ResourceUri, _stringWalker.SourceText);
				        _syntaxTokenList.Add(new PlusToken(textSpan));
                    }
                    break;
                case '-':
                    if (_stringWalker.PeekCharacter(1) == '-')
                    {
                        LexerUtils.LexMinusMinusToken(_stringWalker, _syntaxTokenList);
                    }
                    else
                    {
                        var entryPositionIndex = _stringWalker.PositionIndex;
				        _stringWalker.ReadCharacter();
				        var textSpan = new TextEditorTextSpan(entryPositionIndex, _stringWalker.PositionIndex, (byte)GenericDecorationKind.None, _stringWalker.ResourceUri, _stringWalker.SourceText);
				        _syntaxTokenList.Add(new MinusToken(textSpan));
                    }
                    break;
                case '=':
                    if (_stringWalker.PeekCharacter(1) == '=')
                    {
                        LexerUtils.LexEqualsEqualsToken(_stringWalker, _syntaxTokenList);
                    }
                    else
                    {
                        var entryPositionIndex = _stringWalker.PositionIndex;
				        _stringWalker.ReadCharacter();
				        var textSpan = new TextEditorTextSpan(entryPositionIndex, _stringWalker.PositionIndex, (byte)GenericDecorationKind.None, _stringWalker.ResourceUri, _stringWalker.SourceText);
				        _syntaxTokenList.Add(new EqualsToken(textSpan));
                    }
                    break;
                case '?':
                    if (_stringWalker.PeekCharacter(1) == '?')
                    {
                        LexerUtils.LexQuestionMarkQuestionMarkToken(_stringWalker, _syntaxTokenList);
                    }
                    else
                    {
                        var entryPositionIndex = _stringWalker.PositionIndex;
				        _stringWalker.ReadCharacter();
				        var textSpan = new TextEditorTextSpan(entryPositionIndex, _stringWalker.PositionIndex, (byte)GenericDecorationKind.None, _stringWalker.ResourceUri, _stringWalker.SourceText);
				        _syntaxTokenList.Add(new QuestionMarkToken(textSpan));
                    }
                    break;
                case '*':
                {
                	var entryPositionIndex = _stringWalker.PositionIndex;
			        _stringWalker.ReadCharacter();
			        var textSpan = new TextEditorTextSpan(entryPositionIndex, _stringWalker.PositionIndex, (byte)GenericDecorationKind.None, _stringWalker.ResourceUri, _stringWalker.SourceText);
			        _syntaxTokenList.Add(new StarToken(textSpan));
                    break;
                }
                case '!':
                {
                    var entryPositionIndex = _stringWalker.PositionIndex;
			        _stringWalker.ReadCharacter();
			        var textSpan = new TextEditorTextSpan(entryPositionIndex, _stringWalker.PositionIndex, (byte)GenericDecorationKind.None, _stringWalker.ResourceUri, _stringWalker.SourceText);
			        _syntaxTokenList.Add(new BangToken(textSpan));
                    break;
                }
                case ';':
                {
                    var entryPositionIndex = _stringWalker.PositionIndex;
			        _stringWalker.ReadCharacter();
			        var textSpan = new TextEditorTextSpan(entryPositionIndex, _stringWalker.PositionIndex, (byte)GenericDecorationKind.None, _stringWalker.ResourceUri, _stringWalker.SourceText);
			        _syntaxTokenList.Add(new StatementDelimiterToken(textSpan));
                    break;
                }
                case '(':
                {
                    var entryPositionIndex = _stringWalker.PositionIndex;
			        _stringWalker.ReadCharacter();
			        var textSpan = new TextEditorTextSpan(entryPositionIndex, _stringWalker.PositionIndex, (byte)GenericDecorationKind.None, _stringWalker.ResourceUri, _stringWalker.SourceText);
			        _syntaxTokenList.Add(new OpenParenthesisToken(textSpan));
                    break;
                }
                case ')':
                {
                    var entryPositionIndex = _stringWalker.PositionIndex;
			        _stringWalker.ReadCharacter();
			        var textSpan = new TextEditorTextSpan(entryPositionIndex, _stringWalker.PositionIndex, (byte)GenericDecorationKind.None, _stringWalker.ResourceUri, _stringWalker.SourceText);
			        _syntaxTokenList.Add(new CloseParenthesisToken(textSpan));
                    break;
                }
                case '{':
                {
                    var entryPositionIndex = _stringWalker.PositionIndex;
			        _stringWalker.ReadCharacter();
			        var textSpan = new TextEditorTextSpan(entryPositionIndex, _stringWalker.PositionIndex, (byte)GenericDecorationKind.None, _stringWalker.ResourceUri, _stringWalker.SourceText);
			        _syntaxTokenList.Add(new OpenBraceToken(textSpan));
                    break;
                }
                case '}':
                {
                    var entryPositionIndex = _stringWalker.PositionIndex;
			        _stringWalker.ReadCharacter();
			        var textSpan = new TextEditorTextSpan(entryPositionIndex, _stringWalker.PositionIndex, (byte)GenericDecorationKind.None, _stringWalker.ResourceUri, _stringWalker.SourceText);
			        _syntaxTokenList.Add(new CloseBraceToken(textSpan));
                    break;
                }
                case '<':
                {
                    var entryPositionIndex = _stringWalker.PositionIndex;
			        _stringWalker.ReadCharacter();
			        var textSpan = new TextEditorTextSpan(entryPositionIndex, _stringWalker.PositionIndex, (byte)GenericDecorationKind.None, _stringWalker.ResourceUri, _stringWalker.SourceText);
			        _syntaxTokenList.Add(new OpenAngleBracketToken(textSpan));
                    break;
                }
                case '>':
                {
                    var entryPositionIndex = _stringWalker.PositionIndex;
			        _stringWalker.ReadCharacter();
			        var textSpan = new TextEditorTextSpan(entryPositionIndex, _stringWalker.PositionIndex, (byte)GenericDecorationKind.None, _stringWalker.ResourceUri, _stringWalker.SourceText);
			        _syntaxTokenList.Add(new CloseAngleBracketToken(textSpan));
                    break;
                }
                case '[':
                {
                    var entryPositionIndex = _stringWalker.PositionIndex;
			        _stringWalker.ReadCharacter();
			        var textSpan = new TextEditorTextSpan(entryPositionIndex, _stringWalker.PositionIndex, (byte)GenericDecorationKind.None, _stringWalker.ResourceUri, _stringWalker.SourceText);
			        _syntaxTokenList.Add(new OpenSquareBracketToken(textSpan));
                    break;
                }
                case ']':
                {
                    var entryPositionIndex = _stringWalker.PositionIndex;
			        _stringWalker.ReadCharacter();
			        var textSpan = new TextEditorTextSpan(entryPositionIndex, _stringWalker.PositionIndex, (byte)GenericDecorationKind.None, _stringWalker.ResourceUri, _stringWalker.SourceText);
			        _syntaxTokenList.Add(new CloseSquareBracketToken(textSpan));
                    break;
                }
                case '$':
                	if (_stringWalker.NextCharacter == '"')
                	{
                		LexString(countDollarSign: 1, useVerbatim: false);
					}
					else if (_stringWalker.PeekCharacter(1) == '@' && _stringWalker.PeekCharacter(2) == '"')
					{
						LexString(countDollarSign: 1, useVerbatim: true);
                	}
                	else if (_stringWalker.NextCharacter == '$')
                	{
                		var entryPositionIndex = _stringWalker.PositionIndex;
                		
                		// The while loop starts counting from and including the first dollar sign.
                		var countDollarSign = 0;
                	
                		while (!_stringWalker.IsEof)
                		{
                			if (_stringWalker.CurrentCharacter != '$')
                				break;
                			
            				++countDollarSign;
            				_ = _stringWalker.ReadCharacter();
                		}
                		
                		// Only the last '$' (dollar sign character) will be syntax highlighted
                		// if this code is NOT included.
                		var textSpan = new TextEditorTextSpan(entryPositionIndex, _stringWalker.PositionIndex, (byte)GenericDecorationKind.StringLiteral, _stringWalker.ResourceUri, _stringWalker.SourceText);
				        _syntaxTokenList.Add(new StringLiteralToken(textSpan));
                		
                		// From the LexString(...) method:
                		// 	"awkwardly even if there are many of these it is expected
                		//      that the last one will not have been consumed."
                		_ = _stringWalker.BacktrackCharacter();
                		
                		if (_stringWalker.NextCharacter == '"')
	                		LexString(countDollarSign: countDollarSign, useVerbatim: false);
                	}
                	else
                	{
                    	LexerUtils.LexDollarSignToken(_stringWalker, _syntaxTokenList);
                    }
                    break;
                case '@':
                	if (_stringWalker.NextCharacter == '"')
                		LexString(countDollarSign: 0, useVerbatim: true);
					else if (_stringWalker.PeekCharacter(1) == '$' && _stringWalker.PeekCharacter(2) == '"')
						LexString(countDollarSign: 1, useVerbatim: true);
                	else
                    	LexerUtils.LexAtToken(_stringWalker, _syntaxTokenList);
                    break;
                case ':':
                {
                    var entryPositionIndex = _stringWalker.PositionIndex;
			        _stringWalker.ReadCharacter();
			        var textSpan = new TextEditorTextSpan(entryPositionIndex, _stringWalker.PositionIndex, (byte)GenericDecorationKind.None, _stringWalker.ResourceUri, _stringWalker.SourceText);
			        _syntaxTokenList.Add(new ColonToken(textSpan));
                    break;
                }
                case '.':
                {
                    var entryPositionIndex = _stringWalker.PositionIndex;
			        _stringWalker.ReadCharacter();
			        var textSpan = new TextEditorTextSpan(entryPositionIndex, _stringWalker.PositionIndex, (byte)GenericDecorationKind.None, _stringWalker.ResourceUri, _stringWalker.SourceText);
			        _syntaxTokenList.Add(new MemberAccessToken(textSpan));
                    break;
                }
                case ',':
                {
                    var entryPositionIndex = _stringWalker.PositionIndex;
			        _stringWalker.ReadCharacter();
			        var textSpan = new TextEditorTextSpan(entryPositionIndex, _stringWalker.PositionIndex, (byte)GenericDecorationKind.None, _stringWalker.ResourceUri, _stringWalker.SourceText);
			        _syntaxTokenList.Add(new CommaToken(textSpan));
                    break;
                }
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
    
    /// <summary>
    /// The invoker of this method is expected to count the amount of '$' (dollar sign characters).
    /// When it comes to raw strings however, this logic will counted inside of this method.
    ///
    /// The reason being: you don't know if it is a string until you've read all of the '$' (dollar sign characters).
    /// So in order to invoke this method the invoker had to have counted them.
    /// </summary>
    private void LexString(int countDollarSign, bool useVerbatim)
    {
    	var entryPositionIndex = _stringWalker.PositionIndex;

		var useInterpolation = countDollarSign > 0;
		
		if (useInterpolation)
        	_ = _stringWalker.ReadCharacter(); // Move past the '$' (dollar sign character); awkwardly even if there are many of these it is expected that the last one will not have been consumed.
        if (useVerbatim)
        	_ = _stringWalker.ReadCharacter(); // Move past the '@' (at character)
		
		var useRaw = false;
		int countDoubleQuotes = 0;
		
    	if (!useVerbatim && _stringWalker.PeekCharacter(1) == '\"' && _stringWalker.PeekCharacter(2) == '\"')
    	{
    		useRaw = true;
    		
    		// Count the amount of double quotes to be used as the delimiter.
			while (!_stringWalker.IsEof)
			{
				if (_stringWalker.CurrentCharacter != '\"')
					break;
	
				++countDoubleQuotes;
				_ = _stringWalker.ReadCharacter();
			}
    	}
    	else
    	{
        	_ = _stringWalker.ReadCharacter(); // Move past the '"' (double quote character)
        }

        while (!_stringWalker.IsEof)
        {
			if (_stringWalker.CurrentCharacter == '\"')
			{
				if (useRaw)
				{
					var matchDoubleQuotes = 0;
					
					while (!_stringWalker.IsEof)
		    		{
		    			if (_stringWalker.CurrentCharacter != '\"')
		    				break;
		    			
		    			_ = _stringWalker.ReadCharacter();
		    			if (++matchDoubleQuotes == countDoubleQuotes)
		    				goto foundEndDelimiter;
		    		}
		    		
		    		continue;
				}
				else if (useVerbatim && _stringWalker.NextCharacter == '\"')
				{
					if (EscapeCharacterList is not null)
					{
						EscapeCharacterListAdd(new TextEditorTextSpan(
				            _stringWalker.PositionIndex,
				            _stringWalker.PositionIndex + 2,
				            (byte)GenericDecorationKind.EscapeCharacterPrimary,
				            _stringWalker.ResourceUri,
				            _stringWalker.SourceText));
					}
	
					_ = _stringWalker.ReadCharacter();
				}
				else
				{
					_ = _stringWalker.ReadCharacter();
					break;
				}
			}
			else if (!useVerbatim && _stringWalker.CurrentCharacter == '\\')
			{
				if (EscapeCharacterList is not null)
				{
					EscapeCharacterListAdd(new TextEditorTextSpan(
			            _stringWalker.PositionIndex,
			            _stringWalker.PositionIndex + 2,
			            (byte)GenericDecorationKind.EscapeCharacterPrimary,
			            _stringWalker.ResourceUri,
			            _stringWalker.SourceText));
				}

				// Presuming the escaped text is 2 characters, then read an extra character.
				_ = _stringWalker.ReadCharacter();
			}
			else if (useInterpolation && _stringWalker.CurrentCharacter == '{')
			{
				// With raw, one is escaping by way of typing less.
				// With normal interpolation, one is escaping by way of typing more.
				//
				// Thus, these are two separate cases written as an if-else.
				if (useRaw)
				{
					var interpolationTemporaryPositionIndex = _stringWalker.PositionIndex;
					var matchBrace = 0;
						
					while (!_stringWalker.IsEof)
		    		{
		    			if (_stringWalker.CurrentCharacter != '{')
		    				break;
		    			
		    			_ = _stringWalker.ReadCharacter();
		    			if (++matchBrace >= countDollarSign)
		    			{
		    				// Found yet another '{' match beyond what was needed.
		    				// So, this logic will match from the inside to the outside.
		    				if (_stringWalker.CurrentCharacter == '{')
		    				{
		    					++interpolationTemporaryPositionIndex;
		    				}
		    				else
		    				{
		    					var innerTextSpan = new TextEditorTextSpan(
						            entryPositionIndex,
						            interpolationTemporaryPositionIndex,
						            (byte)GenericDecorationKind.StringLiteral,
						            _stringWalker.ResourceUri,
						            _stringWalker.SourceText);
						        _syntaxTokenList.Add(new StringLiteralToken(innerTextSpan));
							
								// 'LexInterpolatedExpression' is expected to consume one more after it is finished.
								// Thus, if this while loop were to consume, it would skip the
								// closing double quotes if the expression were the last thing in the string.
								//
								// So, a backtrack is done.
								LexInterpolatedExpression(countDollarSign);
								entryPositionIndex = _stringWalker.PositionIndex;
								_stringWalker.BacktrackCharacter();
		    				}
		    			}
		    		}
				}
				else
				{
					if (_stringWalker.NextCharacter == '{')
					{
						_ = _stringWalker.ReadCharacter();
					}
					else
					{
						var innerTextSpan = new TextEditorTextSpan(
				            entryPositionIndex,
				            _stringWalker.PositionIndex,
				            (byte)GenericDecorationKind.StringLiteral,
				            _stringWalker.ResourceUri,
				            _stringWalker.SourceText);
				        _syntaxTokenList.Add(new StringLiteralToken(innerTextSpan));
					
						// 'LexInterpolatedExpression' is expected to consume one more after it is finished.
						// Thus, if this while loop were to consume, it would skip the
						// closing double quotes if the expression were the last thing in the string.
						LexInterpolatedExpression(countDollarSign);
						entryPositionIndex = _stringWalker.PositionIndex;
						continue;
					}
				}
			}

            _ = _stringWalker.ReadCharacter();
        }

		foundEndDelimiter:

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            _stringWalker.PositionIndex,
            (byte)GenericDecorationKind.StringLiteral,
            _stringWalker.ResourceUri,
            _stringWalker.SourceText);

        _syntaxTokenList.Add(new StringLiteralToken(textSpan));
    }
    
    private void LexInterpolatedExpression(int countDollarSign)
    {
    	var entryPositionIndex = _stringWalker.PositionIndex;
		
		if (_stringWalker.CurrentCharacter == '{')
		{
			// The normal interpolation will invoke this method with '{' as the current character.
			//
			// But, raw interpolation will invoke this method 1 index further than the final '{' that
			// deliminates the start of the interpolated expression.
        	_ = _stringWalker.ReadCharacter();
        }
        
    	var unmatchedBraceCounter = countDollarSign;
		
		while (!_stringWalker.IsEof)
		{
			if (_stringWalker.CurrentCharacter == '{')
			{
				++unmatchedBraceCounter;
			}
			else if (_stringWalker.CurrentCharacter == '}')
			{
				if (--unmatchedBraceCounter <= 0)
					break;
			}

			_ = _stringWalker.ReadCharacter();
		}

		_ = _stringWalker.ReadCharacter();
    }
    
    private void EscapeCharacterListAdd(TextEditorTextSpan textSpan)
    {
    	if (EscapeCharacterList.Count > 0)
    	{
    		var lastEntry = EscapeCharacterList[^1];
    		
    		if (lastEntry.EndingIndexExclusive == textSpan.StartingIndexInclusive &&
    			_decorationByteLastEscapeCharacter == (byte)GenericDecorationKind.EscapeCharacterPrimary)
    		{
    			textSpan = textSpan with
    			{
    				DecorationByte = (byte)GenericDecorationKind.EscapeCharacterSecondary,
    			};
    		}
    	}
    	
    	_decorationByteLastEscapeCharacter = textSpan.DecorationByte;
    	EscapeCharacterList.Add(textSpan);
    }
    
    public void LexIdentifierOrKeywordOrKeywordContextual()
    {
        var entryPositionIndex = _stringWalker.PositionIndex;

        while (!_stringWalker.IsEof)
        {
            if (!char.IsLetterOrDigit(_stringWalker.CurrentCharacter) &&
                _stringWalker.CurrentCharacter != '_')
            {
                break;
            }

            _ = _stringWalker.ReadCharacter();
        }

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            _stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            _stringWalker.ResourceUri,
            _stringWalker.SourceText);

        var textValue = textSpan.GetText();

        if (CSharpKeywords.ALL_KEYWORDS_HASH_SET.Contains(textValue))
        {
        	var decorationByte = (byte)GenericDecorationKind.Keyword;
        
            if (CSharpKeywords.LexerKeywords.ControlKeywords.Contains(textValue))
            	decorationByte = (byte)GenericDecorationKind.KeywordControl;
            
            textSpan = textSpan with
            {
                DecorationByte = decorationByte,
            };

            if (CSharpKeywords.LexerKeywords.ContextualKeywords.Contains(textValue))
            {
                _syntaxTokenList.Add(new KeywordContextualToken(textSpan, LexerUtils.GetSyntaxKindForContextualKeyword(textSpan)));
                return;
            }

            _syntaxTokenList.Add(new KeywordToken(textSpan, LexerUtils.GetSyntaxKindForKeyword(textSpan)));
            return;
        }

        _syntaxTokenList.Add(new IdentifierToken(textSpan));
    }
}