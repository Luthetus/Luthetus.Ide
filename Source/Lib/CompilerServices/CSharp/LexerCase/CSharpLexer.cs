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
    public CSharpLexer(ResourceUri resourceUri, string sourceText)
        : base(
            resourceUri,
            sourceText,
            new LexerKeywords(CSharpKeywords.NON_CONTEXTUAL_KEYWORDS, CSharpKeywords.CONTROL_KEYWORDS, CSharpKeywords.CONTEXTUAL_KEYWORDS))
    {
    }
    
    private byte _decorationByteLastEscapeCharacter = (byte)GenericDecorationKind.None;
    
    /// <summary>This is the active brace pair.</summary>
	private int _bracePairIndex = -1;

    public List<TextEditorTextSpan> EscapeCharacterList { get; } = new();
    
    /// <summary>
    /// If the '_bracePairIndex' is -1, and the current token SyntaxKind is CloseBraceToken
    /// then do not add modify the BracePairList.
    ///
    /// If the '_bracePairIndex' is -1 or it is a value tuple
    /// (int OpenBraceTokenIndex, int CloseBraceTokenIndex) that does not equal (-1, -1)
    /// then add a new entry to the list '.Add((TokenWalker.Index, -1))'
    ///
    /// If the last entry has a 'CloseBraceTokenIndex' of -1, and the current token SyntaxKind is CloseBraceToken
    /// then set the 'CloseBraceTokenIndex' of the last entry to TokenWalker.Index.
    ///
    /// This list is a flat representation of the hierarchical "scope" that is formed
    /// by the brace pairs at times encompassing eachother.
    ///
    /// It is sorted by each entry's 'OpenBraceTokenIndex'.
    ///
    /// In order to maintain the hierarchical structure while building this list,
    /// maintain state that contains the 'ParentBracePairIndex', 'FirstChildBracePairIndex', 'LastChildBracePairIndex'.
    /// </summary>
    public List<BracePairMetadata> BracePairList { get; set; } = new();

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
                    LexerUtils.LexCharLiteralToken(_stringWalker, _syntaxTokenList, EscapeCharacterList);
                    break;
                case '"':
                	LexString(
				    	_stringWalker,
				    	_syntaxTokenList,
				    	countDollarSign: 0,
				    	useVerbatim: false);
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
                    LexOpenBraceToken(_stringWalker, _syntaxTokenList);
                    break;
                case '}':
                    LexCloseBraceToken(_stringWalker, _syntaxTokenList);
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
                	{
                		LexString(
					    	_stringWalker,
					    	_syntaxTokenList,
					    	countDollarSign: 1,
					    	useVerbatim: false);
					}
					else if (_stringWalker.PeekCharacter(1) == '@' && _stringWalker.PeekCharacter(2) == '"')
					{
						LexString(
					    	_stringWalker,
					    	_syntaxTokenList,
					    	countDollarSign: 1,
					    	useVerbatim: true);
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
                		var textSpan = new TextEditorTextSpan(
				            entryPositionIndex,
				            _stringWalker.PositionIndex,
				            (byte)GenericDecorationKind.StringLiteral,
				            _stringWalker.ResourceUri,
				            _stringWalker.SourceText);
				        _syntaxTokenList.Add(new StringLiteralToken(textSpan));
                		
                		// From the LexString(...) method:
                		// 	"awkwardly even if there are many of these it is expected
                		//      that the last one will not have been consumed."
                		_ = _stringWalker.BacktrackCharacter();
                		
                		if (_stringWalker.NextCharacter == '"')
	                	{
	                		LexString(
						    	_stringWalker,
						    	_syntaxTokenList,
						    	countDollarSign: countDollarSign,
						    	useVerbatim: false);
						}
                	}
                	else
                	{
                    	LexerUtils.LexDollarSignToken(_stringWalker, _syntaxTokenList);
                    }
                    break;
                case '@':
                	if (_stringWalker.NextCharacter == '"')
                	{
                		LexString(
					    	_stringWalker,
					    	_syntaxTokenList,
					    	countDollarSign: 0,
					    	useVerbatim: true);
					}
					else if (_stringWalker.PeekCharacter(1) == '$' && _stringWalker.PeekCharacter(2) == '"')
					{
						LexString(
					    	_stringWalker,
					    	_syntaxTokenList,
					    	countDollarSign: 1,
					    	useVerbatim: true);
					}
                	else
                	{
                    	LexerUtils.LexAtToken(_stringWalker, _syntaxTokenList);
                    }
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
    
    /// <summary>
    /// The invoker of this method is expected to count the amount of '$' (dollar sign characters).
    /// When it comes to raw strings however, this logic will counted inside of this method.
    ///
    /// The reason being: you don't know if it is a string until you've read all of the '$' (dollar sign characters).
    /// So in order to invoke this method the invoker had to have counted them.
    /// </summary>
    private void LexString(
    	StringWalker stringWalker,
    	List<ISyntaxToken> syntaxTokenList,
    	int countDollarSign,
    	bool useVerbatim)
    {
    	var entryPositionIndex = stringWalker.PositionIndex;

		var useInterpolation = countDollarSign > 0;
		
		if (useInterpolation)
        	_ = stringWalker.ReadCharacter(); // Move past the '$' (dollar sign character); awkwardly even if there are many of these it is expected that the last one will not have been consumed.
        if (useVerbatim)
        	_ = stringWalker.ReadCharacter(); // Move past the '@' (at character)
		
		var useRaw = false;
		int countDoubleQuotes = 0;
		
    	if (!useVerbatim && stringWalker.PeekCharacter(1) == '\"' && stringWalker.PeekCharacter(2) == '\"')
    	{
    		useRaw = true;
    		
    		// Count the amount of double quotes to be used as the delimiter.
			while (!stringWalker.IsEof)
			{
				if (stringWalker.CurrentCharacter != '\"')
					break;
	
				++countDoubleQuotes;
				_ = stringWalker.ReadCharacter();
			}
    	}
    	else
    	{
        	_ = stringWalker.ReadCharacter(); // Move past the '"' (double quote character)
        }

        while (!stringWalker.IsEof)
        {
			if (stringWalker.CurrentCharacter == '\"')
			{
				if (useRaw)
				{
					var matchDoubleQuotes = 0;
					
					while (!stringWalker.IsEof)
		    		{
		    			if (stringWalker.CurrentCharacter != '\"')
		    				break;
		    			
		    			_ = stringWalker.ReadCharacter();
		    			if (++matchDoubleQuotes == countDoubleQuotes)
		    				goto foundEndDelimiter;
		    		}
		    		
		    		continue;
				}
				else if (useVerbatim && stringWalker.NextCharacter == '\"')
				{
					if (EscapeCharacterList is not null)
					{
						EscapeCharacterListAdd(new TextEditorTextSpan(
				            stringWalker.PositionIndex,
				            stringWalker.PositionIndex + 2,
				            (byte)GenericDecorationKind.EscapeCharacterPrimary,
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
			else if (!useVerbatim && stringWalker.CurrentCharacter == '\\')
			{
				if (EscapeCharacterList is not null)
				{
					EscapeCharacterListAdd(new TextEditorTextSpan(
			            stringWalker.PositionIndex,
			            stringWalker.PositionIndex + 2,
			            (byte)GenericDecorationKind.EscapeCharacterPrimary,
			            stringWalker.ResourceUri,
			            stringWalker.SourceText));
				}

				// Presuming the escaped text is 2 characters, then read an extra character.
				_ = stringWalker.ReadCharacter();
			}
			else if (useInterpolation && stringWalker.CurrentCharacter == '{')
			{
				// With raw, one is escaping by way of typing less.
				// With normal interpolation, one is escaping by way of typing more.
				//
				// Thus, these are two separate cases written as an if-else.
				if (useRaw)
				{
					var interpolationTemporaryPositionIndex = stringWalker.PositionIndex;
					var matchBrace = 0;
						
					while (!stringWalker.IsEof)
		    		{
		    			if (stringWalker.CurrentCharacter != '{')
		    				break;
		    			
		    			_ = stringWalker.ReadCharacter();
		    			if (++matchBrace >= countDollarSign)
		    			{
		    				// Found yet another '{' match beyond what was needed.
		    				// So, this logic will match from the inside to the outside.
		    				if (stringWalker.CurrentCharacter == '{')
		    				{
		    					++interpolationTemporaryPositionIndex;
		    				}
		    				else
		    				{
		    					var innerTextSpan = new TextEditorTextSpan(
						            entryPositionIndex,
						            interpolationTemporaryPositionIndex,
						            (byte)GenericDecorationKind.StringLiteral,
						            stringWalker.ResourceUri,
						            stringWalker.SourceText);
						        _syntaxTokenList.Add(new StringLiteralToken(innerTextSpan));
							
								// 'LexInterpolatedExpression' is expected to consume one more after it is finished.
								// Thus, if this while loop were to consume, it would skip the
								// closing double quotes if the expression were the last thing in the string.
								//
								// So, a backtrack is done.
								LexInterpolatedExpression(stringWalker, syntaxTokenList, countDollarSign);
								entryPositionIndex = stringWalker.PositionIndex;
								stringWalker.BacktrackCharacter();
		    				}
		    			}
		    		}
				}
				else
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
						LexInterpolatedExpression(stringWalker, syntaxTokenList, countDollarSign);
						entryPositionIndex = stringWalker.PositionIndex;
						continue;
					}
				}
			}

            _ = stringWalker.ReadCharacter();
        }

		foundEndDelimiter:

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            stringWalker.PositionIndex,
            (byte)GenericDecorationKind.StringLiteral,
            stringWalker.ResourceUri,
            stringWalker.SourceText);

        _syntaxTokenList.Add(new StringLiteralToken(textSpan));
    }
    
    private void LexInterpolatedExpression(StringWalker stringWalker, List<ISyntaxToken> syntaxTokenList, int countDollarSign)
    {
    	var entryPositionIndex = stringWalker.PositionIndex;
		
		if (stringWalker.CurrentCharacter == '{')
		{
			// The normal interpolation will invoke this method with '{' as the current character.
			//
			// But, raw interpolation will invoke this method 1 index further than the final '{' that
			// deliminates the start of the interpolated expression.
        	_ = stringWalker.ReadCharacter();
        }
        
    	var unmatchedBraceCounter = countDollarSign;
		
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
    
    public void LexOpenBraceToken(StringWalker stringWalker, List<ISyntaxToken> syntaxTokens)
    {
        var entryPositionIndex = stringWalker.PositionIndex;

        stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            stringWalker.ResourceUri,
            stringWalker.SourceText);

        syntaxTokens.Add(new OpenBraceToken(textSpan));
        
        // Child
        {
	        var childBracePair = new BracePairMetadata();
	    	childBracePair.OpenBraceTokenIndex = syntaxTokens.Count - 1;
			childBracePair.CloseBraceTokenIndex = -1;
			childBracePair.ParentBracePairIndex = _bracePairIndex;
			childBracePair.FirstChildBracePairIndex = -1;
			childBracePair.LastChildBracePairIndex = -1;
			
			BracePairList.Add(childBracePair);
		}
        
        // Parent
        if (_bracePairIndex != -1)
        {
        	var parentBracePair = BracePairList[_bracePairIndex];
        	
        	if (parentBracePair.FirstChildBracePairIndex == -1)
	    		parentBracePair.FirstChildBracePairIndex = BracePairList.Count - 1;
	    		
	    	parentBracePair.LastChildBracePairIndex = BracePairList.Count - 1;
	    	
	    	BracePairList[_bracePairIndex] = parentBracePair;
        }
        
        _bracePairIndex = BracePairList.Count - 1;
    }

    public void LexCloseBraceToken(StringWalker stringWalker, List<ISyntaxToken> syntaxTokens)
    {
        var entryPositionIndex = stringWalker.PositionIndex;

        stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            stringWalker.ResourceUri,
            stringWalker.SourceText);

        syntaxTokens.Add(new CloseBraceToken(textSpan));
        
        if (_bracePairIndex == -1)
        	return;
        
    	var bracePair = BracePairList[_bracePairIndex];
    	bracePair.CloseBraceTokenIndex = syntaxTokens.Count - 1;
    	BracePairList[_bracePairIndex] = bracePair;
    	_bracePairIndex = bracePair.ParentBracePairIndex;
    }
    
    public BracePairMetadata? GetBracePairByOpenBraceTokenIndex(int openBraceTokenIndex)
    {
    	foreach (var bracePair in BracePairList)
    	{
    		if (bracePair.OpenBraceTokenIndex == openBraceTokenIndex)
	    		return bracePair;
    	}
    	
    	return null;
    }
}