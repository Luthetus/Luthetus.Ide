using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.Extensions.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax;
using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Enums;

namespace Luthetus.CompilerServices.CSharp.LexerCase;

public static class CSharpLexer
{
	/// <summary>
	/// Initialize the CSharpLexerOutput here, then start the while loop with 'Lex_Frame(...)'.
	/// </summary>
    public static CSharpLexerOutput Lex(ResourceUri resourceUri, string sourceText)
    {
    	var lexerOutput = new CSharpLexerOutput();
    	var stringWalker = new StringWalkerStruct(resourceUri, sourceText);
    	
    	var previousEscapeCharacterTextSpan = new TextEditorTextSpan(
    		0,
		    0,
		    (byte)GenericDecorationKind.None,
		    ResourceUri.Empty,
		    string.Empty,
		    string.Empty);
		    
		var interpolatedExpressionUnmatchedBraceCount = -1;
    	
    	Lex_Frame(ref lexerOutput, ref stringWalker, ref previousEscapeCharacterTextSpan, ref interpolatedExpressionUnmatchedBraceCount);
    	
    	var endOfFileTextSpan = new TextEditorTextSpan(
            stringWalker.PositionIndex,
            stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            stringWalker.ResourceUri,
            stringWalker.SourceText);

        lexerOutput.SyntaxTokenList.Add(new SyntaxToken(SyntaxKind.EndOfFileToken, endOfFileTextSpan));
        return lexerOutput;
    }
    
    /// <summary>
    /// Isolate the while loop within its own function in order to permit recursion without allocating new state.
    /// </summary>
    public static void Lex_Frame(
    	ref CSharpLexerOutput lexerOutput,
    	ref StringWalkerStruct stringWalker,
    	ref TextEditorTextSpan previousEscapeCharacterTextSpan,
    	ref int interpolatedExpressionUnmatchedBraceCount)
    {
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
                    LexIdentifierOrKeywordOrKeywordContextual(ref lexerOutput, ref stringWalker);
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
                    LexNumericLiteralToken(ref lexerOutput, ref stringWalker);
                    break;
				case '\'':
                    LexCharLiteralToken(ref lexerOutput, ref stringWalker);
                    break;
                case '"':
                	LexString(ref lexerOutput, ref stringWalker, ref previousEscapeCharacterTextSpan, countDollarSign: 0, useVerbatim: false);
                    break;
                case '/':
                    if (stringWalker.PeekCharacter(1) == '/')
                    {
                        LexCommentSingleLineToken(ref lexerOutput, ref stringWalker);
                    }
                    else if (stringWalker.PeekCharacter(1) == '*')
                    {
                        LexCommentMultiLineToken(ref lexerOutput, ref stringWalker);
                    }
                    else
                    {
                        var entryPositionIndex = stringWalker.PositionIndex;
				        stringWalker.ReadCharacter();
				        var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.None, stringWalker.ResourceUri, stringWalker.SourceText);
				        lexerOutput.SyntaxTokenList.Add(new SyntaxToken(SyntaxKind.DivisionToken, textSpan));
                    }
                    break;
                case '+':
                    if (stringWalker.PeekCharacter(1) == '+')
                    {
                        var entryPositionIndex = stringWalker.PositionIndex;
				        stringWalker.ReadCharacter();
				        stringWalker.ReadCharacter();
				        var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.None, stringWalker.ResourceUri, stringWalker.SourceText);
				        lexerOutput.SyntaxTokenList.Add(new SyntaxToken(SyntaxKind.PlusPlusToken, textSpan));
                    }
                    else
                    {
                        var entryPositionIndex = stringWalker.PositionIndex;
				        stringWalker.ReadCharacter();
				        var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.None, stringWalker.ResourceUri, stringWalker.SourceText);
				        lexerOutput.SyntaxTokenList.Add(new SyntaxToken(SyntaxKind.PlusToken, textSpan));
                    }
                    break;
                case '-':
                    if (stringWalker.PeekCharacter(1) == '-')
                    {
                        var entryPositionIndex = stringWalker.PositionIndex;
				        stringWalker.ReadCharacter();
				        stringWalker.ReadCharacter();
				        var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.None, stringWalker.ResourceUri, stringWalker.SourceText);
				        lexerOutput.SyntaxTokenList.Add(new SyntaxToken(SyntaxKind.MinusMinusToken, textSpan));
                    }
                    else
                    {
                        var entryPositionIndex = stringWalker.PositionIndex;
				        stringWalker.ReadCharacter();
				        var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.None, stringWalker.ResourceUri, stringWalker.SourceText);
				        lexerOutput.SyntaxTokenList.Add(new SyntaxToken(SyntaxKind.MinusToken, textSpan));
                    }
                    break;
                case '=':
                    if (stringWalker.PeekCharacter(1) == '=')
                    {
                        var entryPositionIndex = stringWalker.PositionIndex;
				        stringWalker.ReadCharacter();
				        stringWalker.ReadCharacter();
				        var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.None, stringWalker.ResourceUri, stringWalker.SourceText);
				        lexerOutput.SyntaxTokenList.Add(new SyntaxToken(SyntaxKind.EqualsEqualsToken, textSpan));
                    }
                    else if (stringWalker.PeekCharacter(1) == '>')
                	{
                		var entryPositionIndex = stringWalker.PositionIndex;
				        stringWalker.ReadCharacter();
				        stringWalker.ReadCharacter();
				        var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.None, stringWalker.ResourceUri, stringWalker.SourceText);
				        lexerOutput.SyntaxTokenList.Add(new SyntaxToken(SyntaxKind.EqualsCloseAngleBracketToken, textSpan));
                	}
                    else
                    {
                        var entryPositionIndex = stringWalker.PositionIndex;
				        stringWalker.ReadCharacter();
				        var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.None, stringWalker.ResourceUri, stringWalker.SourceText);
				        lexerOutput.SyntaxTokenList.Add(new SyntaxToken(SyntaxKind.EqualsToken, textSpan));
                    }
                    break;
                case '?':
                    if (stringWalker.PeekCharacter(1) == '?')
                    {
                        var entryPositionIndex = stringWalker.PositionIndex;
				        stringWalker.ReadCharacter();
				        stringWalker.ReadCharacter();
				        var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.None, stringWalker.ResourceUri, stringWalker.SourceText);
				        lexerOutput.SyntaxTokenList.Add(new SyntaxToken(SyntaxKind.QuestionMarkQuestionMarkToken, textSpan));
                    }
                    else
                    {
                        var entryPositionIndex = stringWalker.PositionIndex;
				        stringWalker.ReadCharacter();
				        var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.None, stringWalker.ResourceUri, stringWalker.SourceText);
				        lexerOutput.SyntaxTokenList.Add(new SyntaxToken(SyntaxKind.QuestionMarkToken, textSpan));
                    }
                    break;
                case '|':
                    if (stringWalker.PeekCharacter(1) == '|')
                    {
                        var entryPositionIndex = stringWalker.PositionIndex;
				        stringWalker.ReadCharacter();
				        stringWalker.ReadCharacter();
				        var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.None, stringWalker.ResourceUri, stringWalker.SourceText);
				        lexerOutput.SyntaxTokenList.Add(new SyntaxToken(SyntaxKind.PipePipeToken, textSpan));
                    }
                    else
                    {
                        var entryPositionIndex = stringWalker.PositionIndex;
				        stringWalker.ReadCharacter();
				        var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.None, stringWalker.ResourceUri, stringWalker.SourceText);
				        lexerOutput.SyntaxTokenList.Add(new SyntaxToken(SyntaxKind.PipeToken, textSpan));
                    }
                    break;
                case '&':
                    if (stringWalker.PeekCharacter(1) == '&')
                    {
                        var entryPositionIndex = stringWalker.PositionIndex;
				        stringWalker.ReadCharacter();
				        stringWalker.ReadCharacter();
				        var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.None, stringWalker.ResourceUri, stringWalker.SourceText);
				        lexerOutput.SyntaxTokenList.Add(new SyntaxToken(SyntaxKind.AmpersandAmpersandToken, textSpan));
                    }
                    else
                    {
                        var entryPositionIndex = stringWalker.PositionIndex;
				        stringWalker.ReadCharacter();
				        var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.None, stringWalker.ResourceUri, stringWalker.SourceText);
				        lexerOutput.SyntaxTokenList.Add(new SyntaxToken(SyntaxKind.AmpersandToken, textSpan));
                    }
                    break;
                case '*':
                {
                	var entryPositionIndex = stringWalker.PositionIndex;
			        stringWalker.ReadCharacter();
			        var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.None, stringWalker.ResourceUri, stringWalker.SourceText);
			        lexerOutput.SyntaxTokenList.Add(new SyntaxToken(SyntaxKind.StarToken, textSpan));
                    break;
                }
                case '!':
                {
                	if (stringWalker.PeekCharacter(1) == '=')
                    {
                        var entryPositionIndex = stringWalker.PositionIndex;
				        stringWalker.ReadCharacter();
				        stringWalker.ReadCharacter();
				        var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.None, stringWalker.ResourceUri, stringWalker.SourceText);
				        lexerOutput.SyntaxTokenList.Add(new SyntaxToken(SyntaxKind.BangEqualsToken, textSpan));
                    }
                    else
                    {
                        var entryPositionIndex = stringWalker.PositionIndex;
				        stringWalker.ReadCharacter();
				        var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.None, stringWalker.ResourceUri, stringWalker.SourceText);
				        lexerOutput.SyntaxTokenList.Add(new SyntaxToken(SyntaxKind.BangToken, textSpan));
                    }
                    break;
                }
                case ';':
                {
                    var entryPositionIndex = stringWalker.PositionIndex;
			        stringWalker.ReadCharacter();
			        var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.None, stringWalker.ResourceUri, stringWalker.SourceText);
			        lexerOutput.SyntaxTokenList.Add(new SyntaxToken(SyntaxKind.StatementDelimiterToken, textSpan));
                    break;
                }
                case '(':
                {
                    var entryPositionIndex = stringWalker.PositionIndex;
			        stringWalker.ReadCharacter();
			        var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.None, stringWalker.ResourceUri, stringWalker.SourceText);
			        lexerOutput.SyntaxTokenList.Add(new SyntaxToken(SyntaxKind.OpenParenthesisToken, textSpan));
                    break;
                }
                case ')':
                {
                    var entryPositionIndex = stringWalker.PositionIndex;
			        stringWalker.ReadCharacter();
			        var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.None, stringWalker.ResourceUri, stringWalker.SourceText);
			        lexerOutput.SyntaxTokenList.Add(new SyntaxToken(SyntaxKind.CloseParenthesisToken, textSpan));
                    break;
                }
                case '{':
                {
                	if (interpolatedExpressionUnmatchedBraceCount != -1)
                		++interpolatedExpressionUnmatchedBraceCount;
                
                    var entryPositionIndex = stringWalker.PositionIndex;
			        stringWalker.ReadCharacter();
			        var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.None, stringWalker.ResourceUri, stringWalker.SourceText);
			        lexerOutput.SyntaxTokenList.Add(new SyntaxToken(SyntaxKind.OpenBraceToken, textSpan));
                    break;
                }
                case '}':
                {
                	if (interpolatedExpressionUnmatchedBraceCount != -1)
                	{
						if (--interpolatedExpressionUnmatchedBraceCount <= 0)
							goto forceExit;
					}
                
                    var entryPositionIndex = stringWalker.PositionIndex;
			        stringWalker.ReadCharacter();
			        var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.None, stringWalker.ResourceUri, stringWalker.SourceText);
			        lexerOutput.SyntaxTokenList.Add(new SyntaxToken(SyntaxKind.CloseBraceToken, textSpan));
                    break;
                }
                case '<':
                {
                	if (stringWalker.PeekCharacter(1) == '=')
                    {
                        var entryPositionIndex = stringWalker.PositionIndex;
				        stringWalker.ReadCharacter();
				        stringWalker.ReadCharacter();
				        var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.None, stringWalker.ResourceUri, stringWalker.SourceText);
				        lexerOutput.SyntaxTokenList.Add(new SyntaxToken(SyntaxKind.OpenAngleBracketEqualsToken, textSpan));
                    }
                    else
                    {
                        var entryPositionIndex = stringWalker.PositionIndex;
				        stringWalker.ReadCharacter();
				        var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.None, stringWalker.ResourceUri, stringWalker.SourceText);
				        lexerOutput.SyntaxTokenList.Add(new SyntaxToken(SyntaxKind.OpenAngleBracketToken, textSpan));
                    }
                    break;
                }
                case '>':
                {
                	if (stringWalker.PeekCharacter(1) == '=')
                    {
                        var entryPositionIndex = stringWalker.PositionIndex;
				        stringWalker.ReadCharacter();
				        stringWalker.ReadCharacter();
				        var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.None, stringWalker.ResourceUri, stringWalker.SourceText);
				        lexerOutput.SyntaxTokenList.Add(new SyntaxToken(SyntaxKind.CloseAngleBracketEqualsToken, textSpan));
                    }
                    else
                    {
                        var entryPositionIndex = stringWalker.PositionIndex;
				        stringWalker.ReadCharacter();
				        var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.None, stringWalker.ResourceUri, stringWalker.SourceText);
				        lexerOutput.SyntaxTokenList.Add(new SyntaxToken(SyntaxKind.CloseAngleBracketToken, textSpan));
                    }
                    break;
                }
                case '[':
                {
                    var entryPositionIndex = stringWalker.PositionIndex;
			        stringWalker.ReadCharacter();
			        var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.None, stringWalker.ResourceUri, stringWalker.SourceText);
			        lexerOutput.SyntaxTokenList.Add(new SyntaxToken(SyntaxKind.OpenSquareBracketToken, textSpan));
                    break;
                }
                case ']':
                {
                    var entryPositionIndex = stringWalker.PositionIndex;
			        stringWalker.ReadCharacter();
			        var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.None, stringWalker.ResourceUri, stringWalker.SourceText);
			        lexerOutput.SyntaxTokenList.Add(new SyntaxToken(SyntaxKind.CloseSquareBracketToken, textSpan));
                    break;
                }
                case '$':
                	if (stringWalker.NextCharacter == '"')
                	{
                		LexString(ref lexerOutput, ref stringWalker, ref previousEscapeCharacterTextSpan, countDollarSign: 1, useVerbatim: false);
					}
					else if (stringWalker.PeekCharacter(1) == '@' && stringWalker.PeekCharacter(2) == '"')
					{
						LexString(ref lexerOutput, ref stringWalker, ref previousEscapeCharacterTextSpan, countDollarSign: 1, useVerbatim: true);
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
				        lexerOutput.SyntaxTokenList.Add(new SyntaxToken(SyntaxKind.StringLiteralToken, textSpan));
                		
                		// From the LexString(...) method:
                		// 	"awkwardly even if there are many of these it is expected
                		//      that the last one will not have been consumed."
                		_ = stringWalker.BacktrackCharacter();
                		
                		if (stringWalker.NextCharacter == '"')
	                		LexString(ref lexerOutput, ref stringWalker, ref previousEscapeCharacterTextSpan, countDollarSign: countDollarSign, useVerbatim: false);
                	}
                	else
                	{
                    	var entryPositionIndex = stringWalker.PositionIndex;
				        stringWalker.ReadCharacter();
				        var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.None, stringWalker.ResourceUri, stringWalker.SourceText);
				        lexerOutput.SyntaxTokenList.Add(new SyntaxToken(SyntaxKind.DollarSignToken, textSpan));
	                    break;
                    }
                    break;
                case '@':
                	if (stringWalker.NextCharacter == '"')
                	{
                		LexString(ref lexerOutput, ref stringWalker, ref previousEscapeCharacterTextSpan, countDollarSign: 0, useVerbatim: true);
					}
					else if (stringWalker.PeekCharacter(1) == '$' && stringWalker.PeekCharacter(2) == '"')
					{
						LexString(ref lexerOutput, ref stringWalker, ref previousEscapeCharacterTextSpan, countDollarSign: 1, useVerbatim: true);
      			  }
                	else
                	{
                    	var entryPositionIndex = stringWalker.PositionIndex;
				        stringWalker.ReadCharacter();
				        var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.None, stringWalker.ResourceUri, stringWalker.SourceText);
				        lexerOutput.SyntaxTokenList.Add(new SyntaxToken(SyntaxKind.AtToken, textSpan));
	                    break;
                    }
                    break;
                case ':':
                {
                    var entryPositionIndex = stringWalker.PositionIndex;
			        stringWalker.ReadCharacter();
			        var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.None, stringWalker.ResourceUri, stringWalker.SourceText);
			        lexerOutput.SyntaxTokenList.Add(new SyntaxToken(SyntaxKind.ColonToken, textSpan));
                    break;
                }
                case '.':
                {
                    var entryPositionIndex = stringWalker.PositionIndex;
			        stringWalker.ReadCharacter();
			        var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.None, stringWalker.ResourceUri, stringWalker.SourceText);
			        lexerOutput.SyntaxTokenList.Add(new SyntaxToken(SyntaxKind.MemberAccessToken, textSpan));
                    break;
                }
                case ',':
                {
                    var entryPositionIndex = stringWalker.PositionIndex;
			        stringWalker.ReadCharacter();
			        var textSpan = new TextEditorTextSpan(entryPositionIndex, stringWalker.PositionIndex, (byte)GenericDecorationKind.None, stringWalker.ResourceUri, stringWalker.SourceText);
			        lexerOutput.SyntaxTokenList.Add(new SyntaxToken(SyntaxKind.CommaToken, textSpan));
                    break;
                }
                case '#':
                    LexPreprocessorDirectiveToken(ref lexerOutput, ref stringWalker);
                    break;
                default:
                    _ = stringWalker.ReadCharacter();
                    break;
            }
		}

        forceExit:
        return;
	}
    
    /// <summary>
    /// The invoker of this method is expected to count the amount of '$' (dollar sign characters).
    /// When it comes to raw strings however, this logic will counted inside of this method.
    ///
    /// The reason being: you don't know if it is a string until you've read all of the '$' (dollar sign characters).
    /// So in order to invoke this method the invoker had to have counted them.
    /// </summary>
    private static void LexString(ref CSharpLexerOutput lexerOutput, ref StringWalkerStruct stringWalker, ref TextEditorTextSpan previousEscapeCharacterTextSpan, int countDollarSign, bool useVerbatim)
    {
    	// Interpolated expressions will be done recursively and added to this 'SyntaxTokenList'
    	var syntaxTokenListIndex = lexerOutput.SyntaxTokenList.Count;
    
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
					EscapeCharacterListAdd(ref lexerOutput, ref stringWalker, ref previousEscapeCharacterTextSpan, new TextEditorTextSpan(
				    	stringWalker.PositionIndex,
				        stringWalker.PositionIndex + 2,
				        (byte)GenericDecorationKind.EscapeCharacterPrimary,
				        stringWalker.ResourceUri,
				        stringWalker.SourceText));
	
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
				EscapeCharacterListAdd(ref lexerOutput, ref stringWalker, ref previousEscapeCharacterTextSpan, new TextEditorTextSpan(
		            stringWalker.PositionIndex,
		            stringWalker.PositionIndex + 2,
		            (byte)GenericDecorationKind.EscapeCharacterPrimary,
		            stringWalker.ResourceUri,
		            stringWalker.SourceText));

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
		    					// 'LexInterpolatedExpression' is expected to consume one more after it is finished.
								// Thus, if this while loop were to consume, it would skip the
								// closing double quotes if the expression were the last thing in the string.
								//
								// So, a backtrack is done.
								LexInterpolatedExpression(ref lexerOutput, ref stringWalker, ref previousEscapeCharacterTextSpan, startInclusiveOpenDelimiter: interpolationTemporaryPositionIndex, countDollarSign: countDollarSign);
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
						// 'LexInterpolatedExpression' is expected to consume one more after it is finished.
						// Thus, if this while loop were to consume, it would skip the
						// closing double quotes if the expression were the last thing in the string.
						LexInterpolatedExpression(ref lexerOutput, ref stringWalker, ref previousEscapeCharacterTextSpan, startInclusiveOpenDelimiter: stringWalker.PositionIndex, countDollarSign: countDollarSign);
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

		if (useInterpolation)
		{
			lexerOutput.SyntaxTokenList.Insert(
				syntaxTokenListIndex,
				new SyntaxToken(SyntaxKind.StringInterpolatedStartToken, textSpan));
				
			lexerOutput.SyntaxTokenList.Add(new SyntaxToken(
				SyntaxKind.StringInterpolatedEndToken,
				new TextEditorTextSpan(
		            stringWalker.PositionIndex,
				    stringWalker.PositionIndex,
				    (byte)GenericDecorationKind.None,
				    stringWalker.ResourceUri,
            		stringWalker.SourceText,
				    string.Empty)));
		}
		else
		{
        	lexerOutput.SyntaxTokenList.Add(new SyntaxToken(SyntaxKind.StringLiteralToken, textSpan));
        }
    }
    
    /// <summary>
    /// 'startInclusiveFirstOpenDelimiter' refers to:
    ///     $"Hello, {name}"
    ///
    /// how there is a '{' that deliminates the start of the interpolated expression.
    /// at what position index does it lie at?
    ///
    /// In the case of raw strings, the start-inclusive of the multi-character open delimiter is what needs to be provided.
    /// </summary>
    private static void LexInterpolatedExpression(
    	ref CSharpLexerOutput lexerOutput, ref StringWalkerStruct stringWalker, ref TextEditorTextSpan previousEscapeCharacterTextSpan, int startInclusiveOpenDelimiter, int countDollarSign)
    {
    	var readOpenDelimiterCount = stringWalker.PositionIndex - startInclusiveOpenDelimiter;
    	
    	for (; readOpenDelimiterCount < countDollarSign; readOpenDelimiterCount++)
    	{
    		_ = stringWalker.ReadCharacter();
    	}
		
		var unmatchedBraceCounter = countDollarSign;
        
        // Recursive solution that lexes the interpolated expression only, (not including the '{' or '}').
        Lex_Frame(
        	ref lexerOutput,
        	ref stringWalker,
        	ref previousEscapeCharacterTextSpan,
        	ref unmatchedBraceCounter);
        	
        _ = stringWalker.ReadCharacter(); // This consumes the final '}'.

		// In the event that the C# Parser throws an exception,
		// it is useful for the Lexer to decorate the interpolated expressions
		// with the text color '(byte)GenericDecorationKind.None'
		// so they are distinct from the string itself.
		lexerOutput.MiscTextSpanList.Add(new TextEditorTextSpan(
			startInclusiveOpenDelimiter,
			stringWalker.PositionIndex,
			(byte)GenericDecorationKind.None,
			stringWalker.ResourceUri,
			stringWalker.SourceText));
		
		lexerOutput.SyntaxTokenList.Add(new SyntaxToken(
			SyntaxKind.StringInterpolatedContinueToken,
			new TextEditorTextSpan(
	            stringWalker.PositionIndex,
			    stringWalker.PositionIndex,
			    (byte)GenericDecorationKind.None,
			    stringWalker.ResourceUri,
        		stringWalker.SourceText,
			    string.Empty)));
	}
    
    private static void EscapeCharacterListAdd(
    	ref CSharpLexerOutput lexerOutput, ref StringWalkerStruct stringWalker, ref TextEditorTextSpan previousEscapeCharacterTextSpan, TextEditorTextSpan textSpan)
    {
    	if (lexerOutput.MiscTextSpanList.Count > 0)
    	{
    		if (previousEscapeCharacterTextSpan.EndExclusiveIndex == textSpan.StartInclusiveIndex &&
    			previousEscapeCharacterTextSpan.DecorationByte == (byte)GenericDecorationKind.EscapeCharacterPrimary)
    		{
    			textSpan = textSpan with
    			{
    				DecorationByte = (byte)GenericDecorationKind.EscapeCharacterSecondary,
    			};
    		}
    	}
    	
    	previousEscapeCharacterTextSpan = textSpan;
    	lexerOutput.MiscTextSpanList.Add(textSpan);
    }
    
    public static void LexIdentifierOrKeywordOrKeywordContextual(ref CSharpLexerOutput lexerOutput, ref StringWalkerStruct stringWalker)
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
                lexerOutput.SyntaxTokenList.Add(new SyntaxToken(LexerUtils.GetSyntaxKindForContextualKeyword(textSpan), textSpan));
                return;
            }

            lexerOutput.SyntaxTokenList.Add(new SyntaxToken(LexerUtils.GetSyntaxKindForKeyword(textSpan), textSpan));
            return;
        }

        lexerOutput.SyntaxTokenList.Add(new SyntaxToken(SyntaxKind.IdentifierToken, textSpan));
    }
    
    public static void LexNumericLiteralToken(ref CSharpLexerOutput lexerOutput, ref StringWalkerStruct stringWalker)
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

        lexerOutput.SyntaxTokenList.Add(new SyntaxToken(SyntaxKind.NumericLiteralToken, textSpan));
    }
    
    public static void LexCharLiteralToken(ref CSharpLexerOutput lexerOutput, ref StringWalkerStruct stringWalker)
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
				lexerOutput.MiscTextSpanList.Add(new TextEditorTextSpan(
		            stringWalker.PositionIndex,
		            stringWalker.PositionIndex + 2,
		            (byte)GenericDecorationKind.EscapeCharacterPrimary,
		            stringWalker.ResourceUri,
		            stringWalker.SourceText));

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

        lexerOutput.SyntaxTokenList.Add(new SyntaxToken(SyntaxKind.CharLiteralToken, textSpan));
    }
    
    public static void LexCommentSingleLineToken(ref CSharpLexerOutput lexerOutput, ref StringWalkerStruct stringWalker)
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

        lexerOutput.MiscTextSpanList.Add(textSpan);
    }
    
    public static void LexCommentMultiLineToken(ref CSharpLexerOutput lexerOutput, ref StringWalkerStruct stringWalker)
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

        lexerOutput.MiscTextSpanList.Add(textSpan);
    }
    
    public static void LexPreprocessorDirectiveToken(ref CSharpLexerOutput lexerOutput, ref StringWalkerStruct stringWalker)
    {
        var entryPositionIndex = stringWalker.PositionIndex;

        // Declare outside the while loop to avoid overhead of redeclaring each loop? not sure
        var isNewLineCharacter = false;
        var firstWhitespaceCharacterPositionIndex = -1;

        while (!stringWalker.IsEof)
        {
            switch (stringWalker.CurrentCharacter)
            {
                case '\r':
                case '\n':
                    isNewLineCharacter = true;
                    break;
                case '\t':
                case ' ':
                	if (firstWhitespaceCharacterPositionIndex == -1)
                		firstWhitespaceCharacterPositionIndex = stringWalker.PositionIndex;
                	break;
                default:
                    break;
            }

            if (isNewLineCharacter)
                break;

            _ = stringWalker.ReadCharacter();
        }
        
        if (firstWhitespaceCharacterPositionIndex == -1)
        	firstWhitespaceCharacterPositionIndex = stringWalker.PositionIndex;

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            firstWhitespaceCharacterPositionIndex,
            (byte)GenericDecorationKind.PreprocessorDirective,
            stringWalker.ResourceUri,
            stringWalker.SourceText);

        lexerOutput.SyntaxTokenList.Add(new SyntaxToken(SyntaxKind.PreprocessorDirectiveToken, textSpan));
    }
}