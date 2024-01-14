// TODO: All C logic is commented out due to breaking changes in the TextEditor API. The...
// ...C# compiler service is the focus while API decisions are being made, lest twice the code...
// ...need be modified for every API change (2023-10-04)
//
// TODO: Fix CLexerSession, it broke on (2023-07-26)
//
// namespace Luthetus.CompilerServices.Lang.C.LexerCase;
//
// public class CLexerSession : ILexer
// {
//     private readonly StringWalker _stringWalker;
//     private readonly List<ISyntaxToken> _syntaxTokens = new();
//     private readonly LuthetusIdeDiagnosticBag _diagnosticBag = new();
//
//     public CLexerSession(ResourceUri resourceUri, string sourceText)
//     {
//         _stringWalker = new StringWalker(resourceUri, sourceText);
//     }
//
//     public ImmutableArray<ISyntaxToken> SyntaxTokens => _syntaxTokens.ToImmutableArray();
//     public ImmutableArray<TextEditorDiagnostic> DiagnosticsList => _diagnosticBag.ToImmutableArray();
//
//     /// <summary>General idea for this Lex method is to use a switch statement to invoke a method which returns the specific token.<br/><br/>The method also moves the position in the content forward.</summary>
//     public void Lex()
//     {
//         while (!_stringWalker.IsEof)
//         {
//             switch (_stringWalker.CurrentCharacter)
//             {
//                 /* Digits */
//                 case '0':
//                 case '1':
//                 case '2':
//                 case '3':
//                 case '4':
//                 case '5':
//                 case '6':
//                 case '7':
//                 case '8':
//                 case '9':
//                     var numericLiteralToken = ConsumeNumericLiteral();
//                     _syntaxTokens.Add(numericLiteralToken);
//                     break;
//                 /* Lowercase Letters */
//                 case 'a':
//                 case 'b':
//                 case 'c':
//                 case 'd':
//                 case 'e':
//                 case 'f':
//                 case 'g':
//                 case 'h':
//                 case 'i':
//                 case 'j':
//                 case 'k':
//                 case 'l':
//                 case 'm':
//                 case 'n':
//                 case 'o':
//                 case 'p':
//                 case 'q':
//                 case 'r':
//                 case 's':
//                 case 't':
//                 case 'u':
//                 case 'v':
//                 case 'w':
//                 case 'x':
//                 case 'y':
//                 case 'z':
//                 /* Uppercase Letters */
//                 case 'A':
//                 case 'B':
//                 case 'C':
//                 case 'D':
//                 case 'E':
//                 case 'F':
//                 case 'G':
//                 case 'H':
//                 case 'I':
//                 case 'J':
//                 case 'K':
//                 case 'L':
//                 case 'M':
//                 case 'N':
//                 case 'O':
//                 case 'P':
//                 case 'Q':
//                 case 'R':
//                 case 'S':
//                 case 'T':
//                 case 'U':
//                 case 'V':
//                 case 'W':
//                 case 'X':
//                 case 'Y':
//                 case 'Z':
//                 /* Underscore */
//                 case '_':
//                     var identifierOrKeywordToken = ConsumeIdentifierOrKeyword();
//                     _syntaxTokens.Add(identifierOrKeywordToken);
//                     break;
//                 case '+':
//                     var plusToken = ConsumePlus();
//                     _syntaxTokens.Add(plusToken);
//                     break;
//                 case '=':
//                     var equalsToken = ConsumeEquals();
//                     _syntaxTokens.Add(equalsToken);
//                     break;
//                 case '(':
//                     var openParenthesisToken = ConsumeOpenParenthesis();
//                     _syntaxTokens.Add(openParenthesisToken);
//                     break;
//                 case ')':
//                     var closeParenthesisToken = ConsumeCloseParenthesis();
//                     _syntaxTokens.Add(closeParenthesisToken);
//                     break;
//                 case '{':
//                     var openBraceToken = ConsumeOpenBrace();
//                     _syntaxTokens.Add(openBraceToken);
//                     break;
//                 case '}':
//                     var closeBraceToken = ConsumeCloseBrace();
//                     _syntaxTokens.Add(closeBraceToken);
//                     break;
//                 case CLanguageFacts.STATEMENT_DELIMITER_CHAR:
//                     var statementDelimiterToken = ConsumeStatementDelimiter();
//                     _syntaxTokens.Add(statementDelimiterToken);
//                     break;
//                 case CLanguageFacts.STRING_LITERAL_START:
//                     var stringLiteralToken = ConsumeStringLiteral();
//                     _syntaxTokens.Add(stringLiteralToken);
//                     break;
//                 case CLanguageFacts.COMMENT_SINGLE_LINE_STARTING_CHAR:
//                     if (_stringWalker.PeekCharacter(1) == '/')
//                     {
//                         var commentSingleLineToken = ConsumeCommentSingleLine();
//                         _syntaxTokens.Add(commentSingleLineToken);
//                     }
//                     else
//                     {
//                         var commentMultiLineToken = ConsumeCommentMultiLine();
//                         _syntaxTokens.Add(commentMultiLineToken);
//                     }
//
//                     break;
//                 case CLanguageFacts.PREPROCESSOR_DIRECTIVE_TRANSITION_CHAR:
//                     var preprocessorDirectiveToken = ConsumePreprocessorDirective();
//                     _syntaxTokens.Add(preprocessorDirectiveToken);
//
//                     if (preprocessorDirectiveToken.TextSpan.GetText() ==
//                         CLanguageFacts.Preprocessor.Directives.INCLUDE)
//                     {
//                         if (TryConsumeLibraryReference(out var libraryReferenceToken) &&
//                             libraryReferenceToken is not null)
//                         {
//                             _syntaxTokens.Add(libraryReferenceToken);
//                         }
//                     }
//
//                     break;
//                 default:
//                     _ = _stringWalker.ReadCharacter();
//                     break;
//             }
//         }
//
//         var endOfFileTextSpan = new TextEditorTextSpan(
//             _stringWalker.PositionIndex,
//             _stringWalker.PositionIndex,
//             (byte)GenericDecorationKind.None,
//             _stringWalker.ResourceUri,
//             _stringWalker.SourceText);
//
//         _syntaxTokens.Add(new EndOfFileToken(endOfFileTextSpan));
//     }
//
//     private NumericLiteralToken ConsumeNumericLiteral()
//     {
//         var entryPositionIndex = _stringWalker.PositionIndex;
//
//         while (!_stringWalker.IsEof)
//         {
//             if (char.IsNumber(_stringWalker.CurrentCharacter))
//             {
//                 _ = _stringWalker.ReadCharacter();
//                 continue;
//             }
//
//             break;
//         }
//
//         var textSpan = new TextEditorTextSpan(
//             entryPositionIndex,
//             _stringWalker.PositionIndex,
//             (byte)GenericDecorationKind.None,
//             _stringWalker.ResourceUri,
//             _stringWalker.SourceText);
//
//         return new NumericLiteralToken(textSpan);
//     }
//
//     private StringLiteralToken ConsumeStringLiteral()
//     {
//         var entryPositionIndex = _stringWalker.PositionIndex;
//
//         // Move past starting '"'
//         _ = _stringWalker.ReadCharacter();
//
//         while (!_stringWalker.IsEof)
//         {
//             if (_stringWalker.CurrentCharacter == CLanguageFacts.STRING_LITERAL_END)
//                 break;
//
//             _ = _stringWalker.ReadCharacter();
//         }
//
//         // Move past ending '"'
//         _ = _stringWalker.ReadCharacter();
//
//         var textSpan = new TextEditorTextSpan(
//             entryPositionIndex,
//             _stringWalker.PositionIndex,
//             (byte)GenericDecorationKind.StringLiteral,
//             _stringWalker.ResourceUri,
//             _stringWalker.SourceText);
//
//         return new StringLiteralToken(textSpan);
//     }
//
//     private ISyntaxToken ConsumeIdentifierOrKeyword()
//     {
//         var entryPositionIndex = _stringWalker.PositionIndex;
//
//         while (!_stringWalker.IsEof)
//         {
//             if (char.IsWhiteSpace(_stringWalker.CurrentCharacter) ||
//                 char.IsPunctuation(_stringWalker.CurrentCharacter) &&
//                     _stringWalker.CurrentCharacter != CLanguageFacts.Punctuation.UNDERSCORE_SPECIAL_CASE)
//             {
//                 break;
//             }
//
//             _ = _stringWalker.ReadCharacter();
//         }
//
//         var textSpan = new TextEditorTextSpan(
//             entryPositionIndex,
//             _stringWalker.PositionIndex,
//             (byte)GenericDecorationKind.None,
//             _stringWalker.ResourceUri,
//             _stringWalker.SourceText);
//
//         var textValue = textSpan.GetText();
//
//         if (CLanguageFacts.Keywords.ALL.Contains(textValue))
//         {
//             var decorationByte = (byte)GenericDecorationKind.Keyword;
//
//             if (CLanguageFacts.Keywords.CONTROL_KEYWORDS.Contains(textValue))
//                 decorationByte = (byte)GenericDecorationKind.KeywordControl;
//
//             textSpan = textSpan with
//             {
//                 DecorationByte = decorationByte,
//             };
//
//             return new KeywordToken(textSpan);
//         }
//
//         return new IdentifierToken(textSpan);
//     }
//
//     private CommentSingleLineToken ConsumeCommentSingleLine()
//     {
//         var entryPositionIndex = _stringWalker.PositionIndex;
//
//         _ = _stringWalker.ReadRange(
//             CLanguageFacts.COMMENT_SINGLE_LINE_STARTING_SUBSTRING.Length);
//
//         while (!_stringWalker.IsEof)
//         {
//             if (_stringWalker.CurrentCharacter == CLanguageFacts.Whitespace.CARRIAGE_RETURN_CHAR ||
//                 _stringWalker.CurrentCharacter == CLanguageFacts.Whitespace.LINE_FEED_CHAR)
//             {
//                 break;
//             }
//
//             _ = _stringWalker.ReadCharacter();
//         }
//
//         var textSpan = new TextEditorTextSpan(
//             entryPositionIndex,
//             _stringWalker.PositionIndex,
//             (byte)GenericDecorationKind.CommentSingleLine,
//             _stringWalker.ResourceUri,
//             _stringWalker.SourceText);
//
//         return new CommentSingleLineToken(textSpan);
//     }
//
//     private CommentMultiLineToken ConsumeCommentMultiLine()
//     {
//         var entryPositionIndex = _stringWalker.PositionIndex;
//
//         _ = _stringWalker.ReadRange(
//             CLanguageFacts.COMMENT_MULTI_LINE_STARTING_SUBSTRING.Length);
//
//         while (!_stringWalker.IsEof)
//         {
//             if (_stringWalker.CurrentCharacter == CLanguageFacts.COMMENT_MULTI_LINE_ENDING_IDENTIFYING_CHAR &&
//                 _stringWalker.PeekCharacter(1) == '/')
//             {
//                 _ = _stringWalker.ReadRange(
//                     CLanguageFacts.COMMENT_MULTI_LINE_ENDING_SUBSTRING.Length);
//
//                 break;
//             }
//
//             _ = _stringWalker.ReadCharacter();
//         }
//
//         var textSpan = new TextEditorTextSpan(
//             entryPositionIndex,
//             _stringWalker.PositionIndex,
//             (byte)GenericDecorationKind.CommentMultiLine,
//             _stringWalker.ResourceUri,
//             _stringWalker.SourceText);
//
//         return new CommentMultiLineToken(textSpan);
//     }
//
//     private PreprocessorDirectiveToken ConsumePreprocessorDirective()
//     {
//         var startOfDirective = _stringWalker.PositionIndex;
//
//         while (!_stringWalker.IsEof)
//         {
//             if (char.IsWhiteSpace(_stringWalker.CurrentCharacter))
//                 break;
//
//             _ = _stringWalker.ReadCharacter();
//         }
//
//         var textSpan = new TextEditorTextSpan(
//             startOfDirective,
//             _stringWalker.PositionIndex,
//             (byte)GenericDecorationKind.PreprocessorDirective,
//             _stringWalker.ResourceUri,
//             _stringWalker.SourceText);
//
//         return new PreprocessorDirectiveToken(textSpan);
//     }
//
//     private bool TryConsumeLibraryReference(
//         out LibraryReferenceToken? libraryReferenceToken)
//     {
//         var entryPositionIndex = _stringWalker.PositionIndex;
//
//         while (!_stringWalker.IsEof)
//         {
//             if (char.IsWhiteSpace(_stringWalker.CurrentCharacter))
//             {
//                 _ = _stringWalker.ReadCharacter();
//                 continue;
//             }
//
//             break;
//         }
//
//         var libraryReferenceStartingDelimiterCharacterPositionIndex = _stringWalker.PositionIndex;
//
//         char characterToMatch;
//         Func<TextEditorTextSpan, LibraryReferenceToken> libraryReferenceFactory;
//
//         if (_stringWalker.CurrentCharacter == CLanguageFacts.LIBRARY_REFERENCE_ABSOLUTE_PATH_STARTING_CHAR)
//         {
//             characterToMatch = '>';
//
//             libraryReferenceFactory = textSpan =>
//                 new LibraryReferenceToken(
//                     textSpan,
//                     true);
//         }
//         else if (_stringWalker.CurrentCharacter == CLanguageFacts.LIBRARY_REFERENCE_RELATIVE_PATH_STARTING_CHAR)
//         {
//             characterToMatch = '"';
//
//             libraryReferenceFactory = textSpan =>
//                 new LibraryReferenceToken(
//                     textSpan,
//                     false);
//         }
//         else
//         {
//             _ = _stringWalker.BacktrackRange(
//                 _stringWalker.PositionIndex - entryPositionIndex);
//
//             libraryReferenceToken = null;
//             return false;
//         }
//
//         // Move past the library reference starting delimiter character
//         _ = _stringWalker.ReadCharacter();
//
//         while (!_stringWalker.IsEof)
//         {
//             if (_stringWalker.CurrentCharacter == characterToMatch)
//                 break;
//
//             _ = _stringWalker.ReadCharacter();
//         }
//
//         // Move past the library reference ending delimiter character
//         _ = _stringWalker.ReadCharacter();
//
//         var textSpan = new TextEditorTextSpan(
//             libraryReferenceStartingDelimiterCharacterPositionIndex,
//             _stringWalker.PositionIndex,
//             (byte)GenericDecorationKind.StringLiteral,
//             _stringWalker.ResourceUri,
//             _stringWalker.SourceText);
//
//         libraryReferenceToken = libraryReferenceFactory.Invoke(
//             textSpan);
//
//         return true;
//     }
//
//     private PlusToken ConsumePlus()
//     {
//         var entryPositionIndex = _stringWalker.PositionIndex;
//
//         _ = _stringWalker.ReadCharacter();
//
//         var textSpan = new TextEditorTextSpan(
//             entryPositionIndex,
//             _stringWalker.PositionIndex,
//             (byte)GenericDecorationKind.None,
//             _stringWalker.ResourceUri,
//             _stringWalker.SourceText);
//
//         return new PlusToken(textSpan);
//     }
//
//     private EqualsToken ConsumeEquals()
//     {
//         var entryPositionIndex = _stringWalker.PositionIndex;
//
//         _ = _stringWalker.ReadCharacter();
//
//         var textSpan = new TextEditorTextSpan(
//             entryPositionIndex,
//             _stringWalker.PositionIndex,
//             (byte)GenericDecorationKind.None,
//             _stringWalker.ResourceUri,
//             _stringWalker.SourceText);
//
//         return new EqualsToken(textSpan);
//     }
//
//     private StatementDelimiterToken ConsumeStatementDelimiter()
//     {
//         var entryPositionIndex = _stringWalker.PositionIndex;
//
//         _ = _stringWalker.ReadCharacter();
//
//         var textSpan = new TextEditorTextSpan(
//             entryPositionIndex,
//             _stringWalker.PositionIndex,
//             (byte)GenericDecorationKind.None,
//             _stringWalker.ResourceUri,
//             _stringWalker.SourceText);
//
//         return new StatementDelimiterToken(textSpan);
//     }
//
//     private OpenParenthesisToken ConsumeOpenParenthesis()
//     {
//         var entryPositionIndex = _stringWalker.PositionIndex;
//
//         _ = _stringWalker.ReadCharacter();
//
//         var textSpan = new TextEditorTextSpan(
//             entryPositionIndex,
//             _stringWalker.PositionIndex,
//             (byte)GenericDecorationKind.None,
//             _stringWalker.ResourceUri,
//             _stringWalker.SourceText);
//
//         return new OpenParenthesisToken(textSpan);
//     }
//
//     private CloseParenthesisToken ConsumeCloseParenthesis()
//     {
//         var entryPositionIndex = _stringWalker.PositionIndex;
//
//         _ = _stringWalker.ReadCharacter();
//
//         var textSpan = new TextEditorTextSpan(
//             entryPositionIndex,
//             _stringWalker.PositionIndex,
//             (byte)GenericDecorationKind.None,
//             _stringWalker.ResourceUri,
//             _stringWalker.SourceText);
//
//         return new CloseParenthesisToken(textSpan);
//     }
//
//     private OpenBraceToken ConsumeOpenBrace()
//     {
//         var entryPositionIndex = _stringWalker.PositionIndex;
//
//         _ = _stringWalker.ReadCharacter();
//
//         var textSpan = new TextEditorTextSpan(
//             entryPositionIndex,
//             _stringWalker.PositionIndex,
//             (byte)GenericDecorationKind.None,
//             _stringWalker.ResourceUri,
//             _stringWalker.SourceText);
//
//         return new OpenBraceToken(textSpan);
//     }
//
//     private CloseBraceToken ConsumeCloseBrace()
//     {
//         var entryPositionIndex = _stringWalker.PositionIndex;
//
//         _ = _stringWalker.ReadCharacter();
//
//         var textSpan = new TextEditorTextSpan(
//             entryPositionIndex,
//             _stringWalker.PositionIndex,
//             (byte)GenericDecorationKind.None,
//             _stringWalker.ResourceUri,
//             _stringWalker.SourceText);
//
//         return new CloseBraceToken(textSpan);
//     }
// }