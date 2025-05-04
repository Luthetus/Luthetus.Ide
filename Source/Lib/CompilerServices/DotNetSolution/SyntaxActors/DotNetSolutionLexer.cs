using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.Extensions.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax;
using Luthetus.CompilerServices.DotNetSolution.Facts;
using Luthetus.CompilerServices.Xml.Html.Decoration;

namespace Luthetus.CompilerServices.DotNetSolution.SyntaxActors;

public class DotNetSolutionLexer
{
	public static LexerKeywords LexerKeywords = LexerKeywords.Empty;
	
	private StringWalker _stringWalker;
	
	private readonly List<SyntaxToken> _syntaxTokenList = new();
	public List<SyntaxToken> SyntaxTokenList => _syntaxTokenList;

    public DotNetSolutionLexer(ResourceUri resourceUri, string sourceText)
    {
    	ResourceUri = resourceUri;
    	SourceText = sourceText;
    	
    	_stringWalker = new(ResourceUri, SourceText);
    }
    
    public ResourceUri ResourceUri { get; set; }
    public string SourceText { get; set; }

    public void Lex()
    {
        while (!_stringWalker.IsEof)
        {
            if (_stringWalker.PeekForSubstring(LexSolutionFacts.Header.FORMAT_VERSION_START_TOKEN))
                LexHeaderFormatVersion();
            else if (_stringWalker.PeekForSubstring(LexSolutionFacts.Header.HASHTAG_VISUAL_STUDIO_VERSION_START_TOKEN))
                LexHashtagVisualStudioVersion();
            else if (_stringWalker.PeekForSubstring(LexSolutionFacts.Header.EXACT_VISUAL_STUDIO_VERSION_START_TOKEN))
                LexExactVisualStudioVersion();
            else if (_stringWalker.PeekForSubstring(LexSolutionFacts.Header.MINIMUM_VISUAL_STUDIO_VERSION_START_TOKEN))
                LexMinimumVisualStudioVersion();
            else if (_stringWalker.PeekForSubstring(LexSolutionFacts.Project.PROJECT_DEFINITION_START_TOKEN))
                LexProjectDefinitionEntry();
            else if (_stringWalker.PeekForSubstring(LexSolutionFacts.Global.START_TOKEN))
                LexGlobal();

            _ = _stringWalker.ReadCharacter();
        }

        var endOfFileTextSpan = new TextEditorTextSpan(
            _stringWalker.PositionIndex,
            _stringWalker.PositionIndex,
            (byte)GenericDecorationKind.None,
            _stringWalker.ResourceUri,
            _stringWalker.SourceText);

        _syntaxTokenList.Add(new SyntaxToken(SyntaxKind.EndOfFileToken, endOfFileTextSpan));
    }

    private void LexHeaderFormatVersion()
    {
        var startingPosition = _stringWalker.PositionIndex;

        _ = _stringWalker.ReadRange(LexSolutionFacts.Header.FORMAT_VERSION_START_TOKEN.Length);

        var formatVersionTextSpan = new TextEditorTextSpan(startingPosition, _stringWalker, (byte)HtmlDecorationKind.AttributeName);
        _syntaxTokenList.Add(new SyntaxToken(SyntaxKind.AssociatedNameToken, formatVersionTextSpan));

        _ = _stringWalker.ReadWhitespace();

        var numericLiteralTextSpan = _stringWalker.ReadUnsignedNumericLiteral();
        var associatedValueToken = new SyntaxToken(SyntaxKind.AssociatedValueToken, numericLiteralTextSpan with
        {
            DecorationByte = (byte)HtmlDecorationKind.AttributeValue
        });

        _syntaxTokenList.Add(associatedValueToken);
    }

    private void LexHashtagVisualStudioVersion()
    {
        var startingPosition = _stringWalker.PositionIndex;

        _ = _stringWalker.ReadRange(LexSolutionFacts.Header.HASHTAG_VISUAL_STUDIO_VERSION_START_TOKEN.Length);

        var vSVersionTextSpan = new TextEditorTextSpan(startingPosition, _stringWalker, (byte)HtmlDecorationKind.AttributeName);
        _syntaxTokenList.Add(new SyntaxToken(SyntaxKind.AssociatedNameToken, vSVersionTextSpan));

        _ = _stringWalker.ReadWhitespace();

        var numericLiteralTextSpan = _stringWalker.ReadUnsignedNumericLiteral();
        var associatedValueToken = new SyntaxToken(SyntaxKind.AssociatedValueToken, numericLiteralTextSpan with
        {
            DecorationByte = (byte)HtmlDecorationKind.AttributeValue
        });

        _syntaxTokenList.Add(associatedValueToken);
    }

    private void LexExactVisualStudioVersion()
    {
        var stringStartingPosition = _stringWalker.PositionIndex;

        _ = _stringWalker.ReadRange(LexSolutionFacts.Header.EXACT_VISUAL_STUDIO_VERSION_START_TOKEN.Length);

        var versionStringTextSpan = new TextEditorTextSpan(stringStartingPosition, _stringWalker, (byte)HtmlDecorationKind.AttributeName);
        _syntaxTokenList.Add(new SyntaxToken(SyntaxKind.AssociatedNameToken, versionStringTextSpan));

        _ = _stringWalker.ReadWhitespace();

        var versionIdentifierStartingPosition = _stringWalker.PositionIndex;

        while (!_stringWalker.IsEof)
        {
            if (!char.IsDigit(_stringWalker.CurrentCharacter) &&
                _stringWalker.CurrentCharacter != '.')
            {
                break;
            }

            _ = _stringWalker.ReadCharacter();
        }

        var versionTextSpan = new TextEditorTextSpan(versionIdentifierStartingPosition, _stringWalker, (byte)HtmlDecorationKind.AttributeValue);
        _syntaxTokenList.Add(new SyntaxToken(SyntaxKind.AssociatedValueToken, versionTextSpan));
    }

    private void LexMinimumVisualStudioVersion()
    {
        var stringStartingPosition = _stringWalker.PositionIndex;

        _ = _stringWalker.ReadRange(LexSolutionFacts.Header.MINIMUM_VISUAL_STUDIO_VERSION_START_TOKEN.Length);

        var versionStringTextSpan = new TextEditorTextSpan(stringStartingPosition, _stringWalker, (byte)HtmlDecorationKind.AttributeName);
        _syntaxTokenList.Add(new SyntaxToken(SyntaxKind.AssociatedNameToken, versionStringTextSpan));

        _ = _stringWalker.ReadWhitespace();

        var versionIdentifierStartingPosition = _stringWalker.PositionIndex;

        while (!_stringWalker.IsEof)
        {
            if (!char.IsDigit(_stringWalker.CurrentCharacter) &&
                _stringWalker.CurrentCharacter != '.')
            {
                break;
            }

            _ = _stringWalker.ReadCharacter();
        }

        var versionTextSpan = new TextEditorTextSpan(versionIdentifierStartingPosition, _stringWalker, (byte)HtmlDecorationKind.AttributeValue);
        _syntaxTokenList.Add(new SyntaxToken(SyntaxKind.AssociatedValueToken, versionTextSpan));
    }

    public void LexProjectDefinitionEntry()
    {
        var startPosition = _stringWalker.PositionIndex;
        _ = _stringWalker.ReadRange(LexSolutionFacts.Project.PROJECT_DEFINITION_START_TOKEN.Length);

        var textSpan = new TextEditorTextSpan(startPosition, _stringWalker, (byte)HtmlDecorationKind.TagName);
        _syntaxTokenList.Add(new SyntaxToken(SyntaxKind.OpenAssociatedGroupToken, textSpan));

        while (!_stringWalker.IsEof)
        {
            if (_stringWalker.CurrentCharacter == '"')
            {
                if (_stringWalker.NextCharacter == '{')
                    LexGuid();
                else
                    LexString();
            }
            else if (_stringWalker.PeekForSubstring(LexSolutionFacts.Project.PROJECT_DEFINITION_END_TOKEN))
            {
                startPosition = _stringWalker.PositionIndex;
                _stringWalker.ReadRange(LexSolutionFacts.Project.PROJECT_DEFINITION_END_TOKEN.Length);

                textSpan = new TextEditorTextSpan(startPosition, _stringWalker, (byte)HtmlDecorationKind.TagName);
                _syntaxTokenList.Add(new SyntaxToken(SyntaxKind.CloseAssociatedGroupToken, textSpan));
                break;
            }

            _ = _stringWalker.ReadCharacter();
        }
    }

    public void LexGuid()
    {
        // "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"
        // ^
        _ = _stringWalker.ReadCharacter();
        _ = _stringWalker.ReadCharacter();

        // "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"
        //   ^
        var startPosition = _stringWalker.PositionIndex;
        _ = _stringWalker.ReadUntil('}');

        // "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"
        //                                       ^
        var guidTextSpan = new TextEditorTextSpan(startPosition, _stringWalker, (byte)HtmlDecorationKind.AttributeValue);

        // guidTextSpan.GetText() == "FAE04EC0-301F-11D3-BF4B-00C04F79EFBC"
        _syntaxTokenList.Add(new SyntaxToken(SyntaxKind.AssociatedValueToken, guidTextSpan));

        _ = _stringWalker.ReadCharacter();
        _ = _stringWalker.ReadCharacter();

        // "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"
        //                                         ^
    }

    public void LexString()
    {
        // "ConsoleApp2"
        // ^
        _ = _stringWalker.ReadCharacter();

        // "ConsoleApp2"
        //  ^
        var startPosition = _stringWalker.PositionIndex;
        _ = _stringWalker.ReadUntil('"');

        // "ConsoleApp2"
        //             ^
        var textSpan = new TextEditorTextSpan(startPosition, _stringWalker, (byte)HtmlDecorationKind.AttributeValue);

        // textSpan.GetText() == "ConsoleApp2"
        _syntaxTokenList.Add(new SyntaxToken(SyntaxKind.AssociatedValueToken, textSpan));

        _ = _stringWalker.ReadCharacter();

        // "ConsoleApp2"
        //              ^
    }

    public void LexGlobal()
    {
        var startPosition = _stringWalker.PositionIndex;
        _ = _stringWalker.ReadRange(LexSolutionFacts.Global.START_TOKEN.Length);

        var textSpan = new TextEditorTextSpan(startPosition, _stringWalker, (byte)HtmlDecorationKind.TagName);
        _syntaxTokenList.Add(new SyntaxToken(SyntaxKind.OpenAssociatedGroupToken, textSpan));

        bool BreakPredicate() => _stringWalker.PeekForSubstring(LexSolutionFacts.Global.END_TOKEN);

        while (!_stringWalker.IsEof)
        {
            if (BreakPredicate())
            {
                startPosition = _stringWalker.PositionIndex;
                _stringWalker.ReadRange(LexSolutionFacts.Global.END_TOKEN.Length);

                textSpan = new TextEditorTextSpan(startPosition, _stringWalker, (byte)HtmlDecorationKind.TagName);
                _syntaxTokenList.Add(new SyntaxToken(SyntaxKind.CloseAssociatedGroupToken, textSpan));
                break;
            }
            else if (_stringWalker.PeekForSubstring(LexSolutionFacts.GlobalSection.START_TOKEN))
            {
                LexGlobalSection(BreakPredicate);
            }

            _ = _stringWalker.ReadCharacter();
        }
    }

    public void LexGlobalSection(Func<bool> outerLoopBreakPredicate)
    {
        var startPosition = _stringWalker.PositionIndex;
        _ = _stringWalker.ReadRange(LexSolutionFacts.GlobalSection.START_TOKEN.Length);

        var textSpan = new TextEditorTextSpan(startPosition, _stringWalker, (byte)HtmlDecorationKind.TagName);
        _syntaxTokenList.Add(new SyntaxToken(SyntaxKind.OpenAssociatedGroupToken, textSpan));

        bool BreakPredicate() => _stringWalker.PeekForSubstring(LexSolutionFacts.GlobalSection.END_TOKEN);

        while (!_stringWalker.IsEof)
        {
            if (BreakPredicate())
            {
                startPosition = _stringWalker.PositionIndex;
                _stringWalker.ReadRange(LexSolutionFacts.GlobalSection.END_TOKEN.Length);

                textSpan = new TextEditorTextSpan(startPosition, _stringWalker, (byte)HtmlDecorationKind.TagName);
                _syntaxTokenList.Add(new SyntaxToken(SyntaxKind.CloseAssociatedGroupToken, textSpan));
                break;
            }
            else if (outerLoopBreakPredicate.Invoke())
            {
                break;
            }
            else if (_stringWalker.CurrentCharacter == '(')
            {
                _ = _stringWalker.ReadCharacter();
                var globalSectionParameterStartPosition = _stringWalker.PositionIndex;
                var globalSectionParameter = _stringWalker.ReadUntil(')');

                textSpan = new TextEditorTextSpan(globalSectionParameterStartPosition, _stringWalker, (byte)HtmlDecorationKind.AttributeValue);
                _syntaxTokenList.Add(new SyntaxToken(SyntaxKind.AssociatedValueToken, textSpan));

                // START_TOKEN_ORDER: 'preSolution' OR 'postSolution'
                {
                    _ = _stringWalker.ReadUntil('=');

                    if (_stringWalker.IsEof)
                        break;

                    _ = _stringWalker.ReadCharacter();
                    _ = _stringWalker.ReadWhitespace();

                    var startOrderTuple = _stringWalker.ReadWordTuple();
                    _syntaxTokenList.Add(new SyntaxToken(SyntaxKind.AssociatedValueToken, startOrderTuple.textSpan with
                    {
                        DecorationByte = (byte)HtmlDecorationKind.AttributeValue
                    }));
                }

                LexPropertyNameAndValuePairs(() => BreakPredicate() || outerLoopBreakPredicate.Invoke());
            }

            _ = _stringWalker.ReadCharacter();
        }
    }

    private void LexPropertyNameAndValuePairs(Func<bool> outerLoopBreakPredicate)
    {
        while (!_stringWalker.IsEof)
        {
            _ = _stringWalker.ReadWhitespace();

            if (outerLoopBreakPredicate.Invoke())
            {
                _stringWalker.BacktrackCharacter();
                break;
            }

            var propertyNameStartPosition = _stringWalker.PositionIndex;
            var name = _stringWalker.ReadUntil('=');

            var nameNoWhitespace = name.TrimEnd();

            var nameTrailingWhitespaceCount = name.Length - nameNoWhitespace.Length;

            var propertyNameTextSpan = new TextEditorTextSpan(propertyNameStartPosition, _stringWalker, (byte)HtmlDecorationKind.AttributeName);
            propertyNameTextSpan = propertyNameTextSpan with
            {
                EndExclusiveIndex = propertyNameTextSpan.EndExclusiveIndex - nameTrailingWhitespaceCount
            };

            _syntaxTokenList.Add(new SyntaxToken(SyntaxKind.AssociatedNameToken, propertyNameTextSpan));

            if (_stringWalker.IsEof)
                return;

            _ = _stringWalker.ReadCharacter();
            _ = _stringWalker.ReadWhitespace();

            var propertyValueStartPosition = _stringWalker.PositionIndex;
            var value = _stringWalker.ReadLine();

            var valueNoWhitespace = value.TrimEnd();

            var valueTrailingWhitespaceCount = value.Length - valueNoWhitespace.Length;

            var propertyValueTextSpan = new TextEditorTextSpan(propertyValueStartPosition, _stringWalker, (byte)HtmlDecorationKind.AttributeValue);

            propertyValueTextSpan = propertyValueTextSpan with
            {
                EndExclusiveIndex = propertyValueTextSpan.EndExclusiveIndex - valueTrailingWhitespaceCount
            };

            _syntaxTokenList.Add(new SyntaxToken(SyntaxKind.AssociatedValueToken, propertyValueTextSpan));
        }
    }
}
