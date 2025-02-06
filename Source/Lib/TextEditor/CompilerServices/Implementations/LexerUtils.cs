using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.CompilerServices.Utility;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.CompilerServices.Implementations;

/// <summary>
/// TODO: Rewrite this class such that the lexed token is returned,...
///       ...rather than added to the list of 'syntaxTokens'.
///       |
///       It is awkward when one wants to use the 'Lex...(...)' method, yet
///       afterwards needs to replace the last entry of 'syntaxTokens'
///       with something of a different class, or syntax kind.
/// </summary>
public static class LexerUtils
{
    public static void LexNumericLiteralToken(
        StringWalker stringWalker, List<SyntaxToken> syntaxTokens)
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

        syntaxTokens.Add(new SyntaxToken(SyntaxKind.NumericLiteralToken, textSpan));
    }

	public static void LexCharLiteralToken(
        StringWalker stringWalker, List<SyntaxToken> syntaxTokens, List<TextEditorTextSpan>? escapeCharacterList = null, char delimiter = '\'', char escapeCharacter = '\\')
    {
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
				if (escapeCharacterList is not null)
				{
					escapeCharacterList.Add(new TextEditorTextSpan(
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

        syntaxTokens.Add(new SyntaxToken(SyntaxKind.CharLiteralToken, textSpan));
    }

    public static void LexStringLiteralToken(
        StringWalker stringWalker, List<SyntaxToken> syntaxTokens, List<TextEditorTextSpan>? escapeCharacterList = null, char delimiter = '"', char escapeCharacter = '\\')
    {
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
				if (escapeCharacterList is not null)
				{
					escapeCharacterList.Add(new TextEditorTextSpan(
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

        syntaxTokens.Add(new SyntaxToken(SyntaxKind.StringLiteralToken, textSpan));
    }

    public static void LexCommentSingleLineToken(
        StringWalker stringWalker, List<SyntaxToken> syntaxTokens)
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

        syntaxTokens.Add(new SyntaxToken(SyntaxKind.CommentSingleLineToken, textSpan));
    }

    public static void LexCommentMultiLineToken(
        StringWalker stringWalker, List<SyntaxToken> syntaxTokens)
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

        syntaxTokens.Add(new SyntaxToken(SyntaxKind.CommentMultiLineToken, textSpan));
    }

    public static void LexIdentifierOrKeywordOrKeywordContextual(
        StringWalker stringWalker, List<SyntaxToken> syntaxTokens, LexerKeywords lexerKeywords)
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

        if (lexerKeywords.AllKeywords.Contains(textValue))
        {
            var decorationByte = (byte)GenericDecorationKind.Keyword;

            if (lexerKeywords.ControlKeywords.Contains(textValue))
                decorationByte = (byte)GenericDecorationKind.KeywordControl;

            textSpan = textSpan with
            {
                DecorationByte = decorationByte,
            };

            if (lexerKeywords.ContextualKeywords.Contains(textValue))
            {
                syntaxTokens.Add(new SyntaxToken(GetSyntaxKindForContextualKeyword(textSpan), textSpan));
                return;
            }

            syntaxTokens.Add(new SyntaxToken(GetSyntaxKindForKeyword(textSpan), textSpan));
            return;
        }

        syntaxTokens.Add(new SyntaxToken(SyntaxKind.IdentifierToken, textSpan));
    }

    public static void LexPlusToken(
        StringWalker stringWalker, List<SyntaxToken> syntaxTokens)
    {
        var entryPositionIndex = stringWalker.PositionIndex;

        stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            stringWalker.ResourceUri,
            stringWalker.SourceText);

        syntaxTokens.Add(new SyntaxToken(SyntaxKind.PlusToken, textSpan));
    }

    public static void LexPlusPlusToken(
        StringWalker stringWalker, List<SyntaxToken> syntaxTokens)
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

        syntaxTokens.Add(new SyntaxToken(SyntaxKind.PlusPlusToken, textSpan));
    }

    public static void LexMinusToken(
        StringWalker stringWalker, List<SyntaxToken> syntaxTokens)
    {
        var entryPositionIndex = stringWalker.PositionIndex;

        stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            stringWalker.ResourceUri,
            stringWalker.SourceText);

        syntaxTokens.Add(new SyntaxToken(SyntaxKind.MinusToken, textSpan));
    }

    public static void LexMinusMinusToken(
        StringWalker stringWalker, List<SyntaxToken> syntaxTokens)
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

        syntaxTokens.Add(new SyntaxToken(SyntaxKind.MinusMinusToken, textSpan));
    }

    public static void LexEqualsToken(
        StringWalker stringWalker, List<SyntaxToken> syntaxTokens)
    {
        var entryPositionIndex = stringWalker.PositionIndex;

        stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            stringWalker.ResourceUri,
            stringWalker.SourceText);

        syntaxTokens.Add(new SyntaxToken(SyntaxKind.EqualsToken, textSpan));
    }

    public static void LexEqualsEqualsToken(
        StringWalker stringWalker, List<SyntaxToken> syntaxTokens)
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

        syntaxTokens.Add(new SyntaxToken(SyntaxKind.EqualsEqualsToken, textSpan));
    }

    public static void LexQuestionMarkToken(
        StringWalker stringWalker, List<SyntaxToken> syntaxTokens)
    {
        var entryPositionIndex = stringWalker.PositionIndex;

        stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            stringWalker.ResourceUri,
            stringWalker.SourceText);

        syntaxTokens.Add(new SyntaxToken(SyntaxKind.QuestionMarkToken, textSpan));
    }

    public static void LexQuestionMarkQuestionMarkToken(
        StringWalker stringWalker, List<SyntaxToken> syntaxTokens)
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

        syntaxTokens.Add(new SyntaxToken(SyntaxKind.QuestionMarkQuestionMarkToken, textSpan));
    }

    public static void LexStarToken(
        StringWalker stringWalker, List<SyntaxToken> syntaxTokens)
    {
        var entryPositionIndex = stringWalker.PositionIndex;

        stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            stringWalker.ResourceUri,
            stringWalker.SourceText);

        syntaxTokens.Add(new SyntaxToken(SyntaxKind.StarToken, textSpan));
    }

    public static void LexDivisionToken(
        StringWalker stringWalker, List<SyntaxToken> syntaxTokens)
    {
        var entryPositionIndex = stringWalker.PositionIndex;

        stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            stringWalker.ResourceUri,
            stringWalker.SourceText);

        syntaxTokens.Add(new SyntaxToken(SyntaxKind.DivisionToken, textSpan));
    }

    public static void LexBangToken(
        StringWalker stringWalker, List<SyntaxToken> syntaxTokens)
    {
        var entryPositionIndex = stringWalker.PositionIndex;

        stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            stringWalker.ResourceUri,
            stringWalker.SourceText);

        syntaxTokens.Add(new SyntaxToken(SyntaxKind.BangToken, textSpan));
    }

    public static void LexStatementDelimiterToken(
        StringWalker stringWalker, List<SyntaxToken> syntaxTokens)
    {
        var entryPositionIndex = stringWalker.PositionIndex;

        stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            stringWalker.ResourceUri,
            stringWalker.SourceText);

        syntaxTokens.Add(new SyntaxToken(SyntaxKind.StatementDelimiterToken, textSpan));
    }

    public static void LexOpenParenthesisToken(
        StringWalker stringWalker, List<SyntaxToken> syntaxTokens)
    {
        var entryPositionIndex = stringWalker.PositionIndex;

        stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            stringWalker.ResourceUri,
            stringWalker.SourceText);

        syntaxTokens.Add(new SyntaxToken(SyntaxKind.OpenParenthesisToken, textSpan));
    }

    public static void LexCloseParenthesisToken(
        StringWalker stringWalker, List<SyntaxToken> syntaxTokens)
    {
        var entryPositionIndex = stringWalker.PositionIndex;

        stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            stringWalker.ResourceUri,
            stringWalker.SourceText);

        syntaxTokens.Add(new SyntaxToken(SyntaxKind.CloseParenthesisToken, textSpan));
    }

    public static void LexOpenBraceToken(
        StringWalker stringWalker, List<SyntaxToken> syntaxTokens)
    {
        var entryPositionIndex = stringWalker.PositionIndex;

        stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            stringWalker.ResourceUri,
            stringWalker.SourceText);

        syntaxTokens.Add(new SyntaxToken(SyntaxKind.OpenBraceToken, textSpan));
    }

    public static void LexCloseBraceToken(
        StringWalker stringWalker, List<SyntaxToken> syntaxTokens)
    {
        var entryPositionIndex = stringWalker.PositionIndex;

        stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            stringWalker.ResourceUri,
            stringWalker.SourceText);

        syntaxTokens.Add(new SyntaxToken(SyntaxKind.CloseBraceToken, textSpan));
    }

    public static void LexOpenAngleBracketToken(
        StringWalker stringWalker, List<SyntaxToken> syntaxTokens)
    {
        var entryPositionIndex = stringWalker.PositionIndex;

        stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            stringWalker.ResourceUri,
            stringWalker.SourceText);

        syntaxTokens.Add(new SyntaxToken(SyntaxKind.OpenAngleBracketToken, textSpan));
    }

    public static void LexCloseAngleBracketToken(
        StringWalker stringWalker, List<SyntaxToken> syntaxTokens)
    {
        var entryPositionIndex = stringWalker.PositionIndex;

        stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            stringWalker.ResourceUri,
            stringWalker.SourceText);

        syntaxTokens.Add(new SyntaxToken(SyntaxKind.CloseAngleBracketToken, textSpan));
    }

    public static void LexOpenSquareBracketToken(
        StringWalker stringWalker, List<SyntaxToken> syntaxTokens)
    {
        var entryPositionIndex = stringWalker.PositionIndex;

        stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            stringWalker.ResourceUri,
            stringWalker.SourceText);

        syntaxTokens.Add(new SyntaxToken(SyntaxKind.OpenSquareBracketToken, textSpan));
    }

    public static void LexCloseSquareBracketToken(
        StringWalker stringWalker, List<SyntaxToken> syntaxTokens)
    {
        var entryPositionIndex = stringWalker.PositionIndex;

        stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            stringWalker.ResourceUri,
            stringWalker.SourceText);

        syntaxTokens.Add(new SyntaxToken(SyntaxKind.CloseSquareBracketToken, textSpan));
    }

    public static void LexDollarSignToken(
        StringWalker stringWalker, List<SyntaxToken> syntaxTokens)
    {
        var entryPositionIndex = stringWalker.PositionIndex;

        stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            stringWalker.ResourceUri,
            stringWalker.SourceText);

        syntaxTokens.Add(new SyntaxToken(SyntaxKind.DollarSignToken, textSpan));
    }

    public static void LexAtToken(
        StringWalker stringWalker, List<SyntaxToken> syntaxTokens)
    {
        var entryPositionIndex = stringWalker.PositionIndex;

        stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            stringWalker.ResourceUri,
            stringWalker.SourceText);

        syntaxTokens.Add(new SyntaxToken(SyntaxKind.AtToken, textSpan));
    }

    public static void LexColonToken(
        StringWalker stringWalker, List<SyntaxToken> syntaxTokens)
    {
        var entryPositionIndex = stringWalker.PositionIndex;

        stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            stringWalker.ResourceUri,
            stringWalker.SourceText);

        syntaxTokens.Add(new SyntaxToken(SyntaxKind.ColonToken, textSpan));
    }

    public static void LexMemberAccessToken(
        StringWalker stringWalker, List<SyntaxToken> syntaxTokens)
    {
        var entryPositionIndex = stringWalker.PositionIndex;

        stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            stringWalker.ResourceUri,
            stringWalker.SourceText);

        syntaxTokens.Add(new SyntaxToken(SyntaxKind.MemberAccessToken, textSpan));
    }

    public static void LexCommaToken(
        StringWalker stringWalker, List<SyntaxToken> syntaxTokens)
    {
        var entryPositionIndex = stringWalker.PositionIndex;

        stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            stringWalker.ResourceUri,
            stringWalker.SourceText);

        syntaxTokens.Add(new SyntaxToken(SyntaxKind.CommaToken, textSpan));
    }

    public static void LexPreprocessorDirectiveToken(
        StringWalker stringWalker, List<SyntaxToken> syntaxTokens)
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

        syntaxTokens.Add(new SyntaxToken(SyntaxKind.PreprocessorDirectiveToken, textSpan));
    }

    public static SyntaxKind GetSyntaxKindForKeyword(TextEditorTextSpan textSpan)
    {
        var text = textSpan.GetText();

        switch (text)
        {
            case "abstract":
                return SyntaxKind.AbstractTokenKeyword;
            case "as":
                return SyntaxKind.AsTokenKeyword;
            case "base":
                return SyntaxKind.BaseTokenKeyword;
            case "bool":
                return SyntaxKind.BoolTokenKeyword;
            case "break":
                return SyntaxKind.BreakTokenKeyword;
            case "byte":
                return SyntaxKind.ByteTokenKeyword;
            case "case":
                return SyntaxKind.CaseTokenKeyword;
            case "catch":
                return SyntaxKind.CatchTokenKeyword;
            case "char":
                return SyntaxKind.CharTokenKeyword;
            case "checked":
                return SyntaxKind.CheckedTokenKeyword;
            case "class":
                return SyntaxKind.ClassTokenKeyword;
            case "const":
                return SyntaxKind.ConstTokenKeyword;
            case "continue":
                return SyntaxKind.ContinueTokenKeyword;
            case "decimal":
                return SyntaxKind.DecimalTokenKeyword;
            case "default":
                return SyntaxKind.DefaultTokenKeyword;
            case "delegate":
                return SyntaxKind.DelegateTokenKeyword;
            case "do":
                return SyntaxKind.DoTokenKeyword;
            case "double":
                return SyntaxKind.DoubleTokenKeyword;
            case "else":
                return SyntaxKind.ElseTokenKeyword;
            case "enum":
                return SyntaxKind.EnumTokenKeyword;
            case "event":
                return SyntaxKind.EventTokenKeyword;
            case "explicit":
                return SyntaxKind.ExplicitTokenKeyword;
            case "extern":
                return SyntaxKind.ExternTokenKeyword;
            case "false":
                return SyntaxKind.FalseTokenKeyword;
            case "finally":
                return SyntaxKind.FinallyTokenKeyword;
            case "fixed":
                return SyntaxKind.FixedTokenKeyword;
            case "float":
                return SyntaxKind.FloatTokenKeyword;
            case "for":
                return SyntaxKind.ForTokenKeyword;
            case "foreach":
                return SyntaxKind.ForeachTokenKeyword;
            case "goto":
                return SyntaxKind.GotoTokenKeyword;
            case "if":
                return SyntaxKind.IfTokenKeyword;
            case "implicit":
                return SyntaxKind.ImplicitTokenKeyword;
            case "in":
                return SyntaxKind.InTokenKeyword;
            case "int":
                return SyntaxKind.IntTokenKeyword;
            case "interface":
                return SyntaxKind.InterfaceTokenKeyword;
            case "internal":
                return SyntaxKind.InternalTokenKeyword;
            case "is":
                return SyntaxKind.IsTokenKeyword;
            case "lock":
                return SyntaxKind.LockTokenKeyword;
            case "long":
                return SyntaxKind.LongTokenKeyword;
            case "namespace":
                return SyntaxKind.NamespaceTokenKeyword;
            case "new":
                return SyntaxKind.NewTokenKeyword;
            case "null":
                return SyntaxKind.NullTokenKeyword;
            case "object":
                return SyntaxKind.ObjectTokenKeyword;
            case "operator":
                return SyntaxKind.OperatorTokenKeyword;
            case "out":
                return SyntaxKind.OutTokenKeyword;
            case "override":
                return SyntaxKind.OverrideTokenKeyword;
            case "params":
                return SyntaxKind.ParamsTokenKeyword;
            case "private":
                return SyntaxKind.PrivateTokenKeyword;
            case "protected":
                return SyntaxKind.ProtectedTokenKeyword;
            case "public":
                return SyntaxKind.PublicTokenKeyword;
            case "readonly":
                return SyntaxKind.ReadonlyTokenKeyword;
            case "ref":
                return SyntaxKind.RefTokenKeyword;
            case "return":
                return SyntaxKind.ReturnTokenKeyword;
            case "sbyte":
                return SyntaxKind.SbyteTokenKeyword;
            case "sealed":
                return SyntaxKind.SealedTokenKeyword;
            case "short":
                return SyntaxKind.ShortTokenKeyword;
            case "sizeof":
                return SyntaxKind.SizeofTokenKeyword;
            case "stackalloc":
                return SyntaxKind.StackallocTokenKeyword;
            case "static":
                return SyntaxKind.StaticTokenKeyword;
            case "string":
                return SyntaxKind.StringTokenKeyword;
            case "struct":
                return SyntaxKind.StructTokenKeyword;
            case "switch":
                return SyntaxKind.SwitchTokenKeyword;
            case "this":
                return SyntaxKind.ThisTokenKeyword;
            case "throw":
                return SyntaxKind.ThrowTokenKeyword;
            case "true":
                return SyntaxKind.TrueTokenKeyword;
            case "try":
                return SyntaxKind.TryTokenKeyword;
            case "typeof":
                return SyntaxKind.TypeofTokenKeyword;
            case "uint":
                return SyntaxKind.UintTokenKeyword;
            case "ulong":
                return SyntaxKind.UlongTokenKeyword;
            case "unchecked":
                return SyntaxKind.UncheckedTokenKeyword;
            case "unsafe":
                return SyntaxKind.UnsafeTokenKeyword;
            case "ushort":
                return SyntaxKind.UshortTokenKeyword;
            case "using":
                return SyntaxKind.UsingTokenKeyword;
            case "virtual":
                return SyntaxKind.VirtualTokenKeyword;
            case "void":
                return SyntaxKind.VoidTokenKeyword;
            case "volatile":
                return SyntaxKind.VolatileTokenKeyword;
            case "while":
                return SyntaxKind.WhileTokenKeyword;
            default:
                return SyntaxKind.UnrecognizedTokenKeyword;
        }
    }

    public static SyntaxKind GetSyntaxKindForContextualKeyword(TextEditorTextSpan textSpan)
    {
        var text = textSpan.GetText();

        switch (text)
        {
            case "add":
                return SyntaxKind.AddTokenContextualKeyword;
            case "and":
                return SyntaxKind.AndTokenContextualKeyword;
            case "alias":
                return SyntaxKind.AliasTokenContextualKeyword;
            case "ascending":
                return SyntaxKind.AscendingTokenContextualKeyword;
            case "args":
                return SyntaxKind.ArgsTokenContextualKeyword;
            case "async":
                return SyntaxKind.AsyncTokenContextualKeyword;
            case "await":
                return SyntaxKind.AwaitTokenContextualKeyword;
            case "by":
                return SyntaxKind.ByTokenContextualKeyword;
            case "descending":
                return SyntaxKind.DescendingTokenContextualKeyword;
            case "dynamic":
                return SyntaxKind.DynamicTokenContextualKeyword;
            case "equals":
                return SyntaxKind.EqualsTokenContextualKeyword;
            case "file":
                return SyntaxKind.FileTokenContextualKeyword;
            case "from":
                return SyntaxKind.FromTokenContextualKeyword;
            case "get":
                return SyntaxKind.GetTokenContextualKeyword;
            case "global":
                return SyntaxKind.GlobalTokenContextualKeyword;
            case "group":
                return SyntaxKind.GroupTokenContextualKeyword;
            case "init":
                return SyntaxKind.InitTokenContextualKeyword;
            case "into":
                return SyntaxKind.IntoTokenContextualKeyword;
            case "join":
                return SyntaxKind.JoinTokenContextualKeyword;
            case "let":
                return SyntaxKind.LetTokenContextualKeyword;
            case "managed":
                return SyntaxKind.ManagedTokenContextualKeyword;
            case "nameof":
                return SyntaxKind.NameofTokenContextualKeyword;
            case "nint":
                return SyntaxKind.NintTokenContextualKeyword;
            case "not":
                return SyntaxKind.NotTokenContextualKeyword;
            case "notnull":
                return SyntaxKind.NotnullTokenContextualKeyword;
            case "nuint":
                return SyntaxKind.NuintTokenContextualKeyword;
            case "on":
                return SyntaxKind.OnTokenContextualKeyword;
            case "or":
                return SyntaxKind.OrTokenContextualKeyword;
            case "orderby":
                return SyntaxKind.OrderbyTokenContextualKeyword;
            case "partial":
                return SyntaxKind.PartialTokenContextualKeyword;
            case "record":
                return SyntaxKind.RecordTokenContextualKeyword;
            case "remove":
                return SyntaxKind.RemoveTokenContextualKeyword;
            case "required":
                return SyntaxKind.RequiredTokenContextualKeyword;
            case "scoped":
                return SyntaxKind.ScopedTokenContextualKeyword;
            case "select":
                return SyntaxKind.SelectTokenContextualKeyword;
            case "set":
                return SyntaxKind.SetTokenContextualKeyword;
            case "unmanaged":
                return SyntaxKind.UnmanagedTokenContextualKeyword;
            case "value":
                return SyntaxKind.ValueTokenContextualKeyword;
            case "var":
                return SyntaxKind.VarTokenContextualKeyword;
            case "when":
                return SyntaxKind.WhenTokenContextualKeyword;
            case "where":
                return SyntaxKind.WhereTokenContextualKeyword;
            case "with":
                return SyntaxKind.WithTokenContextualKeyword;
            case "yield":
                return SyntaxKind.YieldTokenContextualKeyword;
            default:
                return SyntaxKind.UnrecognizedTokenContextualKeyword;
        }
    }
}
