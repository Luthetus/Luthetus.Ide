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
    private readonly List<ISyntaxToken> _syntaxTokenList = new();
    private readonly DiagnosticBag _diagnosticBag = new();
    public static TimeSpan LongestLexTimeSpan = TimeSpan.Zero;
    public static TimeSpan TotalLexTimeSpan = TimeSpan.Zero;
    
    /// <summary>
    /// There are 1,820 CSharpResource's in the CSharpCompilerService.
    /// I don't want to bombard my console with noise by
    /// writing the TotalLexTimeSpan everytime.
    ///
    /// So I'll hackily write it out only 20 times by counting
    /// the amount of Lex invocations.
    ///
    /// Preferably this would be once for the final resource,
    /// but this does not matter, I'm just checking something quickly
    /// then deleting this code.
    /// </summary>
    public static int Hacky_Count_Before_Start_ConsoleWrite = 1800;
    public static int Hacky_Count = 0;

    public CSharpLexer(ResourceUri resourceUri, string sourceText)
    {
    	#if DEBUG
    	++LuthetusDebugSomething.Lexer_ConstructorInvocationCount;
    	#endif
    	
    	ResourceUri = resourceUri;
        SourceText = sourceText;
    	LexerKeywords = CSharpKeywords.LexerKeywords;
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
    	++Hacky_Count;
    	
    	var startTime = DateTime.UtcNow;
    
    	var stringWalker = new StringWalkerStruct(ResourceUri, SourceText);
    	
        while (!stringWalker.IsEof)
        {
            switch (stringWalker.CurrentCharacter)
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
                    LexIdentifierOrKeywordOrKeywordContextual(ref stringWalker);
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
                    LexNumericLiteralToken(ref stringWalker);
                    break;
				case '\'':
                    LexCharLiteralToken(ref stringWalker);
                    break;
                case '"':
                	LexString(ref stringWalker, countDollarSign: 0, useVerbatim: false);
                    break;
                case '/':
                    if (stringWalker.PeekCharacter(1) == '/')
                    {
                        LexCommentSingleLineToken(ref stringWalker);
                    }
                    else if (stringWalker.PeekCharacter(1) == '*')
                    {
                        LexCommentMultiLineToken(ref stringWalker);
                    }
                    else
                    {
                        var entryPositionIndex = stringWalker.PositionIndex;
				        stringWalker.ReadCharacter();
				        var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.None, stringWalker.ResourceUri, stringWalker.SourceText);
				        _syntaxTokenList.Add(new DivisionToken(textSpan));
                    }
                    break;
                case '+':
                    if (stringWalker.PeekCharacter(1) == '+')
                    {
                        LexPlusPlusToken(ref stringWalker);
                    }
                    else
                    {
                        var entryPositionIndex = stringWalker.PositionIndex;
				        stringWalker.ReadCharacter();
				        var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.None, stringWalker.ResourceUri, stringWalker.SourceText);
				        _syntaxTokenList.Add(new PlusToken(textSpan));
                    }
                    break;
                case '-':
                    if (stringWalker.PeekCharacter(1) == '-')
                    {
                        LexMinusMinusToken(ref stringWalker);
                    }
                    else
                    {
                        var entryPositionIndex = stringWalker.PositionIndex;
				        stringWalker.ReadCharacter();
				        var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.None, stringWalker.ResourceUri, stringWalker.SourceText);
				        _syntaxTokenList.Add(new MinusToken(textSpan));
                    }
                    break;
                case '=':
                    if (stringWalker.PeekCharacter(1) == '=')
                    {
                        LexEqualsEqualsToken(ref stringWalker);
                    }
                    else
                    {
                        var entryPositionIndex = stringWalker.PositionIndex;
				        stringWalker.ReadCharacter();
				        var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.None, stringWalker.ResourceUri, stringWalker.SourceText);
				        _syntaxTokenList.Add(new EqualsToken(textSpan));
                    }
                    break;
                case '?':
                    if (stringWalker.PeekCharacter(1) == '?')
                    {
                        LexQuestionMarkQuestionMarkToken(ref stringWalker);
                    }
                    else
                    {
                        var entryPositionIndex = stringWalker.PositionIndex;
				        stringWalker.ReadCharacter();
				        var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.None, stringWalker.ResourceUri, stringWalker.SourceText);
				        _syntaxTokenList.Add(new QuestionMarkToken(textSpan));
                    }
                    break;
                case '*':
                {
                	var entryPositionIndex = stringWalker.PositionIndex;
			        stringWalker.ReadCharacter();
			        var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.None, stringWalker.ResourceUri, stringWalker.SourceText);
			        _syntaxTokenList.Add(new StarToken(textSpan));
                    break;
                }
                case '!':
                {
                    var entryPositionIndex = stringWalker.PositionIndex;
			        stringWalker.ReadCharacter();
			        var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.None, stringWalker.ResourceUri, stringWalker.SourceText);
			        _syntaxTokenList.Add(new BangToken(textSpan));
                    break;
                }
                case ';':
                {
                    var entryPositionIndex = stringWalker.PositionIndex;
			        stringWalker.ReadCharacter();
			        var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.None, stringWalker.ResourceUri, stringWalker.SourceText);
			        _syntaxTokenList.Add(new StatementDelimiterToken(textSpan));
                    break;
                }
                case '(':
                {
                    var entryPositionIndex = stringWalker.PositionIndex;
			        stringWalker.ReadCharacter();
			        var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.None, stringWalker.ResourceUri, stringWalker.SourceText);
			        _syntaxTokenList.Add(new OpenParenthesisToken(textSpan));
                    break;
                }
                case ')':
                {
                    var entryPositionIndex = stringWalker.PositionIndex;
			        stringWalker.ReadCharacter();
			        var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.None, stringWalker.ResourceUri, stringWalker.SourceText);
			        _syntaxTokenList.Add(new CloseParenthesisToken(textSpan));
                    break;
                }
                case '{':
                {
                    var entryPositionIndex = stringWalker.PositionIndex;
			        stringWalker.ReadCharacter();
			        var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.None, stringWalker.ResourceUri, stringWalker.SourceText);
			        _syntaxTokenList.Add(new OpenBraceToken(textSpan));
                    break;
                }
                case '}':
                {
                    var entryPositionIndex = stringWalker.PositionIndex;
			        stringWalker.ReadCharacter();
			        var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.None, stringWalker.ResourceUri, stringWalker.SourceText);
			        _syntaxTokenList.Add(new CloseBraceToken(textSpan));
                    break;
                }
                case '<':
                {
                    var entryPositionIndex = stringWalker.PositionIndex;
			        stringWalker.ReadCharacter();
			        var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.None, stringWalker.ResourceUri, stringWalker.SourceText);
			        _syntaxTokenList.Add(new OpenAngleBracketToken(textSpan));
                    break;
                }
                case '>':
                {
                    var entryPositionIndex = stringWalker.PositionIndex;
			        stringWalker.ReadCharacter();
			        var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.None, stringWalker.ResourceUri, stringWalker.SourceText);
			        _syntaxTokenList.Add(new CloseAngleBracketToken(textSpan));
                    break;
                }
                case '[':
                {
                    var entryPositionIndex = stringWalker.PositionIndex;
			        stringWalker.ReadCharacter();
			        var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.None, stringWalker.ResourceUri, stringWalker.SourceText);
			        _syntaxTokenList.Add(new OpenSquareBracketToken(textSpan));
                    break;
                }
                case ']':
                {
                    var entryPositionIndex = stringWalker.PositionIndex;
			        stringWalker.ReadCharacter();
			        var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.None, stringWalker.ResourceUri, stringWalker.SourceText);
			        _syntaxTokenList.Add(new CloseSquareBracketToken(textSpan));
                    break;
                }
                case '$':
                	if (stringWalker.NextCharacter == '"')
                	{
                		LexString(ref stringWalker, countDollarSign: 1, useVerbatim: false);
					}
					else if (stringWalker.PeekCharacter(1) == '@' && stringWalker.PeekCharacter(2) == '"')
					{
						LexString(ref stringWalker, countDollarSign: 1, useVerbatim: true);
                	}
                	else if (stringWalker.NextCharacter == '$')
                	{
                		var entryPositionIndex = stringWalker.PositionIndex;
                		
                		// The while loop starts counting from and including the first dollar sign.
                		var countDollarSign = 0;
                	
                		while (!stringWalker.IsEof)
                		{
                			if (stringWalker.CurrentCharacter != '$')
                				break;
                			
            				++countDollarSign;
            				_ = stringWalker.ReadCharacter();
                		}
                		
                		// Only the last '$' (dollar sign character) will be syntax highlighted
                		// if this code is NOT included.
                		var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.StringLiteral, stringWalker.ResourceUri, stringWalker.SourceText);
				        _syntaxTokenList.Add(new StringLiteralToken(textSpan));
                		
                		// From the LexString(...) method:
                		// 	"awkwardly even if there are many of these it is expected
                		//      that the last one will not have been consumed."
                		_ = stringWalker.BacktrackCharacter();
                		
                		if (stringWalker.NextCharacter == '"')
	                		LexString(ref stringWalker, countDollarSign: countDollarSign, useVerbatim: false);
                	}
                	else
                	{
                    	LexDollarSignToken(ref stringWalker);
                    }
                    break;
                case '@':
                	if (stringWalker.NextCharacter == '"')
                		LexString(ref stringWalker, countDollarSign: 0, useVerbatim: true);
					else if (stringWalker.PeekCharacter(1) == '$' && stringWalker.PeekCharacter(2) == '"')
						LexString(ref stringWalker, countDollarSign: 1, useVerbatim: true);
                	else
                    	LexAtToken(ref stringWalker);
                    break;
                case ':':
                {
                    var entryPositionIndex = stringWalker.PositionIndex;
			        stringWalker.ReadCharacter();
			        var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.None, stringWalker.ResourceUri, stringWalker.SourceText);
			        _syntaxTokenList.Add(new ColonToken(textSpan));
                    break;
                }
                case '.':
                {
                    var entryPositionIndex = stringWalker.PositionIndex;
			        stringWalker.ReadCharacter();
			        var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.None, stringWalker.ResourceUri, stringWalker.SourceText);
			        _syntaxTokenList.Add(new MemberAccessToken(textSpan));
                    break;
                }
                case ',':
                {
                    var entryPositionIndex = stringWalker.PositionIndex;
			        stringWalker.ReadCharacter();
			        var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.None, stringWalker.ResourceUri, stringWalker.SourceText);
			        _syntaxTokenList.Add(new CommaToken(textSpan));
                    break;
                }
                case '#':
                    LexPreprocessorDirectiveToken(ref stringWalker);
                    break;
                default:
                    _ = stringWalker.ReadCharacter();
                    break;
            }
        }

        var endOfFileTextSpan = new TextEditorTextSpan(
            stringWalker.PositionIndex,
            stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            stringWalker.ResourceUri,
            stringWalker.SourceText);

        _syntaxTokenList.Add(new EndOfFileToken(endOfFileTextSpan));
        
        /*var totalTimeSpan = DateTime.UtcNow - startTime;
        
        TotalLexTimeSpan += totalTimeSpan;
        if (Hacky_Count > Hacky_Count_Before_Start_ConsoleWrite)
        {
        	Console.WriteLine($"TotalLexTimeSpan: {TotalLexTimeSpan.TotalSeconds:N3}");
        }
        
        if (totalTimeSpan > LongestLexTimeSpan)
        {
        	LongestLexTimeSpan = totalTimeSpan;
        	Console.WriteLine($"LongestLexTimeSpan: {LongestLexTimeSpan.TotalSeconds:N3}");
        }*/
    }
    
    /// <summary>
    /// The invoker of this method is expected to count the amount of '$' (dollar sign characters).
    /// When it comes to raw strings however, this logic will counted inside of this method.
    ///
    /// The reason being: you don't know if it is a string until you've read all of the '$' (dollar sign characters).
    /// So in order to invoke this method the invoker had to have counted them.
    /// </summary>
    private void LexString(ref StringWalkerStruct stringWalker, int countDollarSign, bool useVerbatim)
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
						EscapeCharacterListAdd(ref stringWalker, new TextEditorTextSpan(
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
					EscapeCharacterListAdd(ref stringWalker, new TextEditorTextSpan(
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
								LexInterpolatedExpression(ref stringWalker, countDollarSign);
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
						LexInterpolatedExpression(ref stringWalker, countDollarSign);
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
    
    private void LexInterpolatedExpression(ref StringWalkerStruct stringWalker, int countDollarSign)
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
    
    private void EscapeCharacterListAdd(ref StringWalkerStruct stringWalker, TextEditorTextSpan textSpan)
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
    
    public void LexIdentifierOrKeywordOrKeywordContextual(ref StringWalkerStruct stringWalker)
    {
        var entryPositionIndex = stringWalker.PositionIndex;

        while (!stringWalker.IsEof)
        {
            if (!char.IsLetterOrDigit(stringWalker.CurrentCharacter) &&
                stringWalker.CurrentCharacter != '_')
            {
                break;
            }

            _ = stringWalker.ReadCharacter();
        }

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            stringWalker.ResourceUri,
            stringWalker.SourceText);

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
    
    public void LexNumericLiteralToken(ref StringWalkerStruct stringWalker)
    {
        var entryPositionIndex = stringWalker.PositionIndex;

        // Declare outside the while loop to avoid overhead of redeclaring each loop? not sure
        var isNotANumber = false;

        while (!stringWalker.IsEof)
        {
            switch (stringWalker.CurrentCharacter)
            {
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
                    break;
                default:
                    isNotANumber = true;
                    break;
            }

            if (isNotANumber)
                break;

            _ = stringWalker.ReadCharacter();
        }

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            stringWalker.ResourceUri,
            stringWalker.SourceText);

        _syntaxTokenList.Add(new NumericLiteralToken(textSpan));
    }
    
    public void LexCharLiteralToken(ref StringWalkerStruct stringWalker)
    {
    	var delimiter = '\'';
    	var escapeCharacter = '\\';
    	
        var entryPositionIndex = stringWalker.PositionIndex;

        // Move past the opening delimiter
        _ = stringWalker.ReadCharacter();

        while (!stringWalker.IsEof)
        {
			if (stringWalker.CurrentCharacter == delimiter)
			{
				_ = stringWalker.ReadCharacter();
				break;
			}
			else if (stringWalker.CurrentCharacter == escapeCharacter)
			{
				if (EscapeCharacterList is not null)
				{
					EscapeCharacterList.Add(new TextEditorTextSpan(
			            stringWalker.PositionIndex,
			            stringWalker.PositionIndex + 2,
			            (byte)GenericDecorationKind.EscapeCharacterPrimary,
			            stringWalker.ResourceUri,
			            stringWalker.SourceText));
				}

				// Presuming the escaped text is 2 characters,
				// then read an extra character.
				_ = stringWalker.ReadCharacter();
			}

            _ = stringWalker.ReadCharacter();
        }

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            stringWalker.PositionIndex,
            (byte)GenericDecorationKind.StringLiteral,
            stringWalker.ResourceUri,
            stringWalker.SourceText);

        _syntaxTokenList.Add(new CharLiteralToken(textSpan));
    }
    
    public void LexCommentSingleLineToken(ref StringWalkerStruct stringWalker)
    {
        var entryPositionIndex = stringWalker.PositionIndex;

        // Declare outside the while loop to avoid overhead of redeclaring each loop? not sure
        var isNewLineCharacter = false;

        while (!stringWalker.IsEof)
        {
            switch (stringWalker.CurrentCharacter)
            {
                case '\r':
                case '\n':
                    isNewLineCharacter = true;
                    break;
                default:
                    break;
            }

            if (isNewLineCharacter)
                break;

            _ = stringWalker.ReadCharacter();
        }

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            stringWalker.PositionIndex,
            (byte)GenericDecorationKind.CommentSingleLine,
            stringWalker.ResourceUri,
            stringWalker.SourceText);

        _syntaxTokenList.Add(new CommentSingleLineToken(textSpan));
    }
    
    public void LexCommentMultiLineToken(ref StringWalkerStruct stringWalker)
    {
        var entryPositionIndex = stringWalker.PositionIndex;

        // Move past the initial "/*"
        _ = stringWalker.ReadRange(2);

        // Declare outside the while loop to avoid overhead of redeclaring each loop? not sure
        var possibleClosingText = false;
        var sawClosingText = false;

        while (!stringWalker.IsEof)
        {
            switch (stringWalker.CurrentCharacter)
            {
                case '*':
                    possibleClosingText = true;
                    break;
                case '/':
                    if (possibleClosingText)
                        sawClosingText = true;
                    break;
                default:
                    break;
            }

            _ = stringWalker.ReadCharacter();

            if (sawClosingText)
                break;
        }

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            stringWalker.PositionIndex,
            (byte)GenericDecorationKind.CommentMultiLine,
            stringWalker.ResourceUri,
            stringWalker.SourceText);

        _syntaxTokenList.Add(new CommentMultiLineToken(textSpan));
    }
    
    public void LexPlusPlusToken(ref StringWalkerStruct stringWalker)
    {
        var entryPositionIndex = stringWalker.PositionIndex;

        // First '+'
        stringWalker.ReadCharacter();
        // Second '+'
        stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            stringWalker.ResourceUri,
            stringWalker.SourceText);

        _syntaxTokenList.Add(new PlusPlusToken(textSpan));
    }
    
    public void LexMinusMinusToken(ref StringWalkerStruct stringWalker)
    {
        var entryPositionIndex = stringWalker.PositionIndex;

        // First '-'
        stringWalker.ReadCharacter();
        // Second '-'
        stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            stringWalker.ResourceUri,
            stringWalker.SourceText);

        _syntaxTokenList.Add(new MinusMinusToken(textSpan));
    }
    
    public void LexEqualsEqualsToken(ref StringWalkerStruct stringWalker)
    {
        var entryPositionIndex = stringWalker.PositionIndex;

        // First '='
        stringWalker.ReadCharacter();
        // Second '='
        stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            stringWalker.ResourceUri,
            stringWalker.SourceText);

        _syntaxTokenList.Add(new EqualsEqualsToken(textSpan));
    }
    
    public void LexQuestionMarkQuestionMarkToken(ref StringWalkerStruct stringWalker)
    {
        var entryPositionIndex = stringWalker.PositionIndex;

        // First '?'
        stringWalker.ReadCharacter();
        // Second '?'
        stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            stringWalker.ResourceUri,
            stringWalker.SourceText);

        _syntaxTokenList.Add(new QuestionMarkQuestionMarkToken(textSpan));
    }
    
    public void LexDollarSignToken(ref StringWalkerStruct stringWalker)
    {
        var entryPositionIndex = stringWalker.PositionIndex;

        stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            stringWalker.ResourceUri,
            stringWalker.SourceText);

        _syntaxTokenList.Add(new DollarSignToken(textSpan));
    }
    
    public void LexAtToken(ref StringWalkerStruct stringWalker)
    {
        var entryPositionIndex = stringWalker.PositionIndex;

        stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            stringWalker.ResourceUri,
            stringWalker.SourceText);

        _syntaxTokenList.Add(new AtToken(textSpan));
    }
    
    public void LexPreprocessorDirectiveToken(ref StringWalkerStruct stringWalker)
    {
        var entryPositionIndex = stringWalker.PositionIndex;

        // Declare outside the while loop to avoid overhead of redeclaring each loop? not sure
        var isNewLineCharacter = false;

        while (!stringWalker.IsEof)
        {
            switch (stringWalker.CurrentCharacter)
            {
                case '\r':
                case '\n':
                    isNewLineCharacter = true;
                    break;
                default:
                    break;
            }

            if (isNewLineCharacter)
                break;

            _ = stringWalker.ReadCharacter();
        }

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            stringWalker.ResourceUri,
            stringWalker.SourceText);

        _syntaxTokenList.Add(new PreprocessorDirectiveToken(textSpan));
    }
}