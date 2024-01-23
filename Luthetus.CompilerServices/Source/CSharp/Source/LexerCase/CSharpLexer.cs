using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.CompilerServices.GenericLexer.Decoration;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.CSharp.LexerCase;

public class CSharpLexer : ILexer
{
    private readonly StringWalker _stringWalker;
    private readonly List<ISyntaxToken> _syntaxTokens = new();
    private readonly LuthetusDiagnosticBag _diagnosticBag = new();

    public CSharpLexer(
        ResourceUri resourceUri,
        string sourceText)
    {
        _stringWalker = new(resourceUri, sourceText);
        ResourceUri = resourceUri;
    }

    public ImmutableArray<ISyntaxToken> SyntaxTokens => _syntaxTokens.ToImmutableArray();
    public ImmutableArray<TextEditorDiagnostic> DiagnosticList => _diagnosticBag.ToImmutableArray();

    public ResourceUri ResourceUri { get; }

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
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    LexNumericLiteralToken();
                    break;
                case '"':
                    LexStringLiteralToken();
                    break;
                case '/':
                    if (_stringWalker.PeekCharacter(1) == '/')
                        LexCommentSingleLineToken();
                    else if (_stringWalker.PeekCharacter(1) == '*')
                        LexCommentMultiLineToken();
                    else
                        LexDivisionToken();

                    break;
                case '+':
                    if (_stringWalker.PeekCharacter(1) == '+')
                        LexPlusPlusToken();
                    else
                        LexPlusToken();

                    break;
                case '-':
                    if (_stringWalker.PeekCharacter(1) == '-')
                        LexMinusMinusToken();
                    else
                        LexMinusToken();

                    break;
                case '=':
                    if (_stringWalker.PeekCharacter(1) == '=')
                        LexEqualsEqualsToken();
                    else
                        LexEqualsToken();

                    break;
                case '?':
                    if (_stringWalker.PeekCharacter(1) == '?')
                        LexQuestionMarkQuestionMarkToken();
                    else
                        LexQuestionMarkToken();

                    break;
                case '*':
                    LexStarToken();
                    break;
                case '!':
                    LexBangToken();
                    break;
                case ';':
                    LexStatementDelimiterToken();
                    break;
                case '(':
                    LexOpenParenthesisToken();
                    break;
                case ')':
                    LexCloseParenthesisToken();
                    break;
                case '{':
                    LexOpenBraceToken();
                    break;
                case '}':
                    LexCloseBraceToken();
                    break;
                case '<':
                    LexOpenAngleBracketToken();
                    break;
                case '>':
                    LexCloseAngleBracketToken();
                    break;
                case '[':
                    LexOpenSquareBracketToken();
                    break;
                case ']':
                    LexCloseSquareBracketToken();
                    break;
                case '$':
                    LexDollarSignToken();
                    break;
                case ':':
                    LexColonToken();
                    break;
                case '.':
                    LexMemberAccessToken();
                    break;
                case ',':
                    LexCommaToken();
                    break;
                case '#':
                    LexPreprocessorDirectiveToken();
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

        _syntaxTokens.Add(new EndOfFileToken(endOfFileTextSpan));
    }

    private void LexNumericLiteralToken()
    {
        var entryPositionIndex = _stringWalker.PositionIndex;

        // Declare outside the while loop to avoid overhead of redeclaring each loop? not sure
        var isNotANumber = false;

        while (!_stringWalker.IsEof)
        {
            switch (_stringWalker.CurrentCharacter)
            {
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

            _ = _stringWalker.ReadCharacter();
        }

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            _stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            _stringWalker.ResourceUri,
            _stringWalker.SourceText);

        _syntaxTokens.Add(new NumericLiteralToken(textSpan));
    }

    private void LexStringLiteralToken()
    {
        var entryPositionIndex = _stringWalker.PositionIndex;

        // Move past the initial opening character
        _ = _stringWalker.ReadCharacter();

        // Declare outside the while loop to avoid overhead of redeclaring each loop? not sure
        var wasClosingCharacter = false;

        while (!_stringWalker.IsEof)
        {
            switch (_stringWalker.CurrentCharacter)
            {
                case '"':
                    wasClosingCharacter = true;
                    break;
                default:
                    break;
            }

            _ = _stringWalker.ReadCharacter();

            if (wasClosingCharacter)
                break;
        }

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            _stringWalker.PositionIndex,
            (byte)GenericDecorationKind.StringLiteral,
            _stringWalker.ResourceUri,
            _stringWalker.SourceText);

        _syntaxTokens.Add(new StringLiteralToken(textSpan));
    }

    private void LexCommentSingleLineToken()
    {
        var entryPositionIndex = _stringWalker.PositionIndex;

        // Declare outside the while loop to avoid overhead of redeclaring each loop? not sure
        var isNewLineCharacter = false;

        while (!_stringWalker.IsEof)
        {
            switch (_stringWalker.CurrentCharacter)
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

            _ = _stringWalker.ReadCharacter();
        }

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            _stringWalker.PositionIndex,
            (byte)GenericDecorationKind.CommentSingleLine,
            _stringWalker.ResourceUri,
            _stringWalker.SourceText);

        _syntaxTokens.Add(new CommentSingleLineToken(textSpan));
    }

    private void LexCommentMultiLineToken()
    {
        var entryPositionIndex = _stringWalker.PositionIndex;

        // Move past the initial "/*"
        _ = _stringWalker.ReadRange(2);

        // Declare outside the while loop to avoid overhead of redeclaring each loop? not sure
        var possibleClosingText = false;
        var sawClosingText = false;

        while (!_stringWalker.IsEof)
        {
            switch (_stringWalker.CurrentCharacter)
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

            _ = _stringWalker.ReadCharacter();

            if (sawClosingText)
                break;
        }

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            _stringWalker.PositionIndex,
            (byte)GenericDecorationKind.CommentMultiLine,
            _stringWalker.ResourceUri,
            _stringWalker.SourceText);

        _syntaxTokens.Add(new CommentMultiLineToken(textSpan));
    }

    private void LexIdentifierOrKeywordOrKeywordContextual()
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

        if (CSharpKeywords.ALL_KEYWORDS.Contains(textValue))
        {
            var decorationByte = (byte)GenericDecorationKind.Keyword;

            if (CSharpKeywords.CONTROL_KEYWORDS.Contains(textValue))
                decorationByte = (byte)GenericDecorationKind.KeywordControl;

            textSpan = textSpan with
            {
                DecorationByte = decorationByte,
            };

            if (CSharpKeywords.CONTEXTUAL_KEYWORDS.Contains(textValue))
            {
                _syntaxTokens.Add(new KeywordContextualToken(textSpan, GetSyntaxKindForContextualKeyword(textSpan)));
                return;
            }

            _syntaxTokens.Add(new KeywordToken(textSpan, GetSyntaxKindForKeyword(textSpan)));
            return;
        }

        _syntaxTokens.Add(new IdentifierToken(textSpan));
    }

    private void LexPlusToken()
    {
        var entryPositionIndex = _stringWalker.PositionIndex;

        _stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            _stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            _stringWalker.ResourceUri,
            _stringWalker.SourceText);

        _syntaxTokens.Add(new PlusToken(textSpan));
    }

    private void LexPlusPlusToken()
    {
        var entryPositionIndex = _stringWalker.PositionIndex;

        // First '+'
        _stringWalker.ReadCharacter();
        // Second '+'
        _stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            _stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            _stringWalker.ResourceUri,
            _stringWalker.SourceText);

        _syntaxTokens.Add(new PlusPlusToken(textSpan));
    }

    private void LexMinusToken()
    {
        var entryPositionIndex = _stringWalker.PositionIndex;

        _stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            _stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            _stringWalker.ResourceUri,
            _stringWalker.SourceText);

        _syntaxTokens.Add(new MinusToken(textSpan));
    }

    private void LexMinusMinusToken()
    {
        var entryPositionIndex = _stringWalker.PositionIndex;

        // First '-'
        _stringWalker.ReadCharacter();
        // Second '-'
        _stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            _stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            _stringWalker.ResourceUri,
            _stringWalker.SourceText);

        _syntaxTokens.Add(new MinusMinusToken(textSpan));
    }

    private void LexEqualsToken()
    {
        var entryPositionIndex = _stringWalker.PositionIndex;

        _stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            _stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            _stringWalker.ResourceUri,
            _stringWalker.SourceText);

        _syntaxTokens.Add(new EqualsToken(textSpan));
    }

    private void LexEqualsEqualsToken()
    {
        var entryPositionIndex = _stringWalker.PositionIndex;

        // First '='
        _stringWalker.ReadCharacter();
        // Second '='
        _stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            _stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            _stringWalker.ResourceUri,
            _stringWalker.SourceText);

        _syntaxTokens.Add(new EqualsEqualsToken(textSpan));
    }

    private void LexQuestionMarkToken()
    {
        var entryPositionIndex = _stringWalker.PositionIndex;

        _stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            _stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            _stringWalker.ResourceUri,
            _stringWalker.SourceText);

        _syntaxTokens.Add(new QuestionMarkToken(textSpan));
    }

    private void LexQuestionMarkQuestionMarkToken()
    {
        var entryPositionIndex = _stringWalker.PositionIndex;

        // First '?'
        _stringWalker.ReadCharacter();
        // Second '?'
        _stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            _stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            _stringWalker.ResourceUri,
            _stringWalker.SourceText);

        _syntaxTokens.Add(new QuestionMarkQuestionMarkToken(textSpan));
    }

    private void LexStarToken()
    {
        var entryPositionIndex = _stringWalker.PositionIndex;

        _stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            _stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            _stringWalker.ResourceUri,
            _stringWalker.SourceText);

        _syntaxTokens.Add(new StarToken(textSpan));
    }
    
    private void LexDivisionToken()
    {
        var entryPositionIndex = _stringWalker.PositionIndex;

        _stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            _stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            _stringWalker.ResourceUri,
            _stringWalker.SourceText);

        _syntaxTokens.Add(new DivisionToken(textSpan));
    }
    
    private void LexBangToken()
    {
        var entryPositionIndex = _stringWalker.PositionIndex;

        _stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            _stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            _stringWalker.ResourceUri,
            _stringWalker.SourceText);

        _syntaxTokens.Add(new BangToken(textSpan));
    }

    private void LexStatementDelimiterToken()
    {
        var entryPositionIndex = _stringWalker.PositionIndex;

        _stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            _stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            _stringWalker.ResourceUri,
            _stringWalker.SourceText);

        _syntaxTokens.Add(new StatementDelimiterToken(textSpan));
    }

    private void LexOpenParenthesisToken()
    {
        var entryPositionIndex = _stringWalker.PositionIndex;

        _stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            _stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            _stringWalker.ResourceUri,
            _stringWalker.SourceText);

        _syntaxTokens.Add(new OpenParenthesisToken(textSpan));
    }

    private void LexCloseParenthesisToken()
    {
        var entryPositionIndex = _stringWalker.PositionIndex;

        _stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            _stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            _stringWalker.ResourceUri,
            _stringWalker.SourceText);

        _syntaxTokens.Add(new CloseParenthesisToken(textSpan));
    }

    private void LexOpenBraceToken()
    {
        var entryPositionIndex = _stringWalker.PositionIndex;

        _stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            _stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            _stringWalker.ResourceUri,
            _stringWalker.SourceText);

        _syntaxTokens.Add(new OpenBraceToken(textSpan));
    }

    private void LexCloseBraceToken()
    {
        var entryPositionIndex = _stringWalker.PositionIndex;

        _stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            _stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            _stringWalker.ResourceUri,
            _stringWalker.SourceText);

        _syntaxTokens.Add(new CloseBraceToken(textSpan));
    }

    private void LexOpenAngleBracketToken()
    {
        var entryPositionIndex = _stringWalker.PositionIndex;

        _stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            _stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            _stringWalker.ResourceUri,
            _stringWalker.SourceText);

        _syntaxTokens.Add(new OpenAngleBracketToken(textSpan));
    }

    private void LexCloseAngleBracketToken()
    {
        var entryPositionIndex = _stringWalker.PositionIndex;

        _stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            _stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            _stringWalker.ResourceUri,
            _stringWalker.SourceText);

        _syntaxTokens.Add(new CloseAngleBracketToken(textSpan));
    }

    private void LexOpenSquareBracketToken()
    {
        var entryPositionIndex = _stringWalker.PositionIndex;

        _stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            _stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            _stringWalker.ResourceUri,
            _stringWalker.SourceText);

        _syntaxTokens.Add(new OpenSquareBracketToken(textSpan));
    }

    private void LexCloseSquareBracketToken()
    {
        var entryPositionIndex = _stringWalker.PositionIndex;

        _stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            _stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            _stringWalker.ResourceUri,
            _stringWalker.SourceText);

        _syntaxTokens.Add(new CloseSquareBracketToken(textSpan));
    }

    private void LexDollarSignToken()
    {
        var entryPositionIndex = _stringWalker.PositionIndex;

        _stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            _stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            _stringWalker.ResourceUri,
            _stringWalker.SourceText);

        _syntaxTokens.Add(new DollarSignToken(textSpan));
    }

    private void LexColonToken()
    {
        var entryPositionIndex = _stringWalker.PositionIndex;

        _stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            _stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            _stringWalker.ResourceUri,
            _stringWalker.SourceText);

        _syntaxTokens.Add(new ColonToken(textSpan));
    }

    private void LexMemberAccessToken()
    {
        var entryPositionIndex = _stringWalker.PositionIndex;

        _stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            _stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            _stringWalker.ResourceUri,
            _stringWalker.SourceText);

        _syntaxTokens.Add(new MemberAccessToken(textSpan));
    }

    private void LexCommaToken()
    {
        var entryPositionIndex = _stringWalker.PositionIndex;

        _stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            _stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            _stringWalker.ResourceUri,
            _stringWalker.SourceText);

        _syntaxTokens.Add(new CommaToken(textSpan));
    }

    private void LexPreprocessorDirectiveToken()
    {
        var entryPositionIndex = _stringWalker.PositionIndex;

        // Declare outside the while loop to avoid overhead of redeclaring each loop? not sure
        var isNewLineCharacter = false;

        while (!_stringWalker.IsEof)
        {
            switch (_stringWalker.CurrentCharacter)
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

            _ = _stringWalker.ReadCharacter();
        }

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            _stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            _stringWalker.ResourceUri,
            _stringWalker.SourceText);

        _syntaxTokens.Add(new PreprocessorDirectiveToken(textSpan));
    }
    
    private SyntaxKind GetSyntaxKindForKeyword(TextEditorTextSpan textSpan)
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

    private SyntaxKind GetSyntaxKindForContextualKeyword(TextEditorTextSpan textSpan)
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