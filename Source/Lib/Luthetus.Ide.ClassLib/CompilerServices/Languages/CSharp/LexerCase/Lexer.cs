using Luthetus.Ide.ClassLib.CompilerServices.Common.General;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax;
using Luthetus.Ide.ClassLib.CompilerServices.Common.Syntax.SyntaxTokens;
using Luthetus.TextEditor.RazorLib.Analysis;
using Luthetus.TextEditor.RazorLib.Analysis.CSharp.Facts;
using Luthetus.TextEditor.RazorLib.Analysis.GenericLexer.Decoration;
using Luthetus.TextEditor.RazorLib.Lexing;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.LexerCase;

public class Lexer
{
    private readonly StringWalker _stringWalker;
    private readonly List<ISyntaxToken> _syntaxTokens = new();
    private readonly LuthetusIdeDiagnosticBag _diagnosticBag = new();

    public Lexer(
        ResourceUri resourceUri,
        string sourceText)
    {
        _stringWalker = new(resourceUri, sourceText);
    }

    public ImmutableArray<ISyntaxToken> SyntaxTokens => _syntaxTokens.ToImmutableArray();
    public ImmutableArray<TextEditorDiagnostic> Diagnostics => _diagnosticBag.ToImmutableArray();

    public void Lex()
    {
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
                    var numericLiteralToken = LexNumericLiteralToken();
                    _syntaxTokens.Add(numericLiteralToken);
                    break;
                case '"':
                    var stringLiteralToken = LexStringLiteralToken();
                    _syntaxTokens.Add(stringLiteralToken);
                    break;
                case '/':
                    if (_stringWalker.PeekCharacter(1) == '/')
                    {
                        var commentSingleLineToken = LexCommentSingleLineToken();
                        _syntaxTokens.Add(commentSingleLineToken);
                    }
                    else if (_stringWalker.PeekCharacter(1) == '*')
                    {
                        var commentMultiLineToken = LexCommentMultiLineToken();
                        _syntaxTokens.Add(commentMultiLineToken);
                    }

                    break;
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
                    var identifierOrKeywordToken = LexIdentifierOrKeyword();
                    _syntaxTokens.Add(identifierOrKeywordToken);
                    break;
                case '+':
                    var plusToken = LexPlusToken();
                    _syntaxTokens.Add(plusToken);
                    break;
                case '=':
                    var equalsToken = LexEqualsToken();
                    _syntaxTokens.Add(equalsToken);
                    break;
                case ';':
                    var statementDelimiterToken = LexStatementDelimiterToken();
                    _syntaxTokens.Add(statementDelimiterToken);
                    break;
                case '(':
                    var openParenthesisToken = LexOpenParenthesisToken();
                    _syntaxTokens.Add(openParenthesisToken);
                    break;
                case ')':
                    var closeParenthesisToken = LexCloseParenthesisToken();
                    _syntaxTokens.Add(closeParenthesisToken);
                    break;
                case '{':
                    var openBraceToken = LexOpenBraceToken();
                    _syntaxTokens.Add(openBraceToken);
                    break;
                case '}':
                    var closeBraceToken = LexCloseBraceToken();
                    _syntaxTokens.Add(closeBraceToken);
                    break;
                case ':':
                    var colonToken = LexColonToken();
                    _syntaxTokens.Add(colonToken);
                    break;
                case '.':
                    var memberAccessToken = LexMemberAccessToken();
                    _syntaxTokens.Add(memberAccessToken);
                    break;
                case '#':
                    var preprocessorDirectiveToken = LexPreprocessorDirectiveToken();
                    _syntaxTokens.Add(preprocessorDirectiveToken);
                    break;
                default:
                    _ = _stringWalker.ReadCharacter();
                    break;
            }
        }

        var endOfFileTextSpan = new TextEditorTextSpan(
            _stringWalker.PositionIndex,
            _stringWalker.PositionIndex + 1,
            (byte)GenericDecorationKind.None,
            _stringWalker.ResourceUri,
            _stringWalker.SourceText);

        _syntaxTokens.Add(new EndOfFileToken(endOfFileTextSpan));
    }

    private NumericLiteralToken LexNumericLiteralToken()
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

        return new NumericLiteralToken(textSpan);
    }

    private StringLiteralToken LexStringLiteralToken()
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

        return new StringLiteralToken(textSpan);
    }

    private CommentSingleLineToken LexCommentSingleLineToken()
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

        return new CommentSingleLineToken(textSpan);
    }

    private CommentMultiLineToken LexCommentMultiLineToken()
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

        return new CommentMultiLineToken(textSpan);
    }

    private ISyntaxToken LexIdentifierOrKeyword()
    {
        var entryPositionIndex = _stringWalker.PositionIndex;

        while (!_stringWalker.IsEof)
        {
            if (char.IsWhiteSpace(_stringWalker.CurrentCharacter) ||
                char.IsPunctuation(_stringWalker.CurrentCharacter) &&
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

        if (CSharpKeywords.ALL.Contains(textValue))
        {
            var decorationByte = (byte)GenericDecorationKind.Keyword;

            if (CSharpKeywords.CONTROL_KEYWORDS.Contains(textValue))
                decorationByte = (byte)GenericDecorationKind.KeywordControl;

            textSpan = textSpan with
            {
                DecorationByte = decorationByte,
            };

            return new KeywordToken(textSpan);
        }

        return new IdentifierToken(textSpan);
    }

    private PlusToken LexPlusToken()
    {
        var entryPositionIndex = _stringWalker.PositionIndex;

        _stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            _stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            _stringWalker.ResourceUri,
            _stringWalker.SourceText);

        return new PlusToken(textSpan);
    }

    private EqualsToken LexEqualsToken()
    {
        var entryPositionIndex = _stringWalker.PositionIndex;

        _stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            _stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            _stringWalker.ResourceUri,
            _stringWalker.SourceText);

        return new EqualsToken(textSpan);
    }

    private StatementDelimiterToken LexStatementDelimiterToken()
    {
        var entryPositionIndex = _stringWalker.PositionIndex;

        _stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            _stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            _stringWalker.ResourceUri,
            _stringWalker.SourceText);

        return new StatementDelimiterToken(textSpan);
    }

    private OpenParenthesisToken LexOpenParenthesisToken()
    {
        var entryPositionIndex = _stringWalker.PositionIndex;

        _stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            _stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            _stringWalker.ResourceUri,
            _stringWalker.SourceText);

        return new OpenParenthesisToken(textSpan);
    }

    private CloseParenthesisToken LexCloseParenthesisToken()
    {
        var entryPositionIndex = _stringWalker.PositionIndex;

        _stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            _stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            _stringWalker.ResourceUri,
            _stringWalker.SourceText);

        return new CloseParenthesisToken(textSpan);
    }

    private OpenBraceToken LexOpenBraceToken()
    {
        var entryPositionIndex = _stringWalker.PositionIndex;

        _stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            _stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            _stringWalker.ResourceUri,
            _stringWalker.SourceText);

        return new OpenBraceToken(textSpan);
    }

    private CloseBraceToken LexCloseBraceToken()
    {
        var entryPositionIndex = _stringWalker.PositionIndex;

        _stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            _stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            _stringWalker.ResourceUri,
            _stringWalker.SourceText);

        return new CloseBraceToken(textSpan);
    }
    
    private ColonToken LexColonToken()
    {
        var entryPositionIndex = _stringWalker.PositionIndex;

        _stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            _stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            _stringWalker.ResourceUri,
            _stringWalker.SourceText);

        return new ColonToken(textSpan);
    }
    
    private MemberAccessToken LexMemberAccessToken()
    {
        var entryPositionIndex = _stringWalker.PositionIndex;

        _stringWalker.ReadCharacter();

        var textSpan = new TextEditorTextSpan(
            entryPositionIndex,
            _stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            _stringWalker.ResourceUri,
            _stringWalker.SourceText);

        return new MemberAccessToken(textSpan);
    }

    private PreprocessorDirectiveToken LexPreprocessorDirectiveToken()
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

        return new PreprocessorDirectiveToken(textSpan);
    }
}
